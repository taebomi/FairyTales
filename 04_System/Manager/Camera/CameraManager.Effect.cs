using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public partial class CameraManager
{
    #region 베이스 카메라 트랜스폼 값 컨트롤

    private float _currentNormalSize; // 현재 카메라의 기본 사이즈 ( 이펙트 종료 시 복귀용도 )

    public void ControlMainCamera(Vector2 pos, float size)
    {
        _currentNormalSize = _currentHalfHeight = size;
        _desiredPosition = pos;
    }

    #endregion

    #region 오버레이 카메라

    public void ActivateOverlayCamera(bool willActivate)
    {
        OverlayCamera.enabled = willActivate;
    }

    public void ControlOverlayCamera(Vector2 pos, float size)
    {
        OverlayCamera.transform.position = new Vector3(pos.x, pos.y, CameraZPosition);
        OverlayCamera.orthographicSize = size;
    }

    private void SyncOverlayCameraToMainCamera()
    {
        OverlayCamera.transform.position = MainCamera.transform.position;
        OverlayCamera.orthographicSize = MainCamera.orthographicSize;
    }

    #endregion


    #region 흔들기 효과

    private CancellationTokenSource _shakeCts = new();
    private int _shakeTweenId = Animator.StringToHash(nameof(_shakeTweenId));

    public async UniTaskVoid ShakeCamera(float strength, float duration)
    {
        _shakeCts.CancelAndDispose();
        _shakeCts = new CancellationTokenSource();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, _shakeCts.Token);

        var shakeVector = Vector2.zero;
        var tweener = DOTween.Shake(() => shakeVector, x => shakeVector = x,
            duration, strength, fadeOut: false, ignoreZAxis: true).SetId(_shakeTweenId).Play();
        while (tweener.IsPlaying())
        {
            await UniTask.Yield(PlayerLoopTiming.PreLateUpdate, cts.Token);
            _desiredPosition += shakeVector;
        }
    }

    public void StopShakingCamera()
    {
        DOTween.Kill(_shakeTweenId);
        _shakeCts.Cancel();
    }

    #endregion

    #region 포스트 프로세싱

    [SerializeField] private Volume volume;
    private ColorAdjustments _colorAdjustments;
    private FilmGrain _filmGrain;

    private void InitializePostProcessing(bool isFirst)
    {
        if (isFirst)
        {
            volume.profile.TryGet(out _colorAdjustments);
            volume.profile.TryGet(out _filmGrain);
        }

        volume.enabled = false;

        _colorAdjustments.active = false;
        _colorAdjustments.saturation.value = 0f;
        _colorAdjustments.colorFilter.value = Color.white;

        _filmGrain.active = false;
        _filmGrain.intensity.value = 0f;
    }

    // 포스트 프로세스 사용 시 컴포넌트 활성화 / 비활성화
    private void ActivateVolumeComponent(bool willActivate, params VolumeComponent[] components)
    {
        foreach (var volumeComponent in components)
        {
            volumeComponent.active = willActivate;
        }

        if (willActivate)
        {
            volume.enabled = true;
        }
        else
        {
            if (volume.profile.components.All(volumeComponent => !volumeComponent.active))
            {
                volume.enabled = false;
            }
        }
    }

    #region 회상 효과

    private CancellationTokenSource _reminiscenceCts = new();

    /// 회상 효과 ( 화면 회색 + 노랗게 )
    /// <param name="value">0, 비활성화 / 1, 최대치 적용</param>
    /// <param name="duration">적용되는 데 걸리는 시간</param>
    public async UniTaskVoid PlayEffect_Reminiscence(float value, float duration)
    {
        _reminiscenceCts.CancelAndDispose();
        _reminiscenceCts = new CancellationTokenSource();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, _reminiscenceCts.Token);

        if (!Mathf.Approximately(value, 0f))
        {
            ActivateVolumeComponent(true, _colorAdjustments, _filmGrain);

            await DOTween.Sequence()
                .Join(DOVirtual.Color(_colorAdjustments.colorFilter.value, new Color(1f, 0.85f, 0f), duration,
                    color => _colorAdjustments.colorFilter.value = color))
                .Join(DOTween.To(x => _colorAdjustments.saturation.value = x,
                    _colorAdjustments.saturation.value, -65f, duration))
                .Join(DOTween.To(x => _filmGrain.intensity.value = x,
                    _filmGrain.intensity.value, 1f, duration))
                .SetUpdate(true).Play().ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cts.Token);
        }
        else
        {
            await DOTween.Sequence()
                .Join(DOVirtual.Color(_colorAdjustments.colorFilter.value, Color.white, duration,
                    color => _colorAdjustments.colorFilter.value = color))
                .Join(DOTween.To(x => _colorAdjustments.saturation.value = x,
                    _colorAdjustments.saturation.value, 0f, duration))
                .Join(DOTween.To(x => _filmGrain.intensity.value = x,
                    _filmGrain.intensity.value, 0f, duration))
                .SetUpdate(true).Play().ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cts.Token);

            ActivateVolumeComponent(false, _colorAdjustments, _filmGrain);
        }
    }

    #endregion

    #endregion


    #region 플레이어 데쉬 효과

    private const float DashEffectZoomOutDuration = PlayerScript.DashDuration / 3; // 데쉬 줌 인 지속시간
    private const float DashEffectZoomInDuration = PlayerScript.DashDuration * 2 / 3; // 데쉬 줌 아웃 지속시간
    private const float DashEffectCameraZoomHeight = 0.25f; // 데쉬 시 변경될 사이즈값

    private CancellationTokenSource _dashEffectCts = new(); // 데쉬 효과 unitask 토큰


    /// <summary>
    /// 데쉬 시 화면 줌 아웃, 인 효과 실행
    /// </summary>
    public async UniTaskVoid Play_DashEffect()
    {
        _dashEffectCts.CancelAndDispose();
        _dashEffectCts = new CancellationTokenSource();

        // 카메라 줌 아웃 및 이미 줌 아웃/인 중이었을 경우 빨리 끝나게 되므로 동시에 줌아웃 시간만큼 대기
        await UniTask.WhenAll(
            DashEffect_ZoomOutCamera(_dashEffectCts.Token),
            UniTask.Delay(TimeSpan.FromSeconds(DashEffectZoomOutDuration), cancellationToken: _dashEffectCts.Token)
        );
        // 위 작업이 완료되고 난 이후에 카메라 줌인하여 원상복구
        await DashEffect_ZoomInCamera(_dashEffectCts.Token);
    }

    /// <summary>
    /// 데쉬 초반에 카메라 줌 아웃 효과
    /// </summary>
    private async UniTask DashEffect_ZoomOutCamera(CancellationToken token)
    {
        var desiredHeight = _currentNormalSize + DashEffectCameraZoomHeight;
        // 이전에 이미 진행중이었을 수 있으므로 현재 카메라 사이즈와의 차이값을 이용해 비율 구함
        var ratio = (_currentHalfHeight - _currentNormalSize) / DashEffectCameraZoomHeight;
        // 위에서 구한 비율로 현재 줌 아웃이 어느 시점까지 진행되었는지 구함
        var currentTime = ratio * DashEffectZoomOutDuration;
        while (ratio <= 1f)
        {
            currentTime += Time.deltaTime;
            ratio = currentTime / DashEffectZoomOutDuration;
            _currentHalfHeight = Mathf.Lerp(_currentNormalSize, desiredHeight, ratio);
            await UniTask.Yield(token);
        }

        _currentHalfHeight = desiredHeight;
    }

    /// <summary>
    /// 데쉬 줌 아웃 효과 이후 다시 줌인되는 효과
    /// </summary>
    private async UniTask DashEffect_ZoomInCamera(CancellationToken token)
    {
        // 무조건 처음부터, 화면 최대값부터 시작됨
        var startedHeight = _currentNormalSize + 0.25f;
        var currentTime = 0f;
        while (currentTime < DashEffectZoomInDuration)
        {
            currentTime += Time.deltaTime;
            var ratio = currentTime / DashEffectZoomInDuration;
            _currentHalfHeight = Mathf.Lerp(startedHeight, _currentNormalSize, ratio);
            await UniTask.Yield(token);
        }

        _currentHalfHeight = _currentNormalSize;
    }

    #endregion
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private CancellationTokenSource _disableCts = new();

    protected override void AwakeAfter()
    {
        InitializeOverlayFade();
        InitiateGameOverScreen();
        InitializeCameraFade();
    }

    private void OnDisable()
    {
        _disableCts.Cancel();
    }

    #region 오버레이 페이드

    [SerializeField] private CanvasGroup overlayFadeCanvasGroup;
    [SerializeField] private RawImage overlayFadeImage;
    private CancellationTokenSource _overlayFadeCts = new ();

    private void InitializeOverlayFade()
    {
        overlayFadeCanvasGroup.alpha = 0f;
        overlayFadeImage.color = Color.black;
    }

    public async UniTask FadeOverlay(float desiredValue, float duration)
    {
        _overlayFadeCts.CancelAndDispose();
        _overlayFadeCts = new CancellationTokenSource();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, _overlayFadeCts.Token);

        await overlayFadeCanvasGroup.DOFade(desiredValue, duration).Play().WithCancellation(cts.Token);
    }

    public void SetOverlayFadeColor(Color color)
    {
        overlayFadeImage.color = color;
    }

    #region 카메라 페이드

    [SerializeField] private CanvasGroup cameraFadeCanvasGroup;
    private CancellationTokenSource _cameraFadeCts;

    private void InitializeCameraFade()
    {
        cameraFadeCanvasGroup.alpha = 0f;
    }

    public async UniTask FadeCamera(float fadeValue, float duration)
    {
        _cameraFadeCts.CancelAndDispose();
        _cameraFadeCts = new CancellationTokenSource();
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_disableCts.Token, _cameraFadeCts.Token);
        
        await cameraFadeCanvasGroup.DOFade(fadeValue, duration).Play().WithCancellation(cts.Token);
    }

    #endregion

    #endregion

    #region 게임오버 스크린

    [SerializeField] private CanvasGroup gameOverCanvasGroup;
    [SerializeField] private TMP_Text gameOverText;

    private Sequence _gameOverSequence;

    private void InitiateGameOverScreen()
    {
        gameOverCanvasGroup.alpha = 0f;
        _gameOverSequence = DOTween.Sequence()
            .Join(gameOverCanvasGroup.DOFade(1f, 1.5f))
            .Join(DOTween.To(x => gameOverText.characterSpacing = x, 0f, 20f, 3f))
            .Join(gameOverText.DOFontSize(140f, 3f).From(120f))
            .SetEase(Ease.InOutSine)
            .SetAutoKill(false);
    }

    public async UniTask ActivateGameOverScreen()
    {
        _gameOverSequence.Restart();
        await _gameOverSequence.AwaitForComplete();
    }

    public void DeactivateGameOverScree()
    {
        gameOverCanvasGroup.alpha = 0f;
    }

    #endregion
}
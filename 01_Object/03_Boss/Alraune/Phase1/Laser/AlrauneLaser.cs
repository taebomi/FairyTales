using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FairyTales.Layer;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlrauneLaser : PoolableObject<AlrauneLaser>
{
    // 스프라이트
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite warnSprite;
    [SerializeField] private Sprite attackSprite;

    // 물리
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D bc;

    // 레이저 끝 이펙트
    [SerializeField] private ParticleSystem endEffectParticleSystem;
    [SerializeField] private Light2D endEffectLight;
    
    private float _length, _width;

    // 레이저 및 끝 지점 빛 크기 동기화
    private void Update()
    {
        sr.transform.localScale = new Vector3(_width, _length, 1f);
        endEffectLight.intensity = _width;
    }

    private CancellationTokenSource _disableCts;

    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _disableCts.Cancel();
    }


    public void SetRotation(float rotation)
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public void SetRotation(Vector3 upDirection)
    {
        transform.up = upDirection;
    }


    private CancellationTokenSource _laserStateCts = new();

    /// <summary>
    /// 경고 레이저 생성
    /// </summary>
    public void Warn()
    {
        _laserStateCts.Cancel();
        _laserStateCts = new CancellationTokenSource();

        endEffectLight.enabled = false;
        bc.enabled = false;
        sr.sprite = warnSprite;
        sr.sortingOrder = 10;

        _length = 25f;
        DOTween
            .To(x => _width = x, 0f, 1f, 0.5f)
            .Play()
            .WithCancellation(_laserStateCts.Token);
    }

    /// <summary>
    /// 공격 레이저 생성
    /// </summary>
    /// <param name="aimLock"></param>
    public void Attack(bool aimLock = false)
    {
        _laserStateCts.Cancel();
        _laserStateCts = new CancellationTokenSource();

        endEffectLight.enabled = true;
        bc.enabled = true;
        sr.sprite = attackSprite;
        sr.sortingOrder = 0;
        endEffectParticleSystem.Play(true);

        SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Laser_Shot).Forget();
        AdjustLengthToWall(aimLock).Forget();

        // 레이저 작았다 커졌다 반복효과
        DOTween
            .To(x => _width = x, 1f, 1.25f, 0.25f)
            .SetLoops(-1)
            .SetEase(Ease.InOutSine)
            .Play()
            .WithCancellation(_laserStateCts.Token);
    }

    /// <summary>
    /// 레이저 제거
    /// </summary>
    public async UniTaskVoid Remove()
    {
        _laserStateCts.Cancel();

        var fadingTweener = DOTween.To(x => _width = x, _width, 0f, 0.25f).Play();

        endEffectParticleSystem.Stop(true);

        while (endEffectParticleSystem.IsAlive(true) || fadingTweener.IsActive())
        {
            await UniTask.Yield();
        }

        ManagedPool.Release(this);
    }


    private async UniTaskVoid AdjustLengthToWall(bool aimLock)
    {
        var tr = transform;
        var lastUpDirection = tr.up;
        var hitPoint = tr.position + lastUpDirection;
        while (true)
        {
            if (aimLock)
            {
                tr.up = hitPoint - tr.position;
            }

            var hit = Physics2D.Raycast(transform.position, tr.up, 20f,
                LayerCache.GetLayerMask(LayerName.Wall));

            if (hit)
            {
                _length = hit.distance + 0.15f;
            }

            hitPoint = transform.position + tr.up * _length;
            endEffectParticleSystem.transform.SetPositionAndRotation(hitPoint, Quaternion.identity);
            await UniTask.Yield(_disableCts.Token);
        }
    }


    #region 플레이어 추적

    private CancellationTokenSource _chasingPlayerCts = new();

    public async UniTaskVoid ChasePlayer(bool willEnable = true)
    {
        _chasingPlayerCts.Cancel();
        if (!willEnable)
        {
            return;
        }

        _chasingPlayerCts = new CancellationTokenSource();

        var cts = CancellationTokenSource.CreateLinkedTokenSource(_chasingPlayerCts.Token, _disableCts.Token);
        
        // inoutsin그래프의 형태로 속도 조절
        while (true)
        {
            var dir = (Vector2)PlayerScript.Instance.GetFlyingPosition() - rb.position;
            var desiredRotation = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg + 180f;
            var currentDelta = Mathf.Abs(Mathf.DeltaAngle(desiredRotation, rb.rotation));
            var ratio = Mathf.Clamp(currentDelta / 30f, 0f, 1f);
            var maxDelta = Mathf.Cos(Mathf.PI * -ratio) * -0.4f + 0.55f;
            rb.MoveRotation(Mathf.MoveTowardsAngle(rb.rotation, desiredRotation, maxDelta));
            
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);
        }
    }

    #endregion
}
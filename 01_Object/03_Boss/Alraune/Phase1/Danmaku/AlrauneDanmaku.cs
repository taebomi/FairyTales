using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AlrauneDanmaku : PoolableObject<AlrauneDanmaku>
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Light2D light2D;

    [SerializeField] private Sprite[] danmakuSprites;

    private float _lightWavingTime;
    private const float LightWavingSpeed = 4f;

    #region 색상 관련 고정값

    private readonly Color[] _lightColors =
    {
        new(1f, 0.2216981f, 0.2937847f),
        new(0.9734966f, 1f, 0.3349057f),
        new(0.5235849f, 0.7869875f, 1f),
        new(0.7764706f, 0.345098f, 1f)
    };

    public enum DanmakuColor
    {
        Red,
        Yellow,
        Blue,
        Purple,
    }

    #endregion


    private void Update()
    {
        if (light2D.intensity >= 1f)
        {
            light2D.intensity = 1.125f + Mathf.Sin(_lightWavingTime) * 0.125f;
            _lightWavingTime += Time.deltaTime * LightWavingSpeed;
        }
    }

    // 변동된 값들 초기화
    private void OnDisable()
    {
        _lightWavingTime = 0f;
    }
    
    /// <summary>
    /// 탄막 빛, 스프라이트 0 -> 1로 서서히 변경
    /// </summary>
    public async UniTaskVoid FadeOut()
    {
        var fadingValue = 0f;
        const float fadingSpeed = 2f;
        var color = sr.color;
        while (fadingValue < 1f)
        {
            color.a = fadingValue;
            sr.color = color;
            light2D.intensity = fadingValue;
            fadingValue += Time.deltaTime * fadingSpeed;    
            await UniTask.Yield();
        }

        color.a = 1f;
        sr.color = color;
        light2D.intensity = 1f;
    }

    public async UniTaskVoid ReleaseAfterFadeIn()
    {
        var fadingValue = light2D.intensity;
        const float fadingSpeed = 2f;
        var color = sr.color;

        while (fadingValue > 0f)
        {
            fadingValue -= Time.deltaTime * fadingSpeed;
            
            color.a = fadingValue;
            sr.color = color;
            light2D.intensity = fadingValue;

            await UniTask.Yield();
        }

        ManagedPool.Release(this);
    }

    #region 세팅
    
    public void SetMode(RigidbodyType2D type, bool isTrigger)
    {
        rb.bodyType = type;
    }
    
    /// <summary>
    /// rigidbody를 통해 position 설정, update랑 주기가 다르므로 주의
    /// </summary>
    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetColor(DanmakuColor color)
    {
        var colorIndex = (int)color;
        
        sr.sprite = danmakuSprites[colorIndex];
        light2D.color = _lightColors[colorIndex];
    }
    
    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetLightIntensity(float intensity)
    {
        light2D.intensity = intensity;
    }
    

    #endregion
    
    

    #region 원형 감옥
    
    /// <summary>
    /// 
    /// </summary>
    public async UniTaskVoid KeepDistanceFromPlayer(Vector2 distance, CancellationTokenSource cts)
    {
        rb.position = (Vector2)PlayerScript.Instance.GetFlyingPosition() + distance;
        while (true)
        {
            rb.linearVelocity = PlayerScript.Instance.velocity;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cts.Token);
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    public async UniTaskVoid CancelKeepDistanceFromPlayer()
    {
        var speed = 1f;
        while (speed > 0)
        {
            speed -= Time.fixedDeltaTime * 0.5f;
            rb.linearVelocity = PlayerScript.Instance.velocity * speed;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }

        rb.linearVelocity = Vector2.zero;
    }

    #endregion
}
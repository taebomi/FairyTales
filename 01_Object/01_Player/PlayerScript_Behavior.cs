using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public partial class PlayerScript
{
    // 무브
    [Header("Behavior")] public Vector2 velocity;
    public float currentMovementSpeed = 0f;
    public float MaximumMovementSpeed { get; private set; } = 3f;
    public float DeaccelerationSpeed { get; private set; } = 0.2f;
    public float AccelerationSpeed { get; private set; } = 0.25f;

    // 데쉬
    public float DashSpeed { get; private set; } = 12f;
    public const float DashDuration = 0.3f;
    public float DashCooldown { get; private set; } = 3f;
    [SerializeField] private AudioClip dashSE;

    [SerializeField] private Transform dashCooldownBarTransform;
    private TweenerCore<Vector3, Vector3, VectorOptions> _dashCooldownTweener;

    public bool IsDashCooldown { get; private set; }

    // 피격 관련
    public Vector2 reflectionDirection;
    public bool IsInvisible { get; private set; }
    private TweenerCore<Color, Color, ColorOptions> _invisibleTween;
    
    // 사망
    [field:SerializeField] public GameObject DeathEffectObject { get; private set; }

    private void InitializeBehavior()
    {
        _dashCooldownTweener = dashCooldownBarTransform.DOScaleX(0f, DashCooldown)
            .From(1f)
            .SetAutoKill(false);
        _invisibleTween = MainSpriteRenderer.DOFade(0.5f, 0.25f).From(1f).SetLoops(12).SetAutoKill(false);
    }


    public async UniTaskVoid CooldownForDash()
    {
        IsDashCooldown = true;
        dashCooldownBarTransform.gameObject.SetActive(true);
        _dashCooldownTweener.Restart();
        await _dashCooldownTweener.AsyncWaitForCompletion();
        dashCooldownBarTransform.gameObject.SetActive(false);
        IsDashCooldown = false;
    }

    public void SetReflectionDirection(Vector2 dir)
    {
        reflectionDirection = dir.normalized;
    }

    public async UniTaskVoid SetInvisibleState()
    {
        IsInvisible = true;
        _invisibleTween.Restart();
        await _invisibleTween.AsyncWaitForCompletion();
        IsInvisible = false;
    }
}
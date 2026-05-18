using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    Move,
    Dash,
    Damaged,
    Died,
    Event,
}

public abstract class PlayerStateBase
{
    protected static PlayerScript Player;
    public abstract PlayerState ThisEnum { get; }

    public UnityAction OnStateEnter;
    public UnityAction OnUpdate;
    public UnityAction OnFixedUpdate;
    public UnityAction OnStateExit;

    public UnityAction OnDashButtonDown;

    public UnityAction<Collider2D> OnTriggerEnter;

    public UnityAction<Vector2> OnDamaged;

    public static void Initialize(PlayerScript playerScript)
    {
        Player = playerScript;
    }
    
}

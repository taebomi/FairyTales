using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class PlayerState_Event : PlayerStateBase
{
    public override PlayerState ThisEnum => PlayerState.Event;

    public PlayerState_Event()
    {
        OnStateEnter = StateEnter;
        OnStateExit = StateExit;
    }

    private void StateEnter()
    {
        Player.MainRigidbody2D.linearVelocity = Player.velocity = Vector2.zero;
        Player.currentMovementSpeed = 0f;
    }

    private void StateExit()
    {
        Player.LastInputDir = new Vector2(Player.MainAnimator.GetFloat(AnimatorHash.MoveXDir),
            Player.MainAnimator.GetFloat(AnimatorHash.MoveYDir));
    }
}
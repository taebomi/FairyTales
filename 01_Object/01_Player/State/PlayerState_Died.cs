using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class PlayerState_Died : PlayerStateBase
{
    public override PlayerState ThisEnum => PlayerState.Died;


    public PlayerState_Died()
    {
        OnStateEnter = StateEnter;
    }

    private void StateEnter()
    {
        Player.DeathEffectObject.SetActive(true);
        Player.MainSpriteRenderer.gameObject.SetActive(false);

        Player.currentMovementSpeed = 0f;
        Player.LastInputDir = Vector2.zero;
        Player.MainRigidbody2D.linearVelocity = Player.velocity = Vector2.zero;
    }
}
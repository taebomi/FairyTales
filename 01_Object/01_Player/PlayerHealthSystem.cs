using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthSystem : HealthSystem
{
    [SerializeField] private PlayerScript playerScript;

    public readonly UnityEvent OnPlayerDamaged = new();

    public void ResetHp()
    {
        CurrentHp = MaxHp;
    }

    public override void Damage(int damage)
    {
        if (playerScript.IsInvisible || playerScript.CurrentState.ThisEnum is not PlayerState.Move)
        {
            return;
        }

        CurrentHp -= damage;
        
        if (IsLive)
        {
            playerScript.CurrentState.OnDamaged?.Invoke(Vector2.zero);
            OnPlayerDamaged.Invoke();
        }
        else
        {
            playerScript.ChangeState(PlayerState.Died);
            StageManager.Instance.ChangeStageState(StageState.GameOver);
        }
    }

    public override void Damage(int damage, Vector2 collisionPoint)
    {
        if (playerScript.IsInvisible || playerScript.CurrentState.ThisEnum is not PlayerState.Move)
        {
            return;
        }

        CurrentHp -= damage;
        
        if (IsLive)
        {
            var dir = (Vector2)playerScript.GetFlyingPosition() - collisionPoint;
            playerScript.CurrentState.OnDamaged?.Invoke(dir);
            OnPlayerDamaged.Invoke();
        }
        else
        {
            playerScript.ChangeState(PlayerState.Died);
            StageManager.Instance.ChangeStageState(StageState.GameOver);
        }
    }
}
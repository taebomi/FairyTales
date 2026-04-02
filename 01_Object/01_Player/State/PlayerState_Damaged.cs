using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class PlayerState_Damaged : PlayerStateBase
{
    public override PlayerState ThisEnum => PlayerState.Damaged;

    public PlayerState_Damaged()
    {
        OnStateEnter = UniTask.UnityAction(StateEnter);
        OnStateExit = StateExit;
    }

    private async UniTaskVoid StateEnter()
    {
        await Knockback();

        Player.ChangeState(Player.LastState.ThisEnum);
    }

    private static async UniTask Knockback()
    {
        if (Player.reflectionDirection != Vector2.zero)
        {
            await Player.MainRigidbody2D.DOMove(Player.reflectionDirection * 0.5f, 0.15f)
                .SetRelative(true)
                .Play();
        }

        await Player.transform.DOShakePosition(0.35f, new Vector3(0.25f, 0.25f, 0f), 30)
            .Play();
    }

    private static void StateExit()
    {
        Player.SetInvisibleState().Forget();
    }
}
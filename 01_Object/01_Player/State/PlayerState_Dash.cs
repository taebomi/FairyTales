using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class PlayerState_Dash : PlayerStateBase
{
    public override PlayerState ThisEnum => PlayerState.Dash;
    
    private Vector2 _dashDirection;
    private readonly Tweener _dashTweener;


    public PlayerState_Dash()
    {
        OnStateEnter = UniTask.UnityAction(StateEnter);
        OnFixedUpdate = FixedUpdate;

        _dashTweener = DOTween
            .To(x => Player.currentMovementSpeed = x,
                Player.DashSpeed, Player.MaximumMovementSpeed, PlayerScript.DashDuration)
            .SetEase(Ease.OutQuad)
            .SetAutoKill(false);
    }

    private async UniTaskVoid StateEnter()
    {
        _dashDirection = 
            Player.LastInputDir != Vector2.zero
            ? Player.LastInputDir.normalized
            : new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Player.SetMoveAnimation(_dashDirection);

        _dashTweener.Restart();

        CameraManager.Instance.Play_DashEffect().Forget();
        SoundManager.Instance.PlaySoundEffect(SoundName.Player_Dash).Forget();
        Player.CooldownForDash().Forget();

        while (!_dashTweener.IsComplete())
        {
            await UniTask.Delay(60);
            Player.MakeGhostEffect();
        }

        Player.ChangeState(Player.LastState.ThisEnum);
    }

    private void FixedUpdate()
    {
        
        Player.velocity = _dashDirection * Player.currentMovementSpeed;
        Player.MainRigidbody2D.linearVelocity = Player.velocity;
    }
}
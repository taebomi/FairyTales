using System;
using UnityEngine;

public partial class StageObject
{
    public virtual void SetAnimation(AnimationType animationType, float speed)
    {
        MainAnimator.speed = speed;
        switch (animationType)
        {
            case >= AnimationType.Move and < AnimationType.Act:
                SetMoveAnimation(animationType);
                break;
            case >= AnimationType.Act and < AnimationType.Face: // 행동
                SetActAnimation(animationType);
                break;
            case >= AnimationType.Face:
                SetFaceAnimation(animationType);
                break;
        }
    }

    public virtual void SetIdleAnimation(AnimationType animationType)
    {
        MainAnimator.speed = 1f;

        switch (animationType)
        {
            case >= AnimationType.Move and < AnimationType.Act:
                SetMoveAnimation(AnimationType.Move_IdleCenter);
                break;
            case >= AnimationType.Act and < AnimationType.Face: // 행동
                SetActAnimation(AnimationType.Act_Idle);
                break;
            case >= AnimationType.Face: // 표정
                SetFaceAnimation(AnimationType.Face_Idle);
                break;
        }
    }

    private void SetMoveAnimation(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.Move_IdleCenter:
                SetMoveAnimation(Vector2.zero, true);
                break;
            case AnimationType.Move_IdleLeft:
                SetMoveAnimation(Vector2.left, true);
                break;
            case AnimationType.Move_WalkLeft:
                SetMoveAnimation(Vector2.left, false);
                break;
            case AnimationType.Move_IdleRight:
                SetMoveAnimation(Vector2.right, true);
                break;
            case AnimationType.Move_WalkRight:
                SetMoveAnimation(Vector2.right, false);
                break;
            case AnimationType.Move_IdleUp:
                SetMoveAnimation(Vector2.up, true);
                break;
            case AnimationType.Move_WalkUp:
                SetMoveAnimation(Vector2.up, false);
                break;
            case AnimationType.Move_IdleDown:
                SetMoveAnimation(Vector2.down, true);
                break;
            case AnimationType.Move_WalkDown:
                SetMoveAnimation(Vector2.down, false);
                break;
        }
    }

    public void SetMoveAnimation(Vector2 dir)
    {
        MainAnimator.SetFloat(AnimatorHash.MoveXDir, dir.x);
        MainAnimator.SetFloat(AnimatorHash.MoveYDir, dir.y);
    }

    protected void SetMoveAnimation(Vector2 dir, bool isIdle)
    {
        SetMoveAnimation(dir);
        MainAnimator.SetFloat(AnimatorHash.MoveSqrMagnitude, isIdle ? 0f : 1f);
    }

    public void SetActAnimation(AnimationType animationType)
    {
        MainAnimator.SetInteger(AnimatorHash.Act, animationType - AnimationType.Act);
    }

    protected void SetFaceAnimation(AnimationType animationType)
    {
        MainAnimator.SetInteger(AnimatorHash.Face, animationType - AnimationType.Face);
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class AnimatorHash
{
    public static readonly int MoveXDir = Animator.StringToHash(nameof(MoveXDir));
    public static readonly int MoveYDir = Animator.StringToHash(nameof(MoveYDir));
    public static readonly int MoveSqrMagnitude = Animator.StringToHash(nameof(MoveSqrMagnitude));
    
    public static readonly int OptionBool = Animator.StringToHash(nameof(OptionBool));
    public static readonly int OptionTrigger = Animator.StringToHash(nameof(OptionTrigger));

    public static readonly int Type = Animator.StringToHash(nameof(Type));

    public static readonly int Act = Animator.StringToHash(nameof(Act));
    public static readonly int Option = Animator.StringToHash(nameof(Option));
    public static readonly int Face = Animator.StringToHash(nameof(Face));

    // 트리거
    public static readonly int AttackAgain = Animator.StringToHash(nameof(AttackAgain));
    public static readonly int ActAgain = Animator.StringToHash(nameof(ActAgain));

    public static readonly int AnimationSpeed = Animator.StringToHash(nameof(AnimationSpeed));

    public static readonly int Prepare = Animator.StringToHash(nameof(Prepare));
    public static readonly int Attack = Animator.StringToHash(nameof(Attack));
    public static readonly int Wait = Animator.StringToHash(nameof(Wait));

    public static readonly int Close = Animator.StringToHash(nameof(Close));
}


[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum AnimationType
{
    #region 이동
    Move = 0_00_00_00,
    Move_IdleCenter = 0_00_00_00,
    Move_IdleLeft = 0_00_00_01,
    Move_IdleRight = 0_00_00_02,
    Move_IdleUp = 0_00_00_03,
    Move_IdleDown = 0_00_00_04,
    Move_WalkLeft = 0_00_00_10,
    Move_WalkRight = 0_00_00_20,
    Move_WalkUp = 0_00_00_30,
    Move_WalkDown = 0_00_00_40,

    #endregion

    #region 행동
    Act = 1_00_00_00,
    Act_Idle = 1_00_00_00,

    // 일반
    Act_Jump = 1_00_00_10,
    Act_Hurray = 1_00_00_20,
    Act_Fingering = 1_00_00_30,
    Act_LookAroundMedium = 1_00_00_40,

    // 물체
    Act_Close = 1_10_00_00,
    Act_Open = 1_10_00_01,
    
    Act_Activate = 1_11_00_00,
    Act_Activated = 1_11_00_01,
    Act_Deactivate = 1_11_00_10,
    Act_Deactivated = 1_11_00_11,
    
    // 사망
    Act_Death = 1_99_99_99,

    #endregion

    #region 얼굴 / 감정
    Face = 2_00_00_00,
    // 무감정 / 기타
    Face_Idle = 2_00_00_00,

    Face_ClosedEyes = 2_00_01_00,
    Face_HalfClosedEyes = 2_00_01_01,

    // 기쁨
    Face_Laugh = 2_01_00_00,
    Face_Scoff = 2_01_00_10,

    // 분노
    Face_Angry = 2_02_00_00,
    // 슬픔

    // 아픔
    Face_Damaged = 2_04_00_00,

    // 불안

    // 창피

    // 사랑

    // 바람

    #endregion
}
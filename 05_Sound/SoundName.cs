// ReSharper disable InconsistentNaming
public enum SoundName
{
    #region 배경음악

    #region Stage01

    Forest_Normal = 0_100_000,
    DeepForest_Normal = 0_100_010,
    Alraune_Phase1 = 0_100_100,

    #endregion

    #region Etc

    Happy_Situation = 0_900_000,

    #endregion

    #endregion

    EndBGM_StartSE = 999_999,

    #region 효과음

    #region 감정 표현

    Emotion = 1_000_000,
    Emotion_Dialogue = EmotionBubble.EmotionType.Dialogue + Emotion,
    Emotion_Exclamation,
    Emotion_Question,
    Emotion_Heart,
    Emotion_Sad,
    Emotion_Angry,
    Emotion_Fun,
    Emotion_Droplets,
    Emotion_Save = EmotionBubble.EmotionType.Save + Emotion,

    #endregion

    #region 사물

    #region 세이브포인트

    SavePoint_Activation = 2_000_000,

    #endregion
    

    #endregion
    #region 플레이어

    Player_Dash = 9_000_000,

    #endregion

    #region 알라우네

    Alraune_Danmaku_Make = 9_100_000,
    Alraune_Danmaku_Move = 9_100_001,
    Alraune_Laser_Charge = 9_100_002,
    Alraune_Laser_Shot = 9_100_003,
    Alraune_Thorn_Make = 9_100_004,

    #endregion
    

    #region 전투

    #region 충돌

    Crash_Iron = 15_000_000,

    #endregion

    #region 찌르기

    Stab01 = 15_000_010,
    Stab02 = 15_000_011,
    Stab03 = 15_000_012,

    #endregion


    #endregion

    #endregion
}
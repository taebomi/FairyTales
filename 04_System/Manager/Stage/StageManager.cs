using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public enum StageState
{
    None,
    FirstPlaying,
    Playing,
    StaticEvent,
    GameOver,
    Reset,
}

public partial class StageManager : Singleton<StageManager>
{
    [field: SerializeField] public int StageNum { get; private set; }


    #region 스테이지 상태 및 이벤트

    public StageState StageState { get; private set; } = StageState.None;
    public static readonly UnityEvent<StageState> StageStateChanged = new();

    public void ChangeStageState(StageState state)
    {
        StageState = state;
        StageStateChanged.Invoke(StageState);
        switch (StageState)
        {
            case StageState.None:
                break;
            case StageState.FirstPlaying:
                break;
            case StageState.Playing:
                break;
            case StageState.StaticEvent:
                break;
            case StageState.GameOver:
                GameOver().Forget();
                break;
            case StageState.Reset:

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    public Light2D globalLight;

    [SerializeField] private bool isDebugging;


    protected override void AwakeAfter()
    {
        InitializeCommonObject();

        Room.RoomChangeStartedEvent.AddListener(OnRoomChangeStarted);
    }

    // 스테이지 시작
    public void Start()
    {
        var playerPos = isDebugging
            ? PlayerScript.Instance.transform.position
            : (Vector3)SaveManager.Instance.PlayerSaveData.playerPosition;
        PlayerScript.Instance.transform.position = playerPos;
        ChangeStageState(StageState.FirstPlaying);
        var currentRoom = PlayerScript.Instance.GetCurrentRoom();
        Room.ChangeRoom(currentRoom, true).Forget();
        ChangeStageState(StageState.Playing);
    }


    #region 방 변경

    private void OnRoomChangeStarted(bool instantly)
    {
        if (instantly)
        {
            globalLight.intensity = Room.CurrentRoomData.lightIntensity;
        }
        else
        {
            DOTween.To(x => globalLight.intensity = x, globalLight.intensity,
                    Room.CurrentRoomData.lightIntensity, Room.RoomChangingDuration)
                .SetUpdate(true)
                .Play();
        }
    }

    #endregion
}
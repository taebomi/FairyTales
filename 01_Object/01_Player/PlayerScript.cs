using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FairyTales.Layer;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public partial class PlayerScript : StageObject
{
    #region 싱글톤 용도

    private static PlayerScript _instance;

    public static PlayerScript Instance
    {
        get
        {
            if (_instance)
            {
                return _instance;
            }

            Debug.Log("플레이어 스크립트 찾아서 싱글톤 등록");
            _instance = GameObject.FindWithTag("Player").GetComponent<PlayerScript>();
            return _instance;
        }
        private set => _instance = value;
    }

    #endregion


    [field: SerializeField, Header("Main")]
    public SpriteRenderer MainSpriteRenderer { get; private set; }

    [field: SerializeField] public Rigidbody2D MainRigidbody2D { get; private set; }


    [field: SerializeField] public PlayerHealthSystem HealthSystem { get; private set; }
    [field: SerializeField] public EmotionableObject EmotionSystem { get; private set; }

    public static readonly Vector3 FlyingPosition = new(0f, 0.3f, 0f);


    private void Awake()
    {
        Instance = this;

        InitializeInput();
        InitializeStateMachine();
        InitializeBehavior();
        InitializeSkill();
        InitializeInteraction();

        StageManager.StageStateChanged.AddListener(ApplyStageState);
        Room.RoomChangeStartedEvent.AddListener(OnRoomChangeStarted);
    }

    private void ApplyStageState(StageState state)
    {
        switch (state)
        {
            case StageState.None:
            case StageState.FirstPlaying:
                break;
            case StageState.Playing:
                ChangeState(PlayerState.Move);
                break;
            case StageState.StaticEvent:
                ChangeState(PlayerState.Event);
                break;
            case StageState.GameOver:

                break;
            case StageState.Reset:
                MainSpriteRenderer.gameObject.SetActive(true);
                DeathEffectObject.SetActive(false);
                HealthSystem.ResetHp();
                SetMoveAnimation(Vector2.zero);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    /// <summary>
    /// 이벤트 시작 시 정해진 위치로 이동하도록 명령 및 마지막에 설정한 방향 바라보도록 하기.
    /// </summary>
    /// <param name="destPos">목적지 좌표</param>
    /// <param name="animationType">마지막에 바라볼 방향 설정하는 용도</param>
    public async UniTask MoveToPosition(Vector2 destPos, AnimationType animationType)
    {
        var direction = destPos - (Vector2)transform.position; // 이동해야 할 방향
        SetMoveAnimation(direction.normalized, false);
        await transform.DOMove(destPos, 0.5f).Play();
        SetAnimation(animationType, 1f);
    }

    public Vector3 GetFlyingPosition()
    {
        return transform.position + FlyingPosition;
    }


    public Room GetCurrentRoom()
    {
        var result = Physics2D.CircleCast(transform.position, 0.1f, transform.forward,
            1f, LayerCache.GetLayerMask(LayerName.Room));

        return result.transform.GetComponent<Room>();
    }


    [SerializeField] private Light2D fairyLight;

    private void OnRoomChangeStarted(bool instantly)
    {
        if (Room.CurrentRoomData.lightIntensity <= 0.5f)
        {
            if (instantly)
            {
                fairyLight.gameObject.SetActive(true);
                fairyLight.intensity = 1f;
            }
            else
            {
                fairyLight.gameObject.SetActive(true);
                DOTween.To(x => fairyLight.intensity = x, fairyLight.intensity,
                        1f, Room.RoomChangingDuration)
                    .SetUpdate(true)
                    .Play();
            }
        }
        else
        {
            if (instantly)
            {
                fairyLight.intensity = 0f;
                fairyLight.gameObject.SetActive(false);
            }
            else
            {
                DOTween.To(x => fairyLight.intensity = x, fairyLight.intensity,
                        0f, Room.RoomChangingDuration)
                    .SetUpdate(true)
                    .Play()
                    .OnComplete(()=>fairyLight.gameObject.SetActive(false));
            }
        }
    }
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public partial class CameraManager : Singleton<CameraManager>
{
    // 카메라 
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Camera OverlayCamera { get; private set; }
    [SerializeField] private Transform playerTransform;


    #region 카메라 크기 및 위치 제한 관련

    public const float DefaultCameraSize = 3f; // 기본 카메라 사이즈
    private const float CameraZPosition = -10f; // 기본 카메라 z 위치

    // 현재 카메라의 높이 / 너비 저장용 변수 !!! 카메라 사이즈 컨트롤은 이 변수로 할 것 !!!
    private float _currentHalfHeight, _currentHalfWidth;

    private RoomData _currentRoomData;
    private Vector2 _roomCenterPosition, _roomMinPosition, _roomMaxPosition; // 카메라 범위
    private Vector2 _cameraMinPosition, _cameraMaxPosition;

    #endregion


    private Vector2 _desiredPosition;
    [SerializeField] private float chaseSpeed = 3f;

    public enum CameraMode
    {
        Manual,
        ChasePlayer,
        Event,
        GameOver,
    }

    public CameraMode cameraMode;

    private CancellationTokenSource _disableCts = new();

    protected override void AwakeAfter()
    {
        InitializePostProcessing(true);
        
        ActivateOverlayCamera(false);

        // 카메라 너비/높이 초기화
        _currentNormalSize = _currentHalfHeight = DefaultCameraSize;
        UpdateCameraSize();

        Room.RoomChangeStartedEvent.AddListener(instantly => OnRoomChangeStarted(instantly).Forget());
        StageManager.StageStateChanged.AddListener(OnStageStateChanged);
    }

    private void OnStageStateChanged(StageState state)
    {
        switch (state)
        {
            case StageState.None:
                break;
            case StageState.FirstPlaying:
                _desiredPosition = playerTransform.position;
                break;
            case StageState.Playing:
                ChangeCameraMode(CameraMode.ChasePlayer);
                break;
            case StageState.StaticEvent:
                ChangeCameraMode(CameraMode.Event);
                break;
            case StageState.GameOver:
                ActivateOverlayCamera(true);
                SyncOverlayCameraToMainCamera();
                ChangeCameraMode(CameraMode.GameOver);
                break;
            case StageState.Reset:
                _disableCts.Cancel();
                _disableCts = new CancellationTokenSource();
                _desiredPosition = playerTransform.position;
                InitializePostProcessing(false);
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void UpdateCameraSize()
    {
        MainCamera.orthographicSize = _currentHalfHeight;
        _currentHalfWidth = _currentHalfHeight * MainCamera.aspect;
    }

    private void Update()
    {
        UpdateCameraSize();
        UpdateCameraArea();
    }

    private void LateUpdate()
    {
        switch (cameraMode)
        {
            case CameraMode.Manual:
                return;
            case CameraMode.ChasePlayer:
                _desiredPosition = playerTransform.position;
                break;
            case CameraMode.Event:
                break;
            case CameraMode.GameOver:
                OverlayCamera.transform.position = MainCamera.transform.position;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var mainCameraPosition = MainCamera.transform.localPosition;

        mainCameraPosition = Vector2.Lerp(mainCameraPosition, _desiredPosition,
            Time.deltaTime * chaseSpeed);

        // 카메라 위치를 범위 내로 Clamp 및 z포지션 설정
        var destPos = GetClampedCameraPosition(mainCameraPosition);
        // 카메라 위치 목적지로 이동
        MainCamera.transform.localPosition = destPos;
    }


    public static readonly UnityEvent CameraMoveFinishedEvent = new();

    private async UniTaskVoid OnRoomChangeStarted(bool instantly)
    {
        var roomData = Room.CurrentRoomData;
        UpdateRoomArea(roomData.position, roomData.halfWidth, roomData.halfHeight);
        UpdateCameraArea();
        var destPosition = GetClampedCameraPosition(playerTransform.position);

        if (!instantly)
        {
            cameraMode = CameraMode.Manual;
            Time.timeScale = 0f;

            await MainCamera.transform
                .DOMove(destPosition, Room.RoomChangingDuration)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true)
                .Play();

            cameraMode = CameraMode.ChasePlayer;
            Time.timeScale = 1f;
        }
        else
        {
            MainCamera.transform.position = destPosition;
        }

        CameraMoveFinishedEvent.Invoke();
    }

    private void UpdateRoomArea(Vector2 center, float halfWidth, float halfHeight)
    {
        _roomCenterPosition = center;
        _roomMinPosition = new Vector2(center.x - halfWidth, center.y - halfHeight);
        _roomMaxPosition = new Vector2(center.x + halfWidth, center.y + halfHeight);
    }

    private void UpdateCameraArea()
    {
        var cameraHalfSize = new Vector2(_currentHalfWidth, _currentHalfHeight);
        _cameraMinPosition = _roomMinPosition + cameraHalfSize;
        _cameraMaxPosition = _roomMaxPosition - cameraHalfSize;
    }

    private Vector3 GetClampedCameraPosition(Vector3 desiredPosition)
    {
        Vector3 clampedPosition;
        clampedPosition.x = _cameraMinPosition.x > _cameraMaxPosition.x
            ? _roomCenterPosition.x
            : Mathf.Clamp(desiredPosition.x, _cameraMinPosition.x, _cameraMaxPosition.x);
        clampedPosition.y = _cameraMinPosition.y > _cameraMaxPosition.y
            ? _roomCenterPosition.y
            : Mathf.Clamp(desiredPosition.y, _cameraMinPosition.y, _cameraMaxPosition.y);
        clampedPosition.z = CameraZPosition;

        return clampedPosition;
    }

    public void ChangeCameraMode(CameraMode mode)
    {
        cameraMode = mode;
    }

    public async UniTask MoveTo(Vector3 desiredPos, float time)
    {
        var destPos = GetClampedCameraPosition(desiredPos);
        await MainCamera.transform.DOMove(destPos, time).SetEase(Ease.InOutSine).Play();
    }
}
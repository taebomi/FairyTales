using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyTales.Room;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class RoomData
{
    public Vector2 position;
    public float halfWidth;
    public float halfHeight;
    public float lightIntensity;
    public SoundName bgm;

    public RoomData(RoomData roomData)
    {
        position = roomData.position;
        halfWidth = roomData.halfWidth;
        halfHeight = roomData.halfHeight;
        lightIntensity = roomData.lightIntensity;
        bgm = roomData.bgm;
    }
} // 방의 정보

public class Room : MonoBehaviour
{
    [field: SerializeField] public RoomData ThisRoomData { get; private set; } // 이 방의 정보

    public static RoomData CurrentRoomData; // 현재 방의 정보, 오브젝트의 이벤트 상태 등으로부터 방의 정보 변동있을 경우 덮어씌우는 용도

    private static Room _previousRoom;
    private static Room _currentRoom;
    private static Room _nextRoom; // 다른 방 들어갔는지 체크 용도

    public const float RoomChangingDuration = 1f;

    public static readonly UnityEvent<bool> RoomChangeStartedEvent = new();
    public static readonly UnityEvent RoomChangeCompletedEvent = new();

    private void OnTriggerEnter2D(Collider2D col)
    {
        _nextRoom = this;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_nextRoom == this)
        {
            return;
        }

        ChangeRoom(_nextRoom).Forget();
    }

    public static async UniTaskVoid ChangeRoom(Room nextRoom, bool instantly = false)
    {
        _previousRoom = _currentRoom;
        _currentRoom = nextRoom;
        CurrentRoomData = new RoomData(_currentRoom.ThisRoomData);

        if (instantly)
        {
            if (_previousRoom)
            {
                _previousRoom.DeactivateObjects();
            }

            RoomChangeStartedEvent.Invoke(true);
            _currentRoom.ActivateObjects();
            RoomChangeCompletedEvent.Invoke();
        }
        else
        {
            _currentRoom.ActivateObjects();
            RoomChangeStartedEvent.Invoke(false);
            await CameraManager.CameraMoveFinishedEvent.OnInvokeAsync(CancellationToken.None);
            _previousRoom.DeactivateObjects();
            RoomChangeCompletedEvent.Invoke();
        }
    }


    /// <summary>
    /// 방에 있는 오브젝트 활성화
    /// </summary>
    private void ActivateObjects()
    {
        foreach (Transform childTransform in transform)
        {
            if (!childTransform.TryGetComponent(out EventObject eventObject))
            {
                childTransform.gameObject.SetActive(true);
                continue;
            }

            eventObject.Initiate();
        }
    }

    /// <summary>
    /// 방에 있는 오브젝트 비활성화.
    /// 현재 활성화 된 오브젝트들 비활성화
    /// </summary>
    private void DeactivateObjects()
    {
        foreach (Transform childTransform in transform)
        {
            var child = childTransform.gameObject;
            if (child.activeSelf)
            {
                child.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 씬 뷰에 방 범위 그려주는 용도
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(ThisRoomData.position,
            new Vector2(ThisRoomData.halfWidth * 2, ThisRoomData.halfHeight * 2));
    }
#endif
}
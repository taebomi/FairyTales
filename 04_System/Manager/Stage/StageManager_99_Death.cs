using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ES3Internal;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public partial class StageManager
{
    // 1. 플레이어 사망 및 애니메이션 재생
    // 플레이어 사망 오브젝트 ( 일반 + 오버레이 사망 애니메이션 (2개), 이펙트 
    // 2. 잠시 후 화면 서서히 암전
    // 3. 화면 완전히 암전 (나비 날개만 남아있음)
    // ============== 완료 ===================
    // 4. 현재 활성화 되어있는 것들 비활성화 및 초기화
    /// 비활성화 -> 
    // 활성화 -
    // 
    // ---- 재시작 버튼?
    // 5. 마지막 세이브 위치 로드 및 플레이어 옮기기
    // 6. 플레이어가 해당하는 방 오브젝트 활성화
    // 7. 화면 서서히 밝아짐.
    public async UniTaskVoid GameOver()
    {
        await UniTask.Delay(500);
        await UIManager.Instance.ActivateGameOverScreen();
        
        UIManager.Instance.SetOverlayFadeColor(Color.black);
        await UIManager.Instance.FadeOverlay(1f, 2f);
        UIManager.Instance.DeactivateGameOverScree();

        // 플레이어 위치 이동시키기 및 카메라 위치 설정
        // 방 활성화 시키기
        // 화면 밝게 하기
        // 아직 데스 상태로 되어있으면 플레이 상태로 변경하기
        var statueName = SaveManager.Instance.PlayerSaveData.savePointName;

        var playerPos = statueName == null ? Vector3.zero : (Vector3)saveStatueDict[statueName].PlayerRespawnPosition;
        PlayerScript.Instance.transform.position = playerPos;
        
        ChangeStageState(StageState.Reset);
        var currentRoom = PlayerScript.Instance.GetCurrentRoom();
        Room.ChangeRoom(currentRoom, true).Forget();
        
        await UIManager.Instance.FadeOverlay(0f, 2f);
        if (StageState == StageState.Reset)
        {
            Debug.Log("아직 유지중");
            ChangeStageState(StageState.Playing);
        }

    }


    #region 세이브 석상

    [System.Serializable]
    public class SaveStatueDict : SerializableDictionaryBase<string, SavePoint>
    {
    }

    [SerializeField] private SaveStatueDict saveStatueDict;
    
    

    #endregion
}
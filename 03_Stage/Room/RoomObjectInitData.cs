using System;
using UnityEngine;



namespace FairyTales.Room
{
    [CreateAssetMenu(menuName = "Fairy Tales/Room Object Activation Data", order = 0)]
    public class RoomObjectInitData : ScriptableObject
    {
        public ObjectActivateData[] dataArr;
        
        [Serializable]
        public class ObjectActivateData
        {
            public string eventName;
            public bool willActivate;
            public Vector3 position;
        }
    }
    // 방 접근 시 위 데이터 읽어서 오브젝트 활성화 / 비활성화 및 위치 결정.
    // 데이터는 이벤트 이름을 체크해서 해당 이벤트 클리어 되었는지를 확인 후 클리어 시 해당 데이터를 사용.
    // 일치하는 데이터가 없을 시에는 씬에 배치되있는 기본 값 사용.
    
    // todo: 위치 및 활성화 / 비활성화 외에 다른 요소가 필요한 경우, 공통적으로 많이 사용되면 추가하기
    // 위 todo에 만족하지 못하는 경우에는 오브젝트 복제해서 배치하고 다른 요소 넣어준 뒤 이벤트 여부로 하나만 활성화되도록 하기
    
    


}

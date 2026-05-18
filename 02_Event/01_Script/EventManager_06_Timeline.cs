using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public partial class EventManager
    {
        public void JumpTimeline(double time)
        {
            playableDirector.time = time; 
        }
        
        
        // 버튼 선택까지 대기 후 누른 버튼 번호에 맞는 위치에서 재생
        public async UniTask BranchTimelineByChoice(double[] destTimes)
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            var clickedButtonIndex= await ChoiceBtnClickEvent.OnInvokeAsync(_eventCts.Token);
            playableDirector.time = destTimes[clickedButtonIndex];
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
        }
    }
}
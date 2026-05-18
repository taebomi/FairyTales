using System;
using System.Collections.Generic;
using System.Threading;
using FairyTales.EventSystem;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Playables;


namespace FairyTales.EventSystem
{
    public partial class EventManager : Singleton<EventManager>
    {
        [SerializeField] private PlayableDirector playableDirector;

        #region 이벤트 상태 및 모드

        private enum EventMode
        {
            None, // 유휴 및 기타 동작 상태
            Dialogue, // 대사 출력 중
            Choice, // 선택지 출력 중
        }

        private static EventMode _eventMode;

        #endregion

        private static StageEventData _stageEventData;

        private CancellationTokenSource _eventCts;

        protected override void AwakeAfter()
        {
            InitializeSystem();
            InitializeInput();
            InitializeLetterbox();
            InitializeDialogueSystem();
            InitializeChoiceSystem();
        }

        private void InitializeSystem()
        {
            _stageEventData = GetStageEventData();
            playableDirector.stopped += playableDirectorOnStopped;
        }

        private void OnDisable()
        {
            playableDirector.stopped -= playableDirectorOnStopped;
        }

        private void playableDirectorOnStopped(PlayableDirector _)
        {
            FinishEvent().Forget();
        }

        public static StageEventData GetStageEventData()
        {
            var text = Resources.Load<TextAsset>("EventData/stage" + StageManager.Instance.StageNum.ToString("00"));
            return JsonConvert.DeserializeObject<StageEventData>(text.text);
        }


        // public void InitiateEvent(StageEvent stageEvent)
        // {        
        //     if (_eventState != EventState.Idle)
        //         return;
        //     
        //     _currentEvent = stageEvent;
        //     ToggleLetterbox(true).Forget();
        //     StageManager.Instance.ChangeStageState(StageState.StaticEvent);
        //
        //     var eventDataJson = _currentStageEventDataJsonDict[_currentEvent.EventName];
        //     _currentEventData = EventDataConverter.ConvertFromJson(eventDataJson);
        // }
        //
        // private void ChangeEventState(EventState eventState)
        // {
        //     _eventState = eventState;
        //     switch (eventState)
        //     {
        //         case EventState.Idle:
        //             _currentEvent.FinishEvent();
        //             StageManager.Instance.ChangeStageState(StageState.Playing);
        //             break;
        //         case EventState.Setting:
        //             break;
        //         case EventState.Playing:
        //             StartEvent();
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(eventState), eventState, null);
        //     }
        // }

        private void ChangeEventMode(EventMode eventMode)
        {
            _eventMode = eventMode;
        }

        // private void StartEvent()
        // {
        //     Debug.Log("◆ Event Manager - 이벤트 시작");
        //     SetEventInputAction(true);
        //     playableDirector.Play(_currentEvent.EventTimelineAsset);
        // }
        //
        // private void FinishEvent(PlayableDirector director)
        // {
        //     Debug.Log("◆ Event Manager - 이벤트 종료");
        //     SetEventInputAction(false);
        //     ToggleLetterbox(false).Forget();
        // }
    }
}
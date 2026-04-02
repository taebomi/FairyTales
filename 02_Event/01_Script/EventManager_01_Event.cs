using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyTales.EventSystem;
using UnityEngine;
using UnityEngine.Events;

namespace FairyTales.EventSystem
{
    public partial class EventManager
    {
        private static EventData _currentEventData;
        private static EventInfo _currentEventInfo;

        public static readonly UnityEvent<string> EventFinishedEvent = new();

        public static bool IsPlaying;


        public async UniTask BeginEvent(EventInfo eventInfo)
        {
            if (_currentEventInfo is not null) return;

            Debug.Log("◆ Event Manager - 이벤트 시작");
            IsPlaying = true;

            _currentEventInfo = eventInfo;
            _currentEventData = _stageEventData[_currentEventInfo.name];

            if (_currentEventInfo.StartPositionType == StartPositionType.Fixed)
            {
                PlayerScript.Instance
                    .MoveToPosition(_currentEventInfo.StartPosition, _currentEventInfo.StartAnimationType).Forget();
            }
            else if (_currentEventInfo.StartPositionType == StartPositionType.Instant)
            {
                PlayerScript.Instance.transform.position = _currentEventInfo.StartPosition;
                PlayerScript.Instance.SetAnimation(_currentEventInfo.StartAnimationType, 1f);
            }

            await CheckEventPlayType(true);

            _eventCts = new CancellationTokenSource();

            playableDirector.Play(_currentEventInfo.EventPlayableAsset);
        }


        private async UniTaskVoid FinishEvent()
        {
            await CheckEventPlayType(false);

            EventFinishedEvent.Invoke(_currentEventInfo.name);

            IsPlaying = false;
            _currentEventInfo = null;

            Debug.Log("◆ Event Manager - 이벤트 종료");
        }


        private async UniTask CheckEventPlayType(bool isBeginning)
        {
            if (isBeginning)
            {
                switch (_currentEventInfo.EventPlayType)
                {
                    case EventPlayType.Static:
                        dialogueBoxRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                            StaticDialogueHeight);
                        StageManager.Instance.ChangeStageState(StageState.StaticEvent);
                        await ToggleLetterbox(true);
                        break;
                    case EventPlayType.Dynamic:
                        dialogueBoxRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                            DynamicDialogueHeight);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                switch (_currentEventInfo.EventPlayType)
                {
                    case EventPlayType.Static:
                        ToggleLetterbox(false).Forget();
                        StageManager.Instance.ChangeStageState(StageState.Playing);
                        break;
                    case EventPlayType.Dynamic:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
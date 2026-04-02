using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyTales.EventSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace FairyTales.EventSystem
{
    public class EventExecutor : MonoBehaviour
    {
        [SerializeField] private EventInfo[] events;

        private void OnTriggerEnter2D(Collider2D col)
        {
            StartEvent(EventTriggerCondition.TriggerEnter).Forget();
        }

        public void OnInteract()
        {
            StartEvent(EventTriggerCondition.Interact).Forget();
        }

        public void StartEvent(string eventName)
        {
            StartEvent(EventTriggerCondition.Manual, eventName).Forget();
        }

        private async UniTaskVoid StartEvent(EventTriggerCondition eventTriggerCondition, string eventName = null)
        {
            if (EventManager.IsPlaying)
            {
                Debug.LogWarning("이벤트 이미 실행 중 !");
                return;
            }
            
            var currentPlayingEvent = GetAppropriateEvent(eventTriggerCondition, eventName);

            if (currentPlayingEvent == null)
            {
                return;
            }

            currentPlayingEvent.Begin();


            await EventManager.EventFinishedEvent.OnInvokeAsync(CancellationToken.None);

            currentPlayingEvent.End();
        }

        /// <summary>
        /// 조건에 맞는 이벤트 디테일을 배열 뒤에서부터 찾아서 반환
        /// </summary>
        private EventInfo GetAppropriateEvent(EventTriggerCondition triggeredType, string eventName = null)
        {
            var saveManager = SaveManager.Instance;

            for (var i = events.Length - 1; i >= 0; i--)
            {
                var eventDetail = events[i];

                // 이벤트 트리거 조건이 동일한지?
                if (eventDetail.EventTriggerCondition != triggeredType)
                {
                    continue;
                }

                // 이벤트 이름 지정되었을 경우 이름 동일한지?
                if (eventName != null && eventDetail.name != eventName)
                {
                    continue;
                }

                // 클리어 해야하는 이벤트 목록이 모두 클리어 되었는지? 
                if (!saveManager.CheckAllEventCleared(eventDetail.HaveToClearEventNames))
                {
                    continue;
                }

                // 클리어 하면 안되는 이벤트 목록이 모두 not 클리어 되었는지?
                if (saveManager.CheckAnyEventCleared(eventDetail.HaveToUnclearEventNames))
                {
                    continue;
                }

                return eventDetail;
            }

            Debug.LogWarning("실행할 이벤트 찾지 못함.");
            return null;
        }
    }
}
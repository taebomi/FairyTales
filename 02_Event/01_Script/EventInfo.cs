using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    public enum StartPositionType
    {
        Free,
        Fixed,
        Instant,
    }
    public enum EventTriggerCondition
    {
        Manual,
        Interact,
        TriggerEnter,
    }
    public enum EventPlayType
    {
        Static,
        Dynamic,
    }

    [CreateAssetMenu(menuName = "Fairy Tales/Event Info")]
    public class EventInfo : ScriptableObject
    {
        [field: SerializeField] public TimelineAsset EventPlayableAsset { get; private set; }

        [field: SerializeField] public EventPlayType EventPlayType { get; private set; }
        [field: SerializeField] public EventTriggerCondition EventTriggerCondition { get; private set; }


        [field: Header("Player Option")]
        [field: SerializeField]
        public StartPositionType StartPositionType { get; private set; }
        [field: SerializeField] public Vector3 StartPosition { get; private set; }
        [field: SerializeField] public AnimationType StartAnimationType { get; private set; }


        [field: SerializeField, Header("Condition of Event Progress")]
        public string[] HaveToClearEventNames { get; private set; }

        [field: SerializeField] public string[] HaveToUnclearEventNames { get; private set; }

        public void Begin()
        {
            EventManager.Instance.BeginEvent(this).Forget();
        }


        public void End()
        {
            SaveManager.Instance.SetEventCleared(name);
        }
    }
}
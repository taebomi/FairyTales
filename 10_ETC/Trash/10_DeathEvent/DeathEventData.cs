using System;
using UnityEngine;
using UnityEngine.Timeline;


namespace FairyTales.Trash
{
    // [CreateAssetMenu(menuName = "Fairy Tales/DeathEventData", fileName = "Death Event Data")]
    public class DeathEventData : ScriptableObject
    {
        public string eventName;
        public TimelineAsset timelineAsset;
        public GameObject eventObjectsPrefab;

        public StartData playerStartData;
        public StartData enemyStartData;
    }

    [Serializable]
    public class StartData
    {
        public Vector3 startPosition;
        public AnimationType startAnimationType;
    }
}


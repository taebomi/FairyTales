using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class ChoiceBehaviour : PlayableBehaviour
    {
        public int id;
        public EventManager.ChoiceInfo[] choiceInfo;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            
            EventManager.Instance.PrintChoice(id, choiceInfo).Forget();
        }

    }
}
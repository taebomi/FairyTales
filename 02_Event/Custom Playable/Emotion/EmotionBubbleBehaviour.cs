using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class EmotionBubbleBehaviour : PlayableBehaviour
    {
        public EmotionBubble.EmotionType emotionType;
        private EmotionableObject _bindObject;

        private bool _isClipPlayed;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (_isClipPlayed) return;
            _isClipPlayed = true;

            _bindObject = (EmotionableObject)playerData;
            _bindObject.CreateEmotionBubble(emotionType);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (!_isClipPlayed) return;
            _isClipPlayed = false;

            _bindObject.RemoveEmotionBubble();
        }
    }
}
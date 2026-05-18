using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class AnimationBehaviour : PlayableBehaviour
    {
        [Serializable]
        public struct AnimationInfo
        {
            public AnimationType animationType;
            public float speed;
            public bool willKeep;
        }

        public AnimationInfo[] animations;
        private StageObject _bindedObject;

        private bool _isClipPlayed;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (_isClipPlayed) return;
            _isClipPlayed = true;

            _bindedObject = (StageObject)playerData;
            foreach (var animation in animations)
            {
                _bindedObject.SetAnimation(animation.animationType, animation.speed);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (!_isClipPlayed) return;
            _isClipPlayed = false;

            foreach (var animation in animations)
            {
                if (animation.willKeep) return;
                
                _bindedObject.SetIdleAnimation(animation.animationType);
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class SoundBehaviour : PlayableBehaviour
    {
        public SoundName soundName;
        public bool playInstantly;

        private bool _isClipPlayed;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)_isClipPlayed = false;
#endif
            if (_isClipPlayed) return;
            _isClipPlayed = true;
        
            if (soundName < SoundName.EndBGM_StartSE)
            {
                SoundManager.Instance.PlayBGM(soundName, playInstantly);
            }
            else
            {
                SoundManager.Instance.PlaySoundEffect(soundName).Forget();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)_isClipPlayed = false;
#endif
            if (!_isClipPlayed) return;
            _isClipPlayed = false;
        }

    }
}
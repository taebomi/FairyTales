using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class DialogueBehaviour : PlayableBehaviour
    {
        public EventManager.DialogueInfo dialogueInfo;

        private bool _isClipPlayed;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
        
            _isClipPlayed = true;
        
            EventManager.Instance.PrintDialogue(dialogueInfo);
        }
    
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
        
            if (!_isClipPlayed) return;
            _isClipPlayed = false;
        
            EventManager.Instance.FinishDialogue();
        }
    }
}
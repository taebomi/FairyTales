using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class OverlayCameraControlBehaviour : PlayableBehaviour
    {
        public Vector2 position;
        public float size;

        private bool _isClipPlayed;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (_isClipPlayed) return;
            _isClipPlayed = true;

            CameraManager.Instance.ActivateOverlayCamera(true);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            CameraManager.Instance.ControlOverlayCamera(position, size);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!_isClipPlayed) return;
            _isClipPlayed = false;

            CameraManager.Instance.ActivateOverlayCamera(false);
        }
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class CameraEffectBehaviour : PlayableBehaviour
    {
        public enum Mode
        {
            Shake,
            OverlayFadeBlack = 1,
            OverlayFadeWhite = 2,
            CameraFade = 5,
            Reminiscence = 100,
        }

        [Serializable]
        public struct CameraEffect
        {
            public Mode mode;
            public float value;
            public float duration;
            public bool willKeep;
        }

        public CameraEffect[] effects;

        private bool _isClipPlayed;
        
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
        
            _isClipPlayed = true;

            foreach (var effect in effects)
            {
                switch (effect.mode)
                {
                    case Mode.Shake:
                        CameraManager.Instance.ShakeCamera(effect.value, effect.duration).Forget();
                        break;
                    case Mode.OverlayFadeBlack:
                        UIManager.Instance.SetOverlayFadeColor(Color.black);
                        UIManager.Instance.FadeOverlay(effect.value, effect.duration).Forget();
                        break;
                    case Mode.OverlayFadeWhite:
                        UIManager.Instance.SetOverlayFadeColor(Color.white);
                        UIManager.Instance.FadeOverlay(effect.value, effect.duration).Forget();
                        break;
                    case Mode.CameraFade:
                        UIManager.Instance.FadeCamera(effect.value, effect.duration).Forget();
                        break;
                    case Mode.Reminiscence:
                        CameraManager.Instance.PlayEffect_Reminiscence(effect.value, effect.duration).Forget();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (!_isClipPlayed) return;
            _isClipPlayed = false;

            foreach (var effect in effects)
            {
                if (effect.willKeep) return;
            
                switch (effect.mode)
                {
                    case Mode.Shake:
                        CameraManager.Instance.StopShakingCamera();
                        break;
                    case Mode.OverlayFadeBlack:
                        break;
                    case Mode.CameraFade:
                        break;
                    case Mode.Reminiscence:
                        CameraManager.Instance.PlayEffect_Reminiscence(0f, effect.duration).Forget();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
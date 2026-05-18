using System;
using Cysharp.Threading.Tasks;
using FairyTales.EventSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.Timeline
{
    [Serializable]
    public class TimeControlBehaviour : PlayableBehaviour
    {
        public int id;
        #region 타임라인 컨트롤 타입

        public enum TimeControlType
        {
            Jump,
            Branch,
            Marker,
        }

        #endregion
        public TimeControlType timeControlType;
        public int[] destIds;
        public double[] destTimes;

        private bool _isClipPlayed;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (_isClipPlayed) return;
            _isClipPlayed = true;

            switch (timeControlType)
            {
                case TimeControlType.Branch:
                    EventManager.Instance.BranchTimelineByChoice(destTimes).Forget();
                    break;
                case TimeControlType.Jump:
                    break;
                case TimeControlType.Marker:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            if (!_isClipPlayed) return;
            _isClipPlayed = false;
            
            switch (timeControlType)
            {
                case TimeControlType.Jump:
                    EventManager.Instance.JumpTimeline(destTimes[0]);
                    break;
                case TimeControlType.Branch:
                    break;
                case TimeControlType.Marker:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
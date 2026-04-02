using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    [TrackColor(1f, 0.5f, 1f)]
    [TrackBindingType(typeof(StageObject))]
    [TrackClipType(typeof(AnimationClip))]
    public class AnimationTrack : TrackAsset
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);

            foreach (var timelineClip in GetClips())
            {
                var animationClip = ((AnimationClip)timelineClip.asset).template;
                if (animationClip.animations == null || animationClip.animations.Length == 0)
                {
                    timelineClip.displayName = "추가해 !";
                    continue;
                }

                timelineClip.displayName = "";
                for (var i = 0; i < animationClip.animations.Length; i++)
                {
                    timelineClip.displayName +=
                        Regex.Replace(animationClip.animations[i].animationType.ToString(), @".*_", "");
                    if (i != animationClip.animations.Length - 1)
                    {
                        timelineClip.displayName += ",";
                    }
                }
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
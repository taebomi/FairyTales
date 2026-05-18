using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    [TrackBindingType(typeof(EmotionableObject))]
    [TrackClipType(typeof(EmotionBubbleClip))]
    [TrackColor(0.52f,1f, 0.6f)]
    public class EmotionBubbleTrack : TrackAsset
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);
        
            // 클립 이름을 선택한 감정표현으로 출력해줌
            foreach (var timelineClip in GetClips())
            {
                var emotion = ((EmotionBubbleClip)timelineClip.asset).template.emotionType;
                timelineClip.displayName = emotion.ToString();
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
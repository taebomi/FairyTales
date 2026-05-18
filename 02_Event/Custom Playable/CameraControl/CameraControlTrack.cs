using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    [TrackColor(0, 0, 0)]
    [TrackClipType(typeof(CameraControlClip))]
    public class CameraControlTrack : TrackAsset
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);
        
            foreach (var timelineClip in GetClips())
            {
                timelineClip.displayName = "메인 카메라 컨트롤";
            }
        
            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
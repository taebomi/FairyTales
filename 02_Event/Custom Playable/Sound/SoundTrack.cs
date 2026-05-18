using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    [TrackColor(1f,1f,0f)]
    [TrackClipType(typeof(SoundClip))]
    public class SoundTrack : TrackAsset
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);
        
            foreach (var timelineClip in GetClips())
            {
                var soundName = ((SoundClip)timelineClip.asset).template.soundName;
                timelineClip.displayName = soundName switch
                {
                    // 브금
                    < SoundName.EndBGM_StartSE => $"BGM_{soundName.ToString()}",
                    // 효과음
                    > SoundName.EndBGM_StartSE => $"SE_{soundName.ToString()}",
                    _ => "설정 필요 ###"
                };
            }
        
            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
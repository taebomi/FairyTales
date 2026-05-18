using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.Timeline
{
    public class TimeControlClip : PlayableAsset, ITimelineClipAsset
    {
        public TimeControlBehaviour template;
        public ClipCaps clipCaps => ClipCaps.None; 
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeControlBehaviour>.Create(graph, template);
            
            return playable;
        }

    }
}

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    public class EmotionBubbleClip : PlayableAsset, ITimelineClipAsset
    {
        public EmotionBubbleBehaviour template = new ();
        public ClipCaps clipCaps => ClipCaps.None;
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EmotionBubbleBehaviour>.Create(graph, template);
        
            return playable;
        }

    }
}
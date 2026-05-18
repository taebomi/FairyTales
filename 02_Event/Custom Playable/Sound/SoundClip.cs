using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class SoundClip : PlayableAsset
    {
        public SoundBehaviour template = new();
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            Playable playable = ScriptPlayable<SoundBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
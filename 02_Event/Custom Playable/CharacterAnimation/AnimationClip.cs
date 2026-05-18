using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class AnimationClip : PlayableAsset
    {
        public AnimationBehaviour template = new ();
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AnimationBehaviour>.Create(graph, template);

            return playable;
        }
    }
}
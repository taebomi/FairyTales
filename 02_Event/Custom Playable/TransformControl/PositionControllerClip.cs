using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class PositionControllerClip : PlayableAsset
    {
        public PositionControllerBehaviour template = new ();
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PositionControllerBehaviour>.Create(graph, template);

            return playable;
        }
    }
}
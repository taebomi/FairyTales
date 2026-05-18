using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class OverlayCameraControlClip : PlayableAsset
    {
        public OverlayCameraControlBehaviour template = new();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<OverlayCameraControlBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
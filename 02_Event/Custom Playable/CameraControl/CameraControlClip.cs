using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class CameraControlClip : PlayableAsset
    {
        public CameraControlBehavior template = new();
    
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CameraControlBehavior>.Create(graph, template);
            return playable;
        }
    }
}
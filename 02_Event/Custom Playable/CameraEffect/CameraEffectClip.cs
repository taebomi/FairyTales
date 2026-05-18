using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public class CameraEffectClip : PlayableAsset
    {
        public CameraEffectBehaviour template = new ();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CameraEffectBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
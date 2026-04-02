using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    public class DialogueClip : PlayableAsset, ITimelineClipAsset
    {
        public DialogueBehaviour template = new (); // 클립에 정보를 담는 대신 Behaviour에 담고 여기서 가져옴
        public ClipCaps clipCaps => ClipCaps.None; // ITimelineClipAsset 인터페이스 상속, 클립 옵션 결정


        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, template);

            return playable;
        }
    }
}
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    public class ChoiceClip : PlayableAsset, ITimelineClipAsset
    {
#if UNITY_EDITOR
        public static string EventName; // 커스텀 에디터에서 인스펙터에 선택지 별 대사 출력하기 위해 사용
#endif
        public ClipCaps clipCaps => ClipCaps.None;

        public ChoiceBehaviour template;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChoiceBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
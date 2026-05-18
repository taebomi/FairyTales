using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace FairyTales.EventSystem
{
    [TrackClipType(typeof(ChoiceClip))]
    [TrackColor(0.85f, 0.5f, 0.3f)]
    public class ChoiceTrack : PlayableTrack
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);

            name = "선택지";
            ChoiceClip.EventName = timelineAsset.name;

            // 현재 스테이지의 이벤트 데이터 불러오기
            var stageEventData = EventManager.GetStageEventData();
            if (!stageEventData.TryGetValue(timelineAsset.name, out var currentEventData))
            {
                Debug.LogAssertion($"{timelineAsset.name} 이름의 이벤트 존재하지 않음.");
                return base.CreateTrackMixer(graph, go, inputCount);
            }

            // id 유효성 체크 및 클립 이름 설정
            foreach (var timelineClip in GetClips())
            {
                var choiceClip = (ChoiceClip)timelineClip.asset;
                var id = choiceClip.template.id;

                if (!currentEventData.choiceSet.ContainsKey(id))
                {
                    Debug.LogAssertion($"{id} 아이디인 선택지 데이터가 존재하지 않음.");
                    continue;
                }

                timelineClip.displayName = "선택지";
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
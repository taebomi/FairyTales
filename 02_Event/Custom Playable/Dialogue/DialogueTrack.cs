using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.EventSystem
{
    [TrackColor(0.85f, 0.5f, 0.3f)] // 트랙의 컬러 rgb값 결정
    [TrackClipType(typeof(DialogueClip))] // 트랙에 들어갈 클립 타입
    public class DialogueTrack : TrackAsset
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);

            // 현재 스테이지의 타임라인 이름인 이벤트 데이터 불러오기
            var currentStageEventData = EventManager.GetStageEventData();
            if (!currentStageEventData.TryGetValue(timelineAsset.name, out var eventData))
            {
                Debug.LogWarning("해당하는 이벤트가 존재하지 않음.");
                return base.CreateTrackMixer(graph, go, inputCount);
            }

            // 각 클립의 id에 해당하는 대사 내용을 clip의 이름으로 설정
            foreach (var timelineClip in GetClips())
            {
                var dialogueClip = (DialogueClip)timelineClip.asset;
                dialogueClip.template.dialogueInfo.pauseDuration = (float)timelineClip.duration;
                dialogueClip.template.dialogueInfo.timeToJumpTo = timelineClip.end;

                // 클립의 id에 해당하는 텍스트 읽기
                if (!eventData.dialogueSet.TryGetValue(dialogueClip.template.dialogueInfo.id, out var dialogue))
                {
                    continue;
                }

                timelineClip.displayName = Regex.Replace(dialogue, @"<[^<>]+>", "");
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
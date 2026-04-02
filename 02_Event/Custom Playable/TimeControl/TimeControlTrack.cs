using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FairyTales.Timeline
{
    [TrackClipType(typeof(TimeControlClip))]
    [TrackColor(1f, 1f, 1f)]
    public class TimeControlTrack : PlayableTrack
    {
#if UNITY_EDITOR
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            if (Application.isPlaying) return base.CreateTrackMixer(graph, go, inputCount);
            
            var clipDictionary = new Dictionary<int, TimelineClip>();

            foreach (var clip in GetClips()) // Dictionary에 담고 이름 변경
            {
                var timeControlClip = (TimeControlClip)clip.asset;
                var timeControlData = timeControlClip.template;
                switch (timeControlData.timeControlType)
                {
                    case TimeControlBehaviour.TimeControlType.Jump:
                        if (timeControlData.destIds.Length == 0)
                        {
                            Debug.LogAssertion($"{timeControlData.id} 클립의 {nameof(timeControlData.destIds)} 변수가 비어있음.");
                            continue;
                        }
                        clip.displayName = $"{timeControlData.id} => {timeControlData.destIds[0]}";
                        break;
                    case TimeControlBehaviour.TimeControlType.Marker:
                        clip.displayName = $"=> {timeControlData.id}";
                        break;
                    case TimeControlBehaviour.TimeControlType.Branch:
                        var displayName = "=> ";
                        for (var i = 0; i < timeControlData.destIds.Length; i++)
                        {
                            displayName += timeControlData.destIds[i];
                            if (i != timeControlData.destIds.Length - 1)
                            {
                                displayName += ",";
                            }
                        }

                        clip.displayName = displayName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (!clipDictionary.TryAdd(timeControlData.id, clip))
                {
                    Debug.LogAssertion($"{timeControlData.id} 삽입 불가");
                }
            }

            foreach (var controlClip in clipDictionary.Select(pair => (TimeControlClip)pair.Value.asset))
            {
                var timeControlData = controlClip.template;
                switch (timeControlData.timeControlType)
                {
                    case TimeControlBehaviour.TimeControlType.Marker:
                        continue;
                    case TimeControlBehaviour.TimeControlType.Jump:
                        if (!clipDictionary.TryGetValue(timeControlData.destIds[0], out var targetClip))
                        {
                            Debug.LogAssertion(timeControlData.destIds[0] + "클립이 존재하지 않음.");
                            continue;
                        }

                        timeControlData.destTimes = new double[1];
                        timeControlData.destTimes[0] = targetClip.start;

                        break;
                    case TimeControlBehaviour.TimeControlType.Branch:
                        timeControlData.destTimes = new double[timeControlData.destIds.Length];
                        for (var index = 0; index < timeControlData.destIds.Length; index++)
                        {
                            var destId = timeControlData.destIds[index];
                            if (!clipDictionary.TryGetValue(destId, out var destClip))
                            {
                                Debug.LogAssertion($"{destId} 아이디의 클립이 존재하지 않음.");
                                continue;
                            }

                            timeControlData.destTimes[index] = destClip.start;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }
#endif
    }
}
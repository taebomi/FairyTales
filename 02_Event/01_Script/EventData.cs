using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FairyTales.EventSystem
{
    public class StageEventData : Dictionary<string, EventData>
    {
    }

    [Serializable]
    public class EventData
    {
        [JsonProperty("dialogues")] public DialogueSet dialogueSet;
        [JsonProperty("choices")] public ChoiceSet choiceSet;
    }

    [Serializable]
    public class DialogueSet : Dictionary<int, string>
    {
    }

    [Serializable]
    public class ChoiceSet : Dictionary<int, string[]>
    {
    }
}
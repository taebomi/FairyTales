using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fairy Tales/Event Object Info")]
public class EventCondition : ScriptableObject
{
    [field: SerializeField] public string[] HaveToClearEventNames { get; private set; }
    [field: SerializeField] public string[] HaveToUnclearEventNames { get; private set; }

    public bool CheckCondition()
    {
        var saveManager = SaveManager.Instance;
        
        if (!saveManager.CheckAllEventCleared(HaveToClearEventNames)) return false;
        
        if (saveManager.CheckAnyEventCleared(HaveToUnclearEventNames)) return false;

        return true;
    }
}
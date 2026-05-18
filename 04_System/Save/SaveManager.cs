using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DG.Tweening;
using ES3Types;
using JetBrains.Annotations;
using Save;
using UnityEngine;


public class SaveManager : Singleton<SaveManager>
{
    public PlayerSaveData PlayerSaveData { get; private set; }


    protected override void AwakeAfter()
    {
        DontDestroyOnLoad(gameObject);

        Load();

        StageManager.StageStateChanged.AddListener(OnStageStateChanged);
    }

    private void OnStageStateChanged(StageState changedState)
    {
        switch (changedState)
        {
            case StageState.None:
                break;
            case StageState.FirstPlaying:
                break;
            case StageState.Playing:
                break;
            case StageState.StaticEvent:
                break;
            case StageState.GameOver:
                break;
            case StageState.Reset:
                // todo: 세이브해야해
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(changedState), changedState, null);
        }
    }

    public void Save()
    {
        ES3.Save("PlayerSaveData", PlayerSaveData);
    }

    public void Save(string savePointName, Vector2 position)
    {
        PlayerSaveData.savePointName = savePointName;
        PlayerSaveData.playerPosition = position;
        ES3.Save("PlayerSaveData", PlayerSaveData);
    }

    public void Load()
    {
        PlayerSaveData = ES3.Load("PlayerSaveData", new PlayerSaveData());
    }

    #region 이벤트 진행도

    public void SetEventCleared(string eventName)
    {
        if (PlayerSaveData.EventProgressDict.ContainsKey(eventName))
        {
            PlayerSaveData.EventProgressDict[eventName] = true;
        }
        else
        {
            PlayerSaveData.EventProgressDict.Add(eventName, true);
        }
    }


    // 해당 이름의 이벤트 클리어 했는지 체크
    public bool CheckEventCleared(string eventName)
    {
        return PlayerSaveData.EventProgressDict.TryGetValue(eventName, out var hasCleared) && hasCleared;
    }

    // 이벤트들을 전부 클리어 했는지 체크
    public bool CheckAllEventCleared(IEnumerable<string> eventNames)
    {
        return eventNames.All(CheckEventCleared);
    }

    public bool CheckAnyEventCleared(IEnumerable<string> eventNames)
    {
        return eventNames.Any(CheckEventCleared);
    }

    #endregion
}
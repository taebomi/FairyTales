using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;


namespace Save
{
    [Serializable]
    public class PlayerSaveData
    {
        public Vector2 playerPosition;
        public string savePointName;
        public Dictionary<string, bool> EventProgressDict;

        public PlayerSaveData()
        {
            playerPosition = new Vector2(-31.75f, 12.5f);
            EventProgressDict = new Dictionary<string, bool>();
        }
    }
}
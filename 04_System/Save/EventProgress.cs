using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class EventProgress
    {
        public bool HasSeenBefore { get; private set; }
        public bool HasClearedBefore { get; private set; }
        
        // 현재 목숨의 이벤트 정보
        public bool HasSeenCurrentLife { get; private set; }
        public bool HasClearedCurrentLife{ get; private set; }
        
        // 회귀 지점까지의 이벤트 정보
        private bool _hasSeenSaved;
        private bool _hasClearedSaved;
        
        public EventProgress()
        {
            _hasSeenSaved = false;
            _hasClearedSaved = false;
            HasSeenBefore = false;
            HasClearedBefore = false;
            HasSeenCurrentLife = false;
            HasClearedCurrentLife = false;
        }
        

        public EventProgress(bool hasSeen)
        {
            _hasSeenSaved = false;
            _hasClearedSaved = false;
            HasSeenBefore = hasSeen;
            HasClearedBefore = false;
            HasSeenCurrentLife = hasSeen;
            HasClearedCurrentLife = false;
        }
        
        public void See()
        {
            HasSeenBefore = true;
            HasSeenCurrentLife = true;
        }

        public void Clear()
        {
            HasClearedCurrentLife = true;
            HasClearedBefore = true;
        }

        public void Memorize()
        {
            _hasSeenSaved = HasSeenCurrentLife;
            _hasClearedSaved = HasClearedCurrentLife;
        }

        public void Regress()
        {
            HasSeenCurrentLife = _hasSeenSaved;
            HasClearedCurrentLife = _hasClearedSaved;
        }

    }
}
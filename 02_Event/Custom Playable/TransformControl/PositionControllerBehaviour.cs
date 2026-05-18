using System;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    [Serializable]
    public class PositionControllerBehaviour : PlayableBehaviour
    {
        public Vector2 position;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ((Transform)playerData).localPosition = position;
        }
    }
}
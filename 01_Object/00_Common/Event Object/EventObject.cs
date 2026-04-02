using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    [SerializeField] private InitData[] initData;

    public void Initiate()
    {
        for (var i = initData.Length - 1; i >= 0; i--)
        {
            if (i == 0 && !initData[0].eventCondition)
            {
                ApplyData(initData[0]);
                return;
            }
            
            if (initData[i].eventCondition.CheckCondition())
            {
                ApplyData(initData[i]);
                return;
            }
        }
    }

    public void ApplyData(InitData data)
    {
        transform.localPosition = data.position;
        GetComponent<StageObject>().SetActAnimation(data.animationType);
        gameObject.SetActive(true);
    }


    [Serializable]
    public class InitData
    {
        public EventCondition eventCondition;
        public Vector2 position;
        public AnimationType animationType;
    }
}
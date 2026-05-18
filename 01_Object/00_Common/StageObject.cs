using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// 스테이지에 등장하는 모든 오브젝트들
/// </summary>
public partial class StageObject : MonoBehaviour
{
    [field:SerializeField] public Animator MainAnimator { get; private set; }


    protected virtual void OnDisable()
    {
        SetActAnimation(AnimationType.Act_Idle);
    }
}

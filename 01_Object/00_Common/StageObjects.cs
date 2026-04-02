using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjects : StageObject
{
    [SerializeField] private StageObject[] stageObjects;

    private void OnEnable()
    {
        transform.SetActiveChildren(true);
    }

    protected override void OnDisable()
    {
        transform.SetActiveChildren(false);
    }

    public override void SetAnimation(AnimationType animationType, float speed)
    {
        foreach (var stageObject in stageObjects)
        {
            stageObject.SetAnimation(animationType, speed);
        }
    }

    public override void SetIdleAnimation(AnimationType animationType)
    {
        foreach (var stageObject in stageObjects)
        {
            stageObject.SetIdleAnimation(animationType);
        }
    }
}

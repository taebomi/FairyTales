using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAnt : StageObject
{


    public void AnimationEvent_DeadFinish()
    {
        gameObject.SetActive(false);
    }

}

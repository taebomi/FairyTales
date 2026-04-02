using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;



public static class TaeBoMiExtensionMethods
{
    /// <summary>
    /// cancel이 안되었다면 cancel시키고, dispose
    /// </summary>
    public static void CancelAndDispose(this CancellationTokenSource cts)
    {
        if (!cts.IsCancellationRequested) cts.Cancel();
        cts.Dispose();
    }
    
    public static async UniTask AsyncWaitForStart(this Animator animator, int nameHash)
    {
        await UniTask.WaitUntil(() =>
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.shortNameHash == nameHash;
        });
    }

    public static async UniTask AsyncWaitForComplete(this Animator animator, int nameHash)
    {
        await UniTask.WaitUntil(() =>
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.shortNameHash == nameHash;
        });

        await UniTask.WaitUntil(() =>
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.normalizedTime >= 1f;
        });
    }

    public static async UniTask AsyncWaitForNormalizedTime(this Animator animator, int nameHash, float normalizedTime)
    {
        await UniTask.WaitUntil(() =>
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.shortNameHash == nameHash && stateInfo.normalizedTime >= normalizedTime;
        });
    }

    #region Transform

    public static void SetActiveChildren(this Transform transform, bool willActivate)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(willActivate);
        }
    }
    

    #endregion
}
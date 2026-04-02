using System;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

public class EmotionBubble : PoolableObject<EmotionBubble>
{
    [SerializeField] private Animator animator;

    public enum EmotionType
    {
        Dialogue,
        Exclamation,
        Question,
        Heart,
        Sad,
        Angry,
        Fun,
        Droplets,
        Save = 100,
        
    }

    /// <summary>
    /// 감정방울 생성 및 효과음 실행
    /// </summary>
    public void Create(EmotionType emotionType, Transform containerTr)
    {
        transform.SetParent(containerTr, false);
        animator.SetInteger(AnimatorHash.Type, (int)emotionType);
        SoundManager.Instance.PlaySoundEffect((int)emotionType + SoundName.Emotion).Forget();
    } 
    
    /// <summary>
    /// 닫는 애니메이션으로 전환
    /// </summary>
    public void Close()
    {
        animator.SetInteger(AnimatorHash.Type, -1);
        
        ReleaseWhenFinished().Forget();
    }
    

    /// <summary>
    /// 닫는 애니메이션 완료 후 릴리즈
    /// </summary>
    private async UniTaskVoid ReleaseWhenFinished()
    {
        await animator.AsyncWaitForComplete(AnimatorHash.Close);
        ManagedPool.Release(this);
    }


    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
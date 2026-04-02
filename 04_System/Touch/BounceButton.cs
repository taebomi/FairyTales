using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BounceButton : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    private Tweener _bounceTweener;
    
    private void Awake()
    {
        _bounceTweener = rectTransform.DOScale(1f, 0.25f)
            .From(0)
            .SetEase(Ease.OutBounce)
            .SetAutoKill(false);
    }

    // ReSharper disable once Unity.IncorrectMethodSignature
    private void OnEnable()
    {
        Bounce().Forget();
    }

    private async UniTaskVoid Bounce()
    {
        _bounceTweener.Restart();
        await _bounceTweener.AwaitForComplete();
        gameObject.SetActive(false);
        _bounceTweener.Rewind();
    }
}

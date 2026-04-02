using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlayerGhostEffect : PoolableObject<PlayerGhostEffect>
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private TweenerCore<Color, Color, ColorOptions> _fadeTweener;

    private void Awake()
    {
        _fadeTweener = spriteRenderer
            .DOFade(0, 0.333f)
            .From(1f)
            .SetAutoKill(false);
    }

    private void OnDestroy()
    {
        _fadeTweener.Kill();
    }

    public async UniTaskVoid MakeGhostEffect(Vector3 position, Sprite sprite, bool isFlip)
    {
        transform.position = position;
        spriteRenderer.sprite = sprite;
        spriteRenderer.flipX = isFlip;
        
        gameObject.SetActive(true);
        _fadeTweener.Restart();
        await _fadeTweener.AsyncWaitForCompletion();
        
        gameObject.SetActive(false);
        ManagedPool.Release(this);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FairyTales.Layer;
using UnityEngine;
using UnityEngine.UIElements;

public class AlrauneWall : PoolableObject<AlrauneWall>
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Transform colliderTransform;

    private float _height;

    

    public async UniTaskVoid Create(Vector3 upDirection)
    {
        var tr = transform;
        tr.up = upDirection;

        var hit = Physics2D.Raycast(tr.position, upDirection, 20f,
            LayerCache.GetLayerMask(LayerName.Wall));

        var desiredLength = 20f;
        if (hit)
        {
            desiredLength = hit.distance + 0.15f;
        }

        var currentLength = 0f;
        const float increasingSpeed = 5f;

        while (currentLength < desiredLength)
        {
            ChangeLength(currentLength);
            currentLength += increasingSpeed * Time.deltaTime;
            await UniTask.Yield();
        }

        ChangeLength(desiredLength);
    }

    public async UniTaskVoid Remove()
    {
        const float desiredLength = 0f;
        var currentLength = sr.size.y;
        var decreasingSpeed = currentLength * 0.5f;

        while (currentLength > desiredLength)
        {
            currentLength -= decreasingSpeed * Time.deltaTime;
            ChangeLength(currentLength);
            await UniTask.Yield();
        }

        ChangeLength(0f);
        ManagedPool.Release(this);
    }

    private void ChangeLength(float length)
    {
        sr.size = new Vector2(0.5f, length);
        colliderTransform.localScale = new Vector3(1f, length, 1f);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public partial class PlayerScript
{
    private void InitializeSkill()
    {
        _ghostEffectContainer = new GameObject("PlayerGhostEffect Pool").transform;
        _ghostEffectContainer.SetParent(StageManager.Instance.PoolContainer);
        _ghostEffectPool = new ObjectPool<PlayerGhostEffect>(createFunc: CreateGhostEffect,
            actionOnGet: OnGetGhostEffect,
            actionOnDestroy: OnDestroyGhostEffect, maxSize: 10);
        
        
    }
    
    
    #region 데쉬 고스트 이펙트 풀링
    private Transform _ghostEffectContainer;                              // 풀링될 고스트 이펙트들을 담아두는 트랜스폼
    [SerializeField] private PlayerGhostEffect ghostEffectPrefab;         // 고스트 이펙트 원본 프리팹
    private IObjectPool<PlayerGhostEffect> _ghostEffectPool;              // 고스트 이펙트 풀

    private PlayerGhostEffect CreateGhostEffect()
    {
        var ghostEffect = Instantiate(ghostEffectPrefab, _ghostEffectContainer);
        ghostEffect.SetManagedPool(_ghostEffectPool);
        return ghostEffect;
    }

    private void OnGetGhostEffect(PlayerGhostEffect ghostEffect)
    {
        ghostEffect.MakeGhostEffect(transform.position, MainSpriteRenderer.sprite, MainSpriteRenderer.flipX).Forget();
    }

    private void OnDestroyGhostEffect(PlayerGhostEffect ghostEffect)
    {
        Destroy(ghostEffect.gameObject);
    }

    public void MakeGhostEffect()
    {
        _ghostEffectPool.Get();
    }
    #endregion



}

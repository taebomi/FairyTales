using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlrauneThorn : PoolableObject<AlrauneThorn>
{
    [SerializeField] private Animator mainAnimator;
    [SerializeField] private AnimatorOverrideController[] randomAnimatorOverrideControllers;

    public async UniTaskVoid Create(Vector3 pos)
    {
        transform.position = pos;
        mainAnimator.runtimeAnimatorController =
            randomAnimatorOverrideControllers[Random.Range(0, randomAnimatorOverrideControllers.Length)];
        await UniTask.Delay(1500);
        ManagedPool.Release(this);
    }
}
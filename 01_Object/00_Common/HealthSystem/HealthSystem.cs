using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthSystem : MonoBehaviour
{
    [field: SerializeField] public int MaxHp { get; protected set; }
    [field: SerializeField] public int CurrentHp { get; protected set; }
    
    protected bool IsLive => CurrentHp > 0;

    public abstract void Damage(int damage);

    public abstract void Damage(int damage, Vector2 collisionPoint);

}

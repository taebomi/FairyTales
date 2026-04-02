using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool Damage(int dmg); // 반환값 : 사망 여부
}

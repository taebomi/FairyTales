using System;
using Unity.VisualScripting;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damage = 1;
    
    private void OnTriggerEnter2D(Collider2D col)
    {  
        col.GetComponent<HealthSystem>().Damage(damage);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        col.gameObject.GetComponent<HealthSystem>().Damage(damage,col.contacts[0].point);
    }
}
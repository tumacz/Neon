using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamage
{
    [SerializeField] private int _startingHealth = 10;
    protected int _health;
    protected bool _dead = false;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        _health = _startingHealth;
    }

    public void TakeHit(int damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0 && !_dead)
        {
            Die();
        }
    }

    protected void Die()
    {
        _dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}

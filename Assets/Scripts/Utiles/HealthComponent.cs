using UnityEngine;

public class HealthComponent : MonoBehaviour, IDamage
{
    public int _startingHealth = 10;
    protected int _health;
    protected bool _dead = false;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        _health = _startingHealth;
    }

    public virtual void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // Do some stuff here with hit var
        TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0 && !_dead)
        {
            Die();
        }
    }

    [ContextMenu("SelfDestruct")]
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

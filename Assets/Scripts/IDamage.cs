using UnityEngine;

public interface IDamage
{
    void TakeHit(int damage, RaycastHit hit);

    void TakeDamage(int damage);
}
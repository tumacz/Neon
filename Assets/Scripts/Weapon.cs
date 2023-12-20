using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _timeGap;
    [SerializeField] private Transform _projectileParent;
    private float _nextShotTime;

    public void Shoot()
    {
        if (Time.time > _nextShotTime)
        {
            _nextShotTime = Time.time + _timeGap;
            Projectile newProjectile = Instantiate(_projectile, _shootPoint.position, _shootPoint.rotation);
        }
    }
}

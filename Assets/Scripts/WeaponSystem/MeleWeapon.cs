using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : WeaponBase
{
    [SerializeField] public Transform _shootPoint;
    void Start()
    {
        Debug.Log("firstattempt");
    }

    void Update()
    {

    }

    public override void AimWeapon(Vector3 aimPoint)
    {
            transform.LookAt(aimPoint);
    }

    public override void OnTriggerHold()
    {
        Debug.Log("Trigger");
    }

    public override void OnTriggerRelease()
    {
        Debug.Log("Release");
    }

    public override void Reload()
    {
        Debug.Log("Reload");
    }

    public override void Shoot()
    {
        Debug.Log("Shoot");
    }
}

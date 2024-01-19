using UnityEngine;

public interface IWeapon
{
    void Shoot();
    void Reload();
    void Aim(Vector3 aimPoint);
    void OnTriggerHold();
    void OnTriggerRelease();
}

using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    public abstract void Shoot();
    public abstract void Reload();
    public abstract void Aim(Vector3 aimPoint);
    public abstract void OnTriggerHold();
    public abstract void OnTriggerRelease();
}

using UnityEngine;

public interface IWeapon
{
    void Reload();
    void AimWeapon(Vector3 aimPoint);
    void OnTriggerHold();
    void OnTriggerRelease();
}

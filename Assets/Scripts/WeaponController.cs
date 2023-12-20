using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private Weapon _startGun;

    public void Start()
    {
        if(_startGun != null)
        {
            EquipWeapon(_startGun);
        }
    }
    public void EquipWeapon(Weapon weaponToEquip)
    {
        if(_currentWeapon != null)
        {
            Destroy(_currentWeapon.gameObject);
        }
        _currentWeapon = Instantiate(weaponToEquip, _weaponHolder.position, _weaponHolder.rotation);
        _currentWeapon.transform.parent = _weaponHolder;
    }

    public void WeaponShoot()
    {
        if (_currentWeapon != null)
        {
            _currentWeapon.Shoot();
        }
        else
            Debug.Log("no weapon to preform this action");
    }
}

﻿using UnityEngine;
using Zenject;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private WeaponBase _startWeapon;
    public WeaponBase _currentWeapon;
    private Dumpster _dumpster;

    [Inject]
    public void Construct(Dumpster dumpster)
    {
        _dumpster = dumpster;
        Debug.Log("dumpster installed");
    }

    private void Start()
    {
        if (_startWeapon != null)
        {
            EquipWeapon(_startWeapon);
        }
    }

    private WeaponBase CurrentWeapon
    {
        get => _currentWeapon;
        set
        {
            if (_currentWeapon != null)
            {
                _currentWeapon = null;
            }
            _currentWeapon = Instantiate(value, _weaponHolder.position, _weaponHolder.rotation, _weaponHolder);
            if(_currentWeapon is FireArms)
            {
                _currentWeapon.SetDumpster(_dumpster);
            }
        }
    }

    public void EquipWeapon(WeaponBase weaponToEquip)
    {
        CurrentWeapon = weaponToEquip;
    }

    public void AimWeapon(WeaponBase weapon, Vector3 aimPoint)
    {
        if (weapon != null)
        {
            weapon.AimWeapon(aimPoint);
        }
    }

    public void OnTriggerHold(WeaponBase weapon)
    {
        if (weapon != null)
        {
            weapon.OnTriggerHold();
        }
        else
        {
            LogNoWeapon();
        }
    }

    public void OnTriggerRelease(WeaponBase weapon)
    {
        if (weapon != null)
        {
            weapon.OnTriggerRelease();
        }
        else
        {
            LogNoWeapon();
        }
    }

    public void Reload(WeaponBase weapon)
    {
        if (weapon != null)
        {
            weapon.Reload();
        }
    }

    private void LogNoWeapon()
    {
        Debug.Log("No weapon to perform this action");
    }
}

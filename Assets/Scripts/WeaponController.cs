using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Transform _weaponHolder;
    [SerializeField] private Weapon _startWeapon;
    public Weapon _currentWeapon;

    private void Start()
    {
        if (_startWeapon != null)
        {
            EquipWeapon(_startWeapon);
        }
    }

    private Weapon CurrentWeapon
    {
        get => _currentWeapon;
        set
        {
            if (_currentWeapon != null)
            {
                Destroy(_currentWeapon.gameObject);
            }
            _currentWeapon = Instantiate(value, _weaponHolder.position, _weaponHolder.rotation, _weaponHolder);//zapytaæ o value i czy to zagrapozniej w playercontoller
        }
    }

    public void EquipWeapon(Weapon weaponToEquip) // przygotowanie pod system zmiany broni, mo¿e byæ wywo³ywane po zetknieciu sie z zbieranym przedmiotem, mo¿na siê pokusic o zmiane na coœ bardziej generycznego np jakies znajdzki typu zdrowie itp
    {
        CurrentWeapon = weaponToEquip;
    }

    public void AimWeapon(Weapon weapon, Vector3 aimPoint) //delikatny ruch broni palnej, broñ nie jest statyczna wzgledem postaci
    {
        if (weapon != null)
        {
            weapon.AimWeapon(aimPoint);
        }
    }

    public void OnTriggerHold(Weapon weapon) //nacisniecie klawisza ataku
    {
        if (weapon != null)
        {
            weapon.OnTriggerHold();
        }
        else
            LogNoWeapon();
    }

    public void OnTriggerRelease(Weapon weapon)//zwolnienie klawisza ataku
    {
        if (weapon != null)
        {
            weapon.OnTriggerRelease();
        }
        else
            LogNoWeapon();
    }

    private void Reload(Weapon weapon) //przeladowanie po wykonczeniu sie dostepnych pociskow w magazynku, mozna upublicznic i przypisaæ akcje w PlayerController
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : WeaponBase
{
    //public enum FireMode { Auto, Burst, Single }//mozna zmienic na np pistol, machinegun, shootgun
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private Transform _shell;
    [SerializeField] private Transform _shellEjectionPoint;
    [SerializeField] public Transform _shootPoint;

    // operational variables
    private float _recoilRotationSmoothDampVelocity;
    private float _recoilAngle;
    private Vector3 _recoilSmoothDampVelocity;
    private PlayerSO _playerSO;
    private MuzzleFlash _muzzleFlash;
    private Dumpster _dumpster;
    private float _nextShotTime;
    private int _shotsRemainingInBurst;
    private int _projectilesRamainingInMagazine;
    private bool _isReloading;
    private bool _triggerRelasedSinceLastShoot;

    private void Start()
    {
        _dumpster = FindObjectOfType<Dumpster>();
        _projectileParent = _dumpster.transform;
        _playerSO = FindObjectOfType<PlayerSO>();
        _muzzleFlash = GetComponent<MuzzleFlash>();
       // _shotsRemainingInBurst = _weaponData._burstCount;
        _projectilesRamainingInMagazine = _weaponData._projectilesPerMagazine;
    }

    private void LateUpdate()
    {
        if (!_isReloading && _projectilesRamainingInMagazine == 0)
        {
            Reload();
        }
    }

    private void HandleProjectile() //redo
    {
        _nextShotTime = Time.time + _weaponData._timeGap;
        Projectile newProjectile = Instantiate(_weaponData._projectile, _shootPoint.position, _shootPoint.rotation, _projectileParent);
        _projectilesRamainingInMagazine--;
        _muzzleFlash.Activate();
        Instantiate(_shell, _shellEjectionPoint.position, _shellEjectionPoint.rotation, _projectileParent);
        transform.localPosition -= Vector3.forward * UnityEngine.Random.Range(_weaponData._recoilKickMinMax.x, _weaponData._recoilKickMinMax.y);
        _recoilAngle += UnityEngine.Random.Range(_weaponData._recoilRotationMinMax.x, _weaponData._recoilRotationMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30);
        StartCoroutine(ApplyRecoil());
    }

    private IEnumerator AnimateReload()
    {
        _isReloading = true;
        float reloadSpeed = 1f / _weaponData._reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30f;
        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null;
        }
        yield return new WaitForSeconds(.1f);

        _isReloading = false;
        _projectilesRamainingInMagazine = _weaponData._projectilesPerMagazine;
    }

    private IEnumerator ApplyRecoil()
    {
        while (Mathf.Abs(_recoilAngle) > 0.01f || Mathf.Abs(transform.localPosition.magnitude) > 0.01f)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, _weaponData._recoilReturnMovementTime);

            _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotationSmoothDampVelocity, _weaponData._recoilReturnRotationTime);
            transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;

            yield return null;
        }
    }

    public override void Shoot()
    {
        bool canShoot = Time.time > _nextShotTime && _projectilesRamainingInMagazine > 0;

        if (_weaponData._fireMode == WeaponData.FireMode.Burst)
        {
            if (_shotsRemainingInBurst == 0 || !canShoot) return;

            HandleProjectile();
            _shotsRemainingInBurst--;
        }
        else if ((_weaponData._fireMode == WeaponData.FireMode.Single && _triggerRelasedSinceLastShoot) || _weaponData._fireMode == WeaponData.FireMode.Auto)
        {
            if (canShoot) HandleProjectile();
        }
    }

    public override void Reload()
    {
        if (!_isReloading && _projectilesRamainingInMagazine != _weaponData._projectilesPerMagazine)
        {
            StartCoroutine(AnimateReload());
        }
    }

    public override void AimWeapon(Vector3 aimPoint)
    {
        if (!_isReloading)
            transform.LookAt(aimPoint);
    }

    public override void OnTriggerHold()
    {
        if (!_isReloading)
        {
            Shoot();
            _triggerRelasedSinceLastShoot = false;
        }
    }

    public override void OnTriggerRelease()
    {
        _triggerRelasedSinceLastShoot = true;
        _shotsRemainingInBurst = _weaponData._burstCount;
    }
}
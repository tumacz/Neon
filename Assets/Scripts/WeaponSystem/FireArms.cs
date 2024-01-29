using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArms : WeaponBase
{
    // Eventy na zmianê iloœci amunicji i magazynków
    public event Action<int> OnAmmoCountChanged; //UI ready!!!
    public event Action<int> OnMagazinesCountChanged; //UI ready!!!

    #region Serializeable
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _projectileParent;
    [SerializeField] private Transform _shellEjectionPoint;
    [SerializeField] public Transform _shootPoint;
    #endregion

    #region operational variables
    private float _recoilRotationSmoothDampVelocity;//ref
    private Vector3 _recoilSmoothDampVelocity;//ref

    private float _recoilAngle;
    private float _nextShotTime;

    private MuzzleFlash _muzzleFlash;
    private Dumpster _dumpster;

    private bool _canReload;
    private bool _isReloading;
    private bool _triggerRelasedSinceLastShoot;
    #endregion

    #region ammo variables
    private int _ammoRemainingInBurst;
    private int _ammoRamainingInMagazine;
    public int _magazinesCount = 2;
    #endregion

    private void Start()
    {
        GetReferences();

        _ammoRamainingInMagazine = _weaponData._ammoPerMagazine;
    }

    private void LateUpdate()//events?
    {
        if (!_isReloading && _ammoRamainingInMagazine == 0 )
        {
            if (_magazinesCount == 0)
            {
                _canReload = false;
                if(_triggerRelasedSinceLastShoot == false)
                Debug.Log("No magazines to reload");
                return;
            }
            Reload();
        }
    }

    private void HandleProjectile()
    {
        _nextShotTime = Time.time + _weaponData._timeGap;
        Projectile newProjectile = Instantiate(_weaponData._projectile, _shootPoint.position, _shootPoint.rotation, _projectileParent);
        _ammoRamainingInMagazine--;
        _canReload = true;
        _muzzleFlash.Activate();
        Instantiate(_weaponData._shell, _shellEjectionPoint.position, _shellEjectionPoint.rotation, _projectileParent);
        transform.localPosition -= Vector3.forward * UnityEngine.Random.Range(_weaponData._recoilKickMinMax.x, _weaponData._recoilKickMinMax.y);
        _recoilAngle += UnityEngine.Random.Range(_weaponData._recoilRotationMinMax.x, _weaponData._recoilRotationMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, 0, 30);
        StartCoroutine(ApplyRecoil());

        if (OnAmmoCountChanged != null)
            OnAmmoCountChanged.Invoke(_ammoRamainingInMagazine);

        if (OnMagazinesCountChanged != null)
            OnMagazinesCountChanged.Invoke(_magazinesCount);
    }

    private IEnumerator AnimateReload()
    {
        _isReloading = true;
        float reloadSpeed = 1f / _weaponData._reloadTime;
        float percent = 0;
        float maxReloadAngle = 30f;
        Vector3 initialRot = transform.localEulerAngles;

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
        _ammoRamainingInMagazine = _weaponData._ammoPerMagazine;

        if (OnAmmoCountChanged != null)
            OnAmmoCountChanged.Invoke(_ammoRamainingInMagazine);

        if (OnMagazinesCountChanged != null)
            OnMagazinesCountChanged.Invoke(_magazinesCount);
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

    private void Shoot()
    {
        bool canShoot = Time.time > _nextShotTime && _ammoRamainingInMagazine > 0;

        if (_weaponData._fireMode == WeaponData.FireMode.Burst) // seria
        {
            if (_ammoRemainingInBurst == 0 || !canShoot) 
                return;

            HandleProjectile();
            _ammoRemainingInBurst--;
        }
        else if ((_weaponData._fireMode == WeaponData.FireMode.Single && _triggerRelasedSinceLastShoot) || _weaponData._fireMode == WeaponData.FireMode.Auto) //single & auto
        {
            if (canShoot)
                HandleProjectile();
        }
    }

    public override void Reload()
    {
        if (_canReload)
        {
            if (!_isReloading && _ammoRamainingInMagazine != _weaponData._ammoPerMagazine && _magazinesCount>0)
            {
                StartCoroutine(AnimateReload());
                _magazinesCount--;
                _canReload = false;
            }
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
        _ammoRemainingInBurst = _weaponData._burstCount;
    }

    private void GetReferences()
    {
        _dumpster = FindObjectOfType<Dumpster>();
        _projectileParent = _dumpster.transform;
        _muzzleFlash = GetComponent<MuzzleFlash>();
    }
}
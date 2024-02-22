using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleWeapon : WeaponBase
{
    [SerializeField] public Transform _shootPoint;
    [SerializeField] Animation _swingAnim;

    private Dumpster _dumpster;

    private Vector3 _recoilSmoothDampVelocity;
    private float _recoilAngle =  200;
    private float _recoilRotationSmoothDampVelocity;

    private float _timeGap = 1f;
    private float _recoilReturnMovementTime = .5f;
    private float _recoilReturnRotationTime = .5f;
    private float _nextShotTime;

    private Vector2 _recoilKickMinMax = new Vector2(.3f, .6f);
    private Vector2 _recoilRotationMinMax = new Vector2(150, 220);

    private bool _triggerRelasedSinceLastShoot;


    public override void AimWeapon(Vector3 aimPoint)
    {
        if (_swingAnim == null)
        {
            transform.LookAt(aimPoint);
        }
    }

    public override void OnTriggerHold()
    {
        if (_swingAnim == null)
        {
            Shoot();
            _triggerRelasedSinceLastShoot = false;
        }

    }

    public override void OnTriggerRelease()
    {
        _triggerRelasedSinceLastShoot = true;
    }

    public override void Reload()
    {
        Debug.Log("Reload");
    }
    public override void SetDumpster(Dumpster dumpster)
    {
        _dumpster = dumpster;
    }

    private void Shoot()
    {
        if (_triggerRelasedSinceLastShoot)
        {
            bool canShoot = Time.time > _nextShotTime;
            if (!canShoot) return;
            HandleSwing();
        }
    }

    private IEnumerator ApplyReturn()
    {
        while (Mathf.Abs(_recoilAngle) > 0.01f || Mathf.Abs(transform.localPosition.magnitude) > 0.01f)
        {
            //swing up
            transform.localEulerAngles = transform.localEulerAngles + Vector3.left * _recoilAngle;
            //swing left
            transform.localEulerAngles = transform.localEulerAngles + Vector3.down * _recoilAngle;
            //rotate right
            transform.localEulerAngles = transform.localEulerAngles + Vector3.forward * _recoilAngle;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref _recoilSmoothDampVelocity, _recoilReturnMovementTime);
            _recoilAngle = Mathf.SmoothDamp(_recoilAngle, 0, ref _recoilRotationSmoothDampVelocity, _recoilReturnRotationTime);

            yield return null;
        }
    }
    private void HandleSwing()
    {
        _nextShotTime = Time.time + _timeGap;

        transform.localPosition -= Vector3.right * UnityEngine.Random.Range(_recoilKickMinMax.x, _recoilKickMinMax.y);
        _recoilAngle += UnityEngine.Random.Range(_recoilRotationMinMax.x, _recoilRotationMinMax.y);
        _recoilAngle = Mathf.Clamp(_recoilAngle, _recoilRotationMinMax.x, _recoilRotationMinMax.y);
        StartCoroutine(ApplyReturn());
    }

    //redo
    public override int GetAmmoCount()
    {
        return 0;
    }

    public override int GetMagazinesCount()
    {
        return 0;
    }
}

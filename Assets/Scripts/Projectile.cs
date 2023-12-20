using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Projectile : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _speed = 50f;
    [SerializeField] private float _timeToDestroy = 4f;
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _projectileRay = .1f; // safety increase collision detection

    private float _moveDistance;

    private void OnEnable()
    {
        Destroy(gameObject, _timeToDestroy);
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, _collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    private void Update()
    {
        MoveProjectile();
        CheckCollisions(_moveDistance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, moveDistance + _projectileRay, _collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
        }
    }

    private void MoveProjectile()
    {
        _moveDistance = _speed * Time.deltaTime;
        transform.Translate(Vector3.forward * _moveDistance);
    }

    private void OnHitObject(RaycastHit hit)
    {
        IDamage damageObject = hit.collider.GetComponent<IDamage>();
        if(damageObject != null)
        {
            damageObject.TakeHit(_damage, hit);
        }
        GameObject.Destroy(gameObject);
    }

    private void OnHitObject(Collider collider)
    {
        IDamage damageObject = collider.GetComponent<IDamage>();
        if (damageObject != null)
        {
            damageObject.TakeDamage(_damage);
        }
        GameObject.Destroy(gameObject);
    }
}
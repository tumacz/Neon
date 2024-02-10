using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class Enemy : HealthComponent
{
    public enum State { Idle, Chasing, Attacking};
    State _currentState;

    private NavMeshAgent _nav;
    private Transform _target;
    private Material _skinMaterial;
    private HealthComponent _targetHealth;
    private bool _hasTarget;

    [SerializeField] private float _attackDistanceThreshold = 0.5f;
    [SerializeField] private float _timeBetweenAttacks = 5f;
    [SerializeField] private float _nextAttackTime;
    [SerializeField] private Color _originalColor;
    [SerializeField] private ParticleSystem _enemyHitEffect;
    [SerializeField] private int _damageRate = 1;

    private float _enemyCollsionRadius;
    private float _targetCollsionRadius;

    private void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();


        if (GameObject.FindGameObjectWithTag("Player") != null)
        {

            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _targetHealth = _target.GetComponent<HealthComponent>();
            _hasTarget = true;

            _enemyCollsionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollsionRadius = _target.GetComponent<CapsuleCollider>().radius;
        }
    }
    protected override void Start()
    {
        base.Start();
        if (_hasTarget)
        {
            _currentState = State.Chasing;
            _targetHealth.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
        }
    }

    public override void TakeHit(int damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= _health)
        {
#pragma warning disable CS0618 // Typ lub sk³adowa jest przestarza³a
            Destroy(Instantiate(_enemyHitEffect, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as ParticleSystem, _enemyHitEffect.startLifetime);
#pragma warning restore CS0618 // Typ lub sk³adowa jest przestarza³a
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    private void FixedUpdate()
    {
        if(_hasTarget == true && Time.time > _nextAttackTime)
        {
            float sqrDistanceToTarget = (_target.position - transform.position).sqrMagnitude;
            if(sqrDistanceToTarget < Mathf.Pow (_attackDistanceThreshold + _enemyCollsionRadius + _targetCollsionRadius,2))
            {
                _nextAttackTime = Time.time + _timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        _currentState = State.Attacking;
        _nav.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 targetDirection = (_target.position - transform.position).normalized;
        Vector3 attackPosition = _target.position - targetDirection * (_enemyCollsionRadius + _targetCollsionRadius);

        float percent = 0;
        float _attackSpeed = 3;

        _skinMaterial.color = Color.cyan;

        bool hasAppliedDamage = false;

        while (percent<=1)
        {
            if(percent >=.5f && hasAppliedDamage == false)
            {
                hasAppliedDamage = true;
                _targetHealth.TakeDamage(_damageRate);
            }
            percent += Time.deltaTime * _attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            yield return null;
        }

        _skinMaterial.color = _originalColor;
        _currentState = State.Chasing;
        _nav.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.1f;
        while (_hasTarget == true)
        {
            if (_currentState == State.Chasing)
            {
                Vector3 targetDirection = (_target.position- transform.position).normalized;
                Vector3 targetPosition = _target.position - targetDirection * (_enemyCollsionRadius + _targetCollsionRadius+ _attackDistanceThreshold/2);
                if (!_dead)
                {
                    _nav.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer, int enemyHealth, Color skinColour)
    {
        _nav.speed = moveSpeed;

        if (_hasTarget)
        {
            _damageRate = Mathf.CeilToInt(_targetHealth._startingHealth / hitsToKillPlayer);
        }
        _startingHealth = enemyHealth;

        _skinMaterial = GetComponent<Renderer>().sharedMaterial;
        _skinMaterial.color = skinColour;
        _originalColor = _skinMaterial.color;
    }

    private void OnTargetDeath()
    {
        _hasTarget = false;
        _currentState = State.Idle;
    }
}

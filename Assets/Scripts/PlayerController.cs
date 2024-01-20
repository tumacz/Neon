using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : HealthComponent
{
    private CharacterController _controller;
    private WeaponController _weaponController;
    private PlayerInput _playerInput;
    private Spawner _spawner;//do ukrytego teleportu

    [SerializeField] private PlayerSO _playerSO;
    [SerializeField] private float _playerSpeed = 6.0f;

    private Transform _actualPos => transform;
    private Vector3 _aimPos;
    private Vector3 _move;
    private Vector3 _direction;

    private InputAction _moveAction;
    private InputAction _shootAction;

    private void Awake()
    {
        GetReferences();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        CalcMovePlayer();
        CalcRotatePlayer(_weaponController._currentWeapon);
        UseWeapon(_weaponController._currentWeapon);
    }

    private void FixedUpdate()
    {
        ExecuteMovement();
        //Teleport();
    }

    private void ExecuteMovement()
    {
        _controller.Move(_move * Time.deltaTime * _playerSpeed);
        //redo
        if (_direction == new Vector3(0, 0, 0))
            return;
        else
            transform.forward = new Vector3(_direction.x, 0, _direction.z);
    }

    private void CalcRotatePlayer(Weapon weapon)
    {
        _aimPos = _playerSO.aimPos;
        _aimPos.y = weapon._shootPoint.transform.position.y;
        _direction = (_aimPos - _actualPos.position).normalized;
    }

    private void CalcMovePlayer()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        _move = new Vector3(input.x, 0f, input.y).normalized;
    }

    private void UseWeapon(Weapon weapon)
    {
        if (((new Vector2(_aimPos.x, _aimPos.z) - new Vector2(weapon.transform.position.x, weapon.transform.position.z)).magnitude) > 1)
        {
            weapon.AimWeapon(_aimPos);
        }

        if (_shootAction.IsPressed())
        {
            weapon.OnTriggerHold();
        }
        else
        {
            StopWeapon(weapon);
        }
    }

    private void StopWeapon(Weapon weapon)//czy to potrzebne?
    {
        weapon.OnTriggerRelease();
    }

    private void GetReferences()
    {
        _controller = GetComponent<CharacterController>();
        _weaponController = GetComponent<WeaponController>();
        _playerInput = GetComponent<PlayerInput>();
        _spawner = FindObjectOfType<Spawner>();
        //catche particular actions
        _moveAction = _playerInput.actions["Move"];
        _shootAction = _playerInput.actions["Shoot"];
    }

    //private void Teleport()
    //{
    //    if(Input.GetKeyDown(KeyCode.Q))
    //    {
    //        _spawner.ResetPlayerPosition();
    //        _playerSO.Test();
    //    }
    //}
}
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : HealthComponent
{
    private CharacterController _controller;
    private WeaponController _weaponController;
    private PlayerInput _playerInput;

    [SerializeField] private PlayerSO _playerSO;
    [SerializeField] private float playerSpeed = 6.0f;

    private Transform _actualPos;
    private Vector3 _aimPos;
    private Vector3 _move;
    private Vector3 _direction;

    private InputAction _moveAction;
    private InputAction _shootAction;
    //private InputAction _lookAction;


    private void Awake()
    {
        GetReferences();
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        _shootAction.performed += _ => UseWeapon();
    }

    private void OnDisable()
    {
        _shootAction.performed -= _ => UseWeapon();
    }

    void Update()
    {
        CalcMovePlayer();
        CalcRotatePlayer();
        UseWeapon();
    }

    private void FixedUpdate()
    {
        ExecuteMovement();
    }

    private void ExecuteMovement()
    {
        _controller.Move(_move * Time.deltaTime * playerSpeed);
        transform.forward = _direction;
    }

    private void CalcRotatePlayer()
    {
        _actualPos = this.transform;
        Vector3 Position = new Vector3(_actualPos.transform.position.x, 0, _actualPos.transform.position.z);
        _aimPos = _playerSO.aimPos;
        _direction = new Vector3(_aimPos.x - Position.x,0 , _aimPos.z - Position.z);
    }

    private void CalcMovePlayer()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        _move = new Vector3(input.x, 0, input.y);
    }

    private void UseWeapon()
    {
        if (_shootAction.IsPressed())
        {
            _weaponController.WeaponShoot();
        }
    }

    private void GetReferences()
    {
        _controller = GetComponent<CharacterController>();
        _weaponController = GetComponent<WeaponController>();
        _playerInput = GetComponent<PlayerInput>();

        //catche particular actions
        _moveAction = _playerInput.actions["Move"];
        //_lookAction = _playerInput.actions["Look"];
        _shootAction = _playerInput.actions["Shoot"];
    }
}

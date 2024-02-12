using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : HealthComponent
{
    private CharacterController _controller;
    private WeaponController _weaponController;
    private Spawner _spawner;
    private PlayerControls _input;

    public event Action OnStartShoot;
    public event Action OnStopShoot;
    //public event Action OnTeleport;

    [SerializeField] private PlayerSO _playerSO;
    [SerializeField] private float _playerSpeed = 6.0f;

    private Transform _actualPos => transform;
    private bool _isTeleporting = false;
    private Vector3 _aimPos;
    private Vector3 _move;
    private Vector3 _direction;

    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _reloadAction;
    private InputAction _teleportAction;

    private void Awake()
    {
        GetReferences();
    }

    private void OnEnable()
    {
        _input.Enable();
        _reloadAction.performed += ctx => Reload(ctx);
        _teleportAction.performed += ctx => Teleport(ctx);
    }

    private void OnDisable()
    {
        _input.Disable(); 
        _reloadAction.canceled -= ctx => Reload(ctx);
        _teleportAction.canceled += ctx => Teleport(ctx);
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
    }

    private void ExecuteMovement()
    {
        _controller.Move(_move * Time.deltaTime * _playerSpeed);

        if (_direction != Vector3.zero)
        {
            transform.forward = new Vector3(_direction.x, 0, _direction.z);
        }
    }

    private void CalcRotatePlayer(WeaponBase weapon)
    {
        _aimPos = _playerSO.aimPos;
        if (weapon == null)
        {
            _aimPos.y = 0;
        }
        else
            _aimPos.y = (weapon is FireArms) ? ((FireArms)weapon)._shootPoint.position.y : ((MeleWeapon)weapon)._shootPoint.position.y;

        _direction = (_aimPos - _actualPos.position).normalized;
    }

    private void CalcMovePlayer()
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        _move = new Vector3(input.x, 0f, input.y).normalized;
    }

    private void UseWeapon(WeaponBase weapon)
    {
        if (weapon == null)
            return;

        if (((new Vector2(_aimPos.x, _aimPos.z) - new Vector2(weapon.transform.position.x, weapon.transform.position.z)).magnitude) > 1)
        {
            weapon.AimWeapon(_aimPos);
        }

        if (_shootAction.IsPressed())
        {
            OnStartShoot?.Invoke();
        }
        else
        {
            OnStopShoot?.Invoke();
        }
    }

    private void GetReferences()
    {
        _input = new PlayerControls();
        _controller = GetComponent<CharacterController>();
        _weaponController = GetComponent<WeaponController>();
        _spawner = FindObjectOfType<Spawner>();

        _moveAction = _input.Player.Move;
        _shootAction = _input.Player.Shoot;
        _reloadAction = _input.Player.Reload;
        _teleportAction = _input.Player.Teleport;
        
        OnStartShoot += () => _weaponController.OnTriggerHold(_weaponController._currentWeapon);
        OnStopShoot += () => _weaponController.OnTriggerRelease(_weaponController._currentWeapon);
    }

    private void Reload(InputAction.CallbackContext context)
    {
        _weaponController._currentWeapon.Reload();
    }

    private void Teleport(InputAction.CallbackContext context)
    {
        if (!_isTeleporting)
        {
            _isTeleporting = true;
            StartCoroutine(PerformTeleport());
        }
    }

    public void Respawn()
    {
        if (!_isTeleporting)
        {
            _isTeleporting = true;
            StartCoroutine(PerformTeleport());
        }
    }

    private IEnumerator PerformTeleport()
    {
        _controller.enabled = false;
        _spawner.ResetPlayerPosition();
        yield return null;

        _controller.enabled = true;
        _isTeleporting = false;
    }

    private void OnDestroy()
    {
        OnStartShoot -= () => _weaponController.OnTriggerHold(_weaponController._currentWeapon);
        OnStopShoot -= () => _weaponController.OnTriggerRelease(_weaponController._currentWeapon);
        //OnTeleport -= () => _spawner.ResetPlayerPosition();
    }
}

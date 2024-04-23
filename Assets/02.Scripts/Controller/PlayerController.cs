using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : UnitBase, ITakeDamage
{
    #region const
    private const float LIMIT_ROTATE_UP = -40f;
    private const float LIMIT_ROTATE_DOWN = 20f;
    private const float CONTINUE_KILL_TIME = 3f;
    private const float INVINCIBILITY_TIME = 1f;
    #endregion

    #region private variable
    private float _gravity = -9.81f;
    private int _jumpLayer;
    private float _vx;
    private float _vy;
    private float _moveX;
    private float _moveZ;
    private float _doubleKillCheck;
    private float _tripleKillCheck;
    private Vector3 _velocity;
    private float _jumpTimer;
    [SerializeField] private bool _isRun;
    [SerializeField] private float _runSpeed;
    [SerializeField] private bool _isJumping;
    [SerializeField] private bool _isFalling;
    [SerializeField] private Camera _subCamera;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rotateSpeed = 250f;
    [SerializeField] private float _jumpValue = 1.5f;
    [SerializeField] private float _boundTime = 0.1f;
    private CharacterController _cc;

    #endregion

    #region Event
    public Action ShotEvent;
    public Action<float> CrossValueEvent;
    public Action<int, int, int> HpEvent;
    public Action<int, int, int> BulletEvent;
    public Action<Player.WeaponController> ChangeEvent;
    public Action<Transform, Transform> HurtEvent;
    public Action DeadEvent;
    public Action KillEvent;
    public Action RespawnEvent;
    public Action BodyshotEvent;
    public Action HeadshotEvent;
    public Action DoubleKillEvent;
    public Action TripleKillEvent;
    public Action<bool> ScoreboardEvent;
    public Action MenuEvent;
    public Action SettingEvent;
    public Action<bool> CollideItemEvent;
    public Action<bool> AimEvent;
    #endregion

    #region property
    public bool IsAiming { get; private set; }
    public bool IsKill { get; set; }
    public bool IsDoubleKill { get; set; }
    public bool IstripleKill { get; set; }
    public float Sensitiv { get; set; } = 50f;
    public Player.WeaponController CurrentWeapon => _currentWeapon as Player.WeaponController;
    #endregion

    #region Init
    protected override void Awake() {
        base.Awake();
        _cc = GetComponent<CharacterController>();
    }

    private void Start() {
        CrossValueEvent.Invoke(CurrentWeapon.CrossValue);
        _jumpLayer = (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground);
    }

    #endregion

    #region Sfx
    public void JumpSfx() {
        int ran = UnityEngine.Random.Range((int)UnitSfx.Jump1, (int)UnitSfx.Jump3);
        _ufx.PlaySfx((UnitSfx)ran);
    }
    #endregion

    #region Behaviour
    public void Shot() {
        if (!Managers.GameManager.InGame())
            return;

        if (_currentWeapon.TryShot(this)) {
            ChangeState(UnitState.Shot);
        }
    }

    private void Jump() {
        if (!Managers.GameManager.InGame())
            return;

        if (!IsGround()) {
            return;
        }

        if(_isJumping) {
            return;
        }

        JumpSfx();
        _isJumping = true;
        _isFalling = false;
        _velocity.y = Mathf.Sqrt(_jumpValue * -2f * _gravity);
    }

    #endregion

    #region Change


    public override void ChangeWeapon(WeaponType type) {
        base.ChangeWeapon(type);
        CrossValueEvent.Invoke(CurrentWeapon.CrossValue);
        ChangeEvent.Invoke(CurrentWeapon);
    }
    #endregion

    #region Update
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(Managers.GameManager.State == GameState.StartFight ||
                Managers.GameManager.State == GameState.Menu)
                MenuEvent.Invoke();

            if(Managers.GameManager.State == GameState.Setting)
                SettingEvent.Invoke();
        }

        if (!Managers.GameManager.InGame())
            return;

        if (Input.GetKey(KeyCode.Tab)) {
            ScoreboardEvent.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            ScoreboardEvent.Invoke(false);
        }

        if(Input.GetKey(KeyCode.LeftShift) &&
            !_isRun && _state != UnitState.Jump && _state != UnitState.Shot
             && _state != UnitState.Get && _state != UnitState.Dead
             && _state != UnitState.Reload) {
            _isRun = true;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift) &&
            _isRun) {
            ChangeState(UnitState.Move);
            _isRun = false;
        }


        if (_state == UnitState.Dead)
            return;

        if (IsKill) {
            _doubleKillCheck += Time.deltaTime;
            if(_doubleKillCheck > CONTINUE_KILL_TIME) {
                IsKill = false;
                _doubleKillCheck = 0f;
            }
        }

        if (IsDoubleKill)
        {
            _tripleKillCheck += Time.deltaTime;
            if (_tripleKillCheck > CONTINUE_KILL_TIME) {
                IsDoubleKill = false;
                _tripleKillCheck = 0f;
            }
        }

        if(IstripleKill) {
            IsKill = false;
            _doubleKillCheck = 0f;
            IsDoubleKill = false;
            _tripleKillCheck = 0f;
            IstripleKill = false;
        }

        if(_isJumping) {
            _jumpTimer += Time.deltaTime;
        }

        if(_isFalling) {
        }

        if(IsGround() && _isJumping && _isFalling) 
        {
            _isFalling = false;
        }

        if(IsGround() && _isJumping && _jumpTimer > .6f) {
            _isJumping = false;
            _jumpTimer = 0f;
        }

        if(!IsGround() && !_isJumping && !_isFalling) {
            _velocity.y = 0f;
            _isFalling = true;
        }

        if(IsGround()) {
            _isFalling = false;
        }

        OnRotateUpdate();
        PlayerPhycisc();
        CheckForward();

        if (Input.GetMouseButtonDown(0) 
            || Input.GetMouseButton(0)) {
            Shot();
        }

        if(Input.GetMouseButton(1)) {
            if(!IsAiming) {
                IsAiming = true;
                AimEvent.Invoke(true);
            }
        }

        if(IsAiming && Input.GetMouseButtonUp(1)) {
            IsAiming = false;
            AimEvent.Invoke(false);
        }

        if (_state == UnitState.Shot) {
            if (Input.GetMouseButtonUp(0)) {
                if(BaseWeapon.Type == WeaponType.Rifle)
                    BaseWeapon.Delay = 1f;
                _state = UnitState.Idle;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }

        if(Input.GetKeyDown(KeyCode.F)) {
            GetItem();
        }

        switch (_state) {
            case UnitState.Idle:
                OnIdleUpdate();
                break;
        }
    }

    private void OnIdleUpdate() {
        if(_moveX != 0 || _moveZ != 0) {
            _state = UnitState.Move;
            return;
        }
    }

    private void PlayerPhycisc() {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveZ = Input.GetAxisRaw("Vertical");

        if(_state != UnitState.Reload && _state != UnitState.Shot) {
            if (_moveX == 0 && _moveZ == 0) {
                ChangeState(UnitState.Idle);
            } else {
                if(_isRun)
                    ChangeState(UnitState.Run);
                else
                    ChangeState(UnitState.Move);
            }
        }
        
        
        Vector3 move = transform.right * _moveX + transform.forward * _moveZ;
        move.y = 0f;
        float speed = _isRun ? _moveSpeed + _runSpeed : _moveSpeed;
        _cc.Move(move.normalized * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        _velocity.y += _gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    private void OnRotateUpdate() {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");
        Vector3 dir = new Vector3(MouseY, MouseX, 0);

        _vx += dir.x * Sensitiv * 5f * Time.deltaTime;
        _vy += dir.y * Sensitiv * 5f * Time.deltaTime;
        _vx = Mathf.Clamp(_vx, LIMIT_ROTATE_UP, LIMIT_ROTATE_DOWN);

        Vector3 lastDir = new Vector3(-_vx, _vy, 0);
        transform.eulerAngles = lastDir;
        UnitRotate = transform.rotation;
    }
    #endregion

    #region Bound
    public IEnumerator COBound(float verticla, float horizontal) {
        float exitTime = 0;
        float horizontalRecoil = UnityEngine.Random.Range(-0.2f, 0.2f); 

        while (true) {
            exitTime += Time.deltaTime;
            _vx += exitTime * verticla;
            _vy += horizontalRecoil * horizontal; 

            if (exitTime > _boundTime) {
                StartCoroutine(CORebound());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CORebound() {
        float exitTime = 0;
        float horizontalRecoil = UnityEngine.Random.Range(-0.2f, 0.2f); 

        while (true) {
            exitTime += Time.deltaTime;
            _vx -= exitTime * CurrentWeapon.VerticalBoundValue; 
            _vy -= horizontalRecoil * CurrentWeapon.HorizontalBoundValue;

            if (exitTime > _boundTime * .5f)
                break;
            yield return null;
        }
    }
    #endregion

    #region OtherEvent
    protected override void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans) {
        base.IsHitEvent(damage, attackerTrans, myTrans);
        HpEvent.Invoke(_currentHp, _maxHp, damage);
        HurtEvent.Invoke(attackerTrans, myTrans);
    }

    public override int SetHp(int damage) {
        base.SetHp(damage);
        HpEvent.Invoke(_currentHp, _maxHp, damage);
        return _currentHp;
    }

    protected override void IsDeadEvent(Transform attackerTrans) {
        _moveX = 0f;
        _moveZ = 0f;
        base.IsDeadEvent(attackerTrans);
        CurrentWeapon.ChangeAimAC(false);
        _cc.enabled = false;
        DeadEvent.Invoke();
        _mainCamera.gameObject.SetActive(false);
        _subCamera.gameObject.SetActive(true);
    }


    private void CheckForward() {
        if (_state == UnitState.Dead)
            return;

        int layer = (1 << (int)LayerType.Item);
        Debug.DrawRay(_firePoint.position, _firePoint.forward * 3f, Color.green);

        var col = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, 2f,  layer);
        if (!col) {
            CollideItemEvent.Invoke(false);
            CollideItem = null;
            return;
        }

        CollideItem = hit.collider.GetComponent<IItem>();

        if(CollideItem != null) {
            CollideItemEvent.Invoke(true);
        }
    }

    private bool IsGround() {
        Debug.DrawRay(transform.position + Vector3.up, -Vector3.up * 1.3f, Color.red);
        return Physics.Raycast(transform.position + Vector3.up, -Vector3.up, 1.3f, _jumpLayer);
    }

    public override void Init() {
        base.Init();

        _isFalling = false;
        _isJumping = false;
        IsAiming = false;
        AimEvent.Invoke(false);
        CurrentWeapon.ChangeAimAC(false);
        _vx = _vy = _moveX = _moveZ = 0f;
        _velocity = Vector3.zero;
        UnitRotate = Quaternion.identity;
        _mainCamera.gameObject.SetActive(true);
        _subCamera.gameObject.SetActive(false);
        _cc.enabled = true;
        RespawnEvent.Invoke();
        ChangeState(UnitState.Idle);
        Invoke("InvincibilityEnd", INVINCIBILITY_TIME);
    }

    private void InvincibilityEnd() {
        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
    }

    public override void ChangeState(UnitState state) {
        switch (state) {
            case UnitState.Idle:
                if (_state == UnitState.Dead || _state == UnitState.Shot)
                    return;

                _isRun = false;
                Model.Animator.SetBool("Move", false);
                CurrentWeapon.CurrentAnime.SetBool("Move", false);
                CurrentWeapon.CurrentAnime.SetBool("Run", false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                _isRun = false;
                Model.Animator.SetBool("Move", true);
                CurrentWeapon.CurrentAnime.SetBool("Run", false);
                CurrentWeapon.CurrentAnime.SetBool("Move", true);
                break;

            case UnitState.Run:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                CurrentWeapon.CurrentAnime.SetBool("Move", false);
                CurrentWeapon.CurrentAnime.SetBool("Run", true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                _isRun = false;
                Model.Animator.SetBool("Move", false);
                CurrentWeapon.CurrentAnime.SetBool("Move", false);
                CurrentWeapon.CurrentAnime.SetBool("Run", false);

                Model.Animator.SetTrigger($"Shot{BaseWeapon.Type.ToString()}");
                CurrentWeapon.CurrentAnime.SetTrigger("Shot");

                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Run", false);
                Model.Animator.SetTrigger("Reload");
                BaseWeapon.Animator.SetTrigger("Reload");
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Run", false);
                Model.Animator.SetTrigger("Dead");
                BaseWeapon.Animator.SetTrigger("Dead");
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Run", false);
                Model.Animator.SetTrigger("Get");
                BaseWeapon.Animator.SetTrigger("Get");
                break;
        }
        _state = state;

    }
    #endregion
}

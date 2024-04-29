using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using static Define;

public class PlayerController : UnitBase, ITakeDamage
{

    #region private variable
    private int _jumpLayer;
    private float _vx;
    private float _vy;
    private float _moveX;
    private float _moveZ;
    private float _doubleKillCheck;
    private float _tripleKillCheck;
    private Vector3 _velocity;
    private float _jumpTimer;
    private bool _isRun;
    private float _runSpeed = PLAYER_DEFAULT_RUN_SPEED;
    private bool _isJumping;
    private bool _isFalling;
    [SerializeField] private Camera _subCamera;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _handCamera;
    private float _rotateSpeed = PLAYER_DEFAULT_ROTATE_SPEED;
    private float _jumpValue = PLAYER_DEFAULT_JUMP_VALUE;
    private float _boundTime = PLAYER_DEFAULT_BOUND_TIME;
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
    public Action<int> KillEvent;
    public Action RespawnEvent;
    public Action<bool> HurtShotEvent;
    public Action<bool> ShowScoreboardEvent;
    public Action ShowMenuEvent;
    public Action ShowSettingEvent;
    public Action<bool> CollideItemEvent;
    public Action<bool> SetAimEvent;
    public Action<int> ChangeCrosshairEvent;
    public Action<DirType, string, bool, bool> ShowKillAndDeadTextEvent;
    #endregion

    #region property
    public bool IsAiming { get; private set; }
    public bool IsKill { get; set; }
    public bool IsDoubleKill { get; set; }
    public bool IstripleKill { get; set; }
    public float Sensitivy { get; set; } = .5f;
    public Player.WeaponController CurrentWeapon => _currentWeapon as Player.WeaponController;
    #endregion

    #region Init
    protected override void Awake() {
        base.Awake();
        
        _cc = GetComponent<CharacterController>();
    }

    private void Start() {
        CrossValueEvent?.Invoke(CurrentWeapon.CrossValue);
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
        _velocity.y = Mathf.Sqrt(_jumpValue * -2f * PLAYER_GRAVITY);
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
                ShowMenuEvent.Invoke();

            if(Managers.GameManager.State == GameState.Setting)
                ShowSettingEvent.Invoke();
        }

        if (!Managers.GameManager.InGame())
            return;

        if (Input.GetKey(KeyCode.Tab)) {
            ShowScoreboardEvent.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            ShowScoreboardEvent.Invoke(false);
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

        if (IsKill) {
            _doubleKillCheck += Time.deltaTime;
            if (_doubleKillCheck > PLAYER_CONTINUE_KILL_TIME) {
                IsKill = false;
                _doubleKillCheck = 0f;
            }
        }

        if (IsDoubleKill) {
            _tripleKillCheck += Time.deltaTime;
            if (_tripleKillCheck > PLAYER_CONTINUE_KILL_TIME) {
                IsDoubleKill = false;
                _tripleKillCheck = 0f;
            }
        }

        if (IstripleKill) {
            IsKill = false;
            _doubleKillCheck = 0f;
            IsDoubleKill = false;
            _tripleKillCheck = 0f;
            IstripleKill = false;
        }

        if (_state == UnitState.Dead)
            return;



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
                SetAimEvent.Invoke(true);
            }
        }

        if(IsAiming && Input.GetMouseButtonUp(1)) {
            IsAiming = false;
            SetAimEvent.Invoke(false);
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

        _velocity.y += PLAYER_GRAVITY * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    private void OnRotateUpdate() {
        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");
        Vector3 dir = new Vector3(MouseY, MouseX, 0);

        _vx += dir.x * Sensitivy * _rotateSpeed * Time.deltaTime;
        _vy += dir.y * Sensitivy * _rotateSpeed * Time.deltaTime;
        _vx = Mathf.Clamp(_vx, PLAYER_LIMIT_ROTATE_UP, PLAYER_LIMIT_ROTATE_DOWN);

        Vector3 lastDir = new Vector3(-_vx, _vy, 0);
        transform.eulerAngles = lastDir;
    }
    #endregion

    #region Bound
    public IEnumerator COBound(float verticla, float horizontal) {


        float exitTime = 0;
        float horizontalRecoil = UnityEngine.Random.Range(-PLAYER_SHOT_RANDOMBOUND_VALUE, PLAYER_SHOT_RANDOMBOUND_VALUE); 

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
        float horizontalRecoil = UnityEngine.Random.Range(-PLAYER_SHOT_RANDOMBOUND_VALUE, PLAYER_SHOT_RANDOMBOUND_VALUE); 

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

    public void HurtShot(bool headShot) {
        HurtShotEvent.Invoke(headShot);
    }

    public void SetReload(int currentBullet, int maxBullet, int remainBullet) {
        BulletEvent.Invoke(currentBullet, maxBullet, remainBullet);
    }

    public void SetShot(float verticla, float horizontal, int currentBullet, int maxBullet, int remainBullet) {
        StartCoroutine(COBound(verticla, horizontal));
        BulletEvent.Invoke(currentBullet, maxBullet, remainBullet);
    }

    protected override void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans) {
        HpEvent?.Invoke(_currentHp, _maxHp, damage);
        HurtEvent?.Invoke(attackerTrans, myTrans);
        PersonalSfxController.instance.SetShareSfx(ShareSfx.Hurt);
    }

    public override int SetHp(int damage) {
        base.SetHp(damage);
        HpEvent?.Invoke(_currentHp, _maxHp, damage);
        return _currentHp;
    }   

    protected override void IsDeadEvent(Transform attackerTrans, bool headShot) {
        _moveX = 0f;
        _moveZ = 0f;
        _cc.enabled = false;
        CurrentWeapon.ChangeAimAC(false);
        DirType dir = Util.DirectionCalculation(attackerTrans, transform);
        ShowKillAndDeadTextEvent?.Invoke(dir, attackerTrans.name, false, headShot);
        _mainCamera.gameObject.SetActive(false);
        _handCamera.gameObject.SetActive(false);
        _subCamera.gameObject.SetActive(true);
        base.IsDeadEvent(attackerTrans, headShot);
        DeadEvent?.Invoke();
    }


    private void CheckForward() {
        if (_state == UnitState.Dead)
            return;

        int layer = (1 << (int)LayerType.Item);

        var col = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, PLAYER_FORWARDCHECK_LENTHS,  layer);
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
        return Physics.Raycast(transform.position + Vector3.up, -Vector3.up, PLAYER_GROUNDCHECK_LENTHS, _jumpLayer);
    }

    public override void Respawn() {
       
        base.Respawn();
        _isFalling = false;
        _isJumping = false;
        IsAiming = false;
        SetAimEvent.Invoke(false);
        CurrentWeapon.ChangeAimAC(false);
        _vx = _vy = _moveX = _moveZ = 0f;
        _velocity = Vector3.zero;
        _mainCamera.gameObject.SetActive(true);
        _subCamera.gameObject.SetActive(false);
        _handCamera.gameObject.SetActive(true);
        _cc.enabled = true;
        RespawnEvent.Invoke();
        ChangeState(UnitState.Idle);
        Invoke("InvincibilityEnd", PLAYER_INVINCIBILITY_TIME);
    }

    private void InvincibilityEnd() {
        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
    }

    private void SetAnimationBool(string parameter, bool trigger) {
        Model.Animator.SetBool(parameter, trigger);
        CurrentWeapon.CurrentAnime.SetBool(parameter, trigger);
    }

    public override void ChangeState(UnitState state) {
        switch (state) {
            case UnitState.Idle:
                if (_state == UnitState.Dead || _state == UnitState.Shot)
                    return;

                _isRun = false;
                SetAnimationBool(UnitState.Move.ToString(), false);

                CurrentWeapon.CurrentAnime.SetBool(UnitState.Run.ToString(), false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                _isRun = false;
                SetAnimationBool(UnitState.Move.ToString(), true);
                CurrentWeapon.CurrentAnime.SetBool(UnitState.Move.ToString(), true);

                CurrentWeapon.CurrentAnime.SetBool(UnitState.Run.ToString(), false);
                break;

            case UnitState.Run:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                CurrentWeapon.CurrentAnime.SetBool(UnitState.Move.ToString(), false);
                CurrentWeapon.CurrentAnime.SetBool(state.ToString(), true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                _isRun = false;
                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                CurrentWeapon.CurrentAnime.SetBool(UnitState.Move.ToString(), false);
                CurrentWeapon.CurrentAnime.SetBool(UnitState.Run.ToString(), false);

                Model.Animator.SetTrigger($"{state.ToString()}{BaseWeapon.Type.ToString()}");
                CurrentWeapon.CurrentAnime.SetTrigger(state.ToString());

                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Run.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Run.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                _isRun = false;
                CurrentWeapon.ChangeAimAC(false);
                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Run.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
        }
        _state = state;

    }

    public void ContinueKill() {
        if (IstripleKill) {
            return;
        }
        if (!IsKill && !IsDoubleKill) {
            IsKill = true;
            KillEvent?.Invoke(1);
            return;
        }
        if (IsKill && !IsDoubleKill) {
            IsDoubleKill = true;
            IsKill = false;
            PersonalSfxController.instance.SetShareSfx(ShareSfx.Dominate);
            KillEvent?.Invoke(2);
            return;
        }
        if (!IsKill && IsDoubleKill && !IstripleKill) {
            IsDoubleKill = false;
            IstripleKill = true;
            PersonalSfxController.instance.SetShareSfx(ShareSfx.Rampage);
            KillEvent?.Invoke(3);
        }
    }

    public void ShowKillAndDeadText(DirType dir, string name, bool kill, bool headShot) {
        ShowKillAndDeadTextEvent.Invoke(dir, name, true, headShot);
    }
    #endregion
}

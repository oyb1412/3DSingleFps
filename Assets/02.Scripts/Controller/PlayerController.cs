using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class PlayerController : UnitBase, ITakeDamage
{
    private const float LIMIT_ROTATE_UP = -20f;
    private const float LIMIT_ROTATE_DOWN = 10f;
    private const float CONTINUE_KILL_TIME = 3f;
    private float _gravity = -9.81f;

    private int _jumpLayer;
    private float _vx;
    private float _vy;
    private float _moveX;
    private float _moveZ;
    private Vector3 _velocity;

    public Action ShotEvent;
    public Action<float> CrossValueEvent;

    public Action<int, int> HpEvent;
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

    private CharacterController _cc;

    public bool IsKill { get; set; }
    public bool IsDoubleKill { get; set; }
    public bool IstripleKill { get; set; }

    private float _doubleKillCheck;
    private float _tripleKillCheck;

    public PlayerStatus MyStatus { get { return _status as PlayerStatus; } set { _status = value; } }
    public Player.WeaponController CurrentWeapon => _currentWeapon as Player.WeaponController;

    [SerializeField] private Camera _subCamera;
    [SerializeField] private Camera _mainCamera;
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

    #region Behaviour
    public void Shot() {
        if (!Managers.GameManager.InGame())
            return;

        if (_currentWeapon.TryShot(this)) {
            ChangeShotState(UnitState.Shot);
        }
    }

    public void Jump() {
        if (!Managers.GameManager.InGame())
            return;

        if (!IsGround())
            return;

        _velocity.y = Mathf.Sqrt(MyStatus._jumpValue * -2f * _gravity);
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
        if (!Managers.GameManager.InGame())
            return;

        if (Input.GetKey(KeyCode.Tab)) {
            ScoreboardEvent.Invoke(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            ScoreboardEvent.Invoke(false);
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

        OnRotateUpdate();
        PlayerPhycisc();
        CheckForward();
       

        if (Input.GetMouseButtonDown(0) 
            || Input.GetMouseButton(0)) {
            Shot();
        }

        if(_state == UnitState.Shot) {
            if (Input.GetMouseButtonUp(0)) {
                _state = UnitState.Idle;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
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

        if(_state != UnitState.Shot && _state != UnitState.Reload
            && _state != UnitState.Dead) {
            if (_moveX == 0 && _moveZ == 0) {
                _state = UnitState.Idle;
                ChangeState(UnitState.Move, false);
            } else {
                _state = UnitState.Move;
                ChangeState(UnitState.Move, true);
            }
        }
        
        Vector3 move = transform.right * _moveX + transform.forward * _moveZ;
        _cc.Move(move * _status._moveSpeed * Time.deltaTime);

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

        _vx += dir.x * MyStatus._rotateSpeed * Time.deltaTime;
        _vy += dir.y * MyStatus._rotateSpeed * Time.deltaTime;
        _vx = Mathf.Clamp(_vx, LIMIT_ROTATE_UP, LIMIT_ROTATE_DOWN);

        Vector3 lastDir = new Vector3(-_vx, _vy, 0);
        transform.eulerAngles = lastDir;
        UnitRotate = transform.rotation;
    }
    #endregion

    #region Bound
    public IEnumerator COBound() {
        float exitTime = 0;
        float horizontalRecoil = UnityEngine.Random.Range(-0.2f, 0.2f); 

        while (true) {
            exitTime += Time.deltaTime;
            _vx += exitTime * CurrentWeapon.VerticalBoundValue;
            _vy += horizontalRecoil * CurrentWeapon.HorizontalBoundValue; 

            if (exitTime > MyStatus._boundTime) {
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

            if (exitTime > MyStatus._boundTime * .5f)
                break;
            yield return null;
        }
    }
    #endregion

    #region OtherEvent

    protected override void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans) {
        base.IsHitEvent(damage, attackerTrans, myTrans);
        HpEvent.Invoke(_status._currentHp, _status._maxHp);
        HurtEvent.Invoke(attackerTrans, myTrans);
    }

    protected override void IsDeadEvent(Transform attackerTrans) {
        _moveX = 0f;
        _moveZ = 0f;
        base.IsDeadEvent(attackerTrans);
        _cc.enabled = false;
        DeadEvent.Invoke();
        _mainCamera.gameObject.SetActive(false);
        _subCamera.gameObject.SetActive(true);
    }

    private void CheckForward() {

        int layer = (1 << (int)LayerType.Item);
        Debug.DrawRay(_firePoint.position, _firePoint.forward * 3f, Color.green);

        var col = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, 2f,  layer);
        if (!col) {
            CollideItem = null;
            return;
        }

        CollideItem = hit.collider.GetComponent<IItem>();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up, -Vector3.up * 1.5f);
    }

    private bool IsGround() {
        return Physics.Raycast(transform.position + Vector3.up, -Vector3.up, 1.5f, _jumpLayer);
    }

    public override void Init() {
        base.Init();
        _vx = _vy = _moveX = _moveZ = 0f;
        _velocity = Vector3.zero;
        UnitRotate = Quaternion.identity;
        _mainCamera.gameObject.SetActive(true);
        _subCamera.gameObject.SetActive(false);
        _cc.enabled = true;
        RespawnEvent.Invoke();
    }
    #endregion
}

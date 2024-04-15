using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    private const float LIMIT_ROTATE_UP = -20f;
    private const float LIMIT_ROTATE_DOWN = 7f;

    [Header("--Status--")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _jumpValue;
    [SerializeField] private float _boundValue;
     private float _boundTime = 0.1f;
    [SerializeField] private float _gravity;

    private bool _isDirty;
    private float _vx;
    private float _vy;
    private float _moveX;
    private float _moveZ;
    private Vector3 _velocity;

    public ModelController Model { get; set; }
    private CharacterController _cc;
    private GameObject _weapons;
    private Transform _firePoint;

    private IItem _collideItem;
    private IWeapon _currentWeapon;
    private IWeapon[] _weaponList = new IWeapon[(int)WeaponType.Count];
    [SerializeField]private PlayerState _state = PlayerState.Idle;
    public PlayerState State => _state;
    private void Awake() {
        _cc = GetComponent<CharacterController>();
        Model = GetComponentInChildren<ModelController>();
    }

    private void Start() {
        Init();
    }

    private void Init() {
        _weapons = Util.FindChild(gameObject, "Weapons", false);
        _firePoint = Util.FindChild(gameObject, "FirePoint", true).transform;
        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false).GetComponent<IWeapon>();

        _currentWeapon = _weaponList[(int)WeaponType.Pistol];
        _currentWeapon.Activation(_firePoint, this);
        foreach (var weapon in _weaponList) {
            if (_currentWeapon != weapon)
                weapon.myObject.SetActive(false);
        }
        _boundValue = _currentWeapon.BoundValue;

    }

    #region Behaviour
    public void Shot() {
        if (_currentWeapon.TryShot(this)) {
            ChangeState(PlayerState.Shot);
            StartCoroutine(COBound());
        }
    }

    public void Reload() {
        if (_currentWeapon.TryReload(this))
            ChangeState(PlayerState.Reload);
    }

    public void Jump() {
        if (!IsGround())
            return;

        _velocity.y = Mathf.Sqrt(_jumpValue * -2f * _gravity);
        Debug.Log("점프 발동");
    }

    #endregion

    #region Change
    public void ChangeState(PlayerState state, bool trigger) {
        _state = state;
        if(state == PlayerState.Move)
            _currentWeapon.SetAnimation(state, trigger);
    }

    public void ChangeState(PlayerState state) {
        _state = state;
        if (state == PlayerState.Reload || state == PlayerState.Shot
            || state == PlayerState.Get)
            _currentWeapon.SetAnimation(state);
    }


    public void ChangeWeapon(WeaponType type) {
        _weaponList[(int)type].myObject.SetActive(true);
        _currentWeapon = _weaponList[(int)type];
        _currentWeapon.Activation(_firePoint, this);
        foreach (var item in _weaponList) {
            if (_currentWeapon != item)
                item.myObject.SetActive(false);
        }
        _collideItem = null;
        _boundValue = _currentWeapon.BoundValue;
        Model.ChangeWeapon(type);
    }

    #endregion

    #region Update
    private void Update() {
        if (_state == PlayerState.Dead)
            return;

        OnRotateUpdate();
        PlayerPhycisc();

        if (Input.GetMouseButtonDown(0) 
            || Input.GetMouseButton(0)) {
            Shot();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }

        if(Input.GetKeyDown(KeyCode.G)) {
            GetItem();
        }

        switch (_state) {
            case PlayerState.Idle:
                OnIdleUpdate();
                break;
        }
    }

    private void GetItem() {
        if (_collideItem == null)
            return;

        _collideItem.Pickup(this);
    }

    private void OnIdleUpdate() {
        if(_moveX != 0 || _moveZ != 0) {
            _state = PlayerState.Move;
            return;
        }
    }

    private void PlayerPhycisc() {
        _moveX = Input.GetAxisRaw("Horizontal");
        _moveZ = Input.GetAxisRaw("Vertical");

        if(_state != PlayerState.Shot && _state != PlayerState.Reload) {
            if (_moveX == 0 && _moveZ == 0) {
                _state = PlayerState.Idle;
                _currentWeapon.SetAnimation(PlayerState.Move, false);
            } else {
                _state = PlayerState.Move;
                _currentWeapon.SetAnimation(PlayerState.Move, true);
            }
        }
        
        Vector3 move = transform.right * _moveX + transform.forward * _moveZ;
        _cc.Move(move * _moveSpeed * Time.deltaTime);

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

        _vx += dir.x * _rotateSpeed * Time.deltaTime;
        _vy += dir.y * _rotateSpeed * Time.deltaTime;
        _vx = Mathf.Clamp(_vx, LIMIT_ROTATE_UP, LIMIT_ROTATE_DOWN);

        Vector3 lastDir = new Vector3(-_vx, _vy, 0);
        transform.eulerAngles = lastDir;
    }
    #endregion
    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position + Vector3.up, -Vector3.up * 1.5f);
    }

    private bool IsGround() {
        return Physics.Raycast(transform.position + Vector3.up, -Vector3.up , 1.5f, LayerMask.GetMask("Ground"));
    }

    private IEnumerator COBound() {
        float exitTime = 0;

        while (true) {
            exitTime += Time.deltaTime;
            _vx += exitTime * _boundValue;

            if (exitTime > _boundTime) {
                StartCoroutine(CORebound());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator CORebound() {
        float exitTime = 0;

        while (true) {
            exitTime += Time.deltaTime;
            _vx -= exitTime * _boundValue;

            if (exitTime > _boundTime * .5f)
                break;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider c) {
        if (!c.CompareTag("Item"))
            return;

        if (_collideItem != null)
            return;

        _collideItem = c.GetComponent<IItem>();
    }

    private void OnTriggerExit(Collider c) {
        if (!c.CompareTag("Item"))
            return;

        if (_collideItem == null)
            return;

        if (_collideItem != c.GetComponent<IItem>())
            return;

        _collideItem = null;
    }
}

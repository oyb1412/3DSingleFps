using System;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public abstract class UnitBase : MonoBehaviour
{
    private GameObject _healKit;
    private GameObject _killFeed;
    protected GameObject _weapons;
    protected Transform _firePoint;
    protected int _currentHp = UNIT_DEFAULT_HP;
    protected int _maxHp = UNIT_DEFAULT_HP;
    [SerializeField] protected float _moveSpeed = UNIT_DEFAULT_MOVESPEED;
    [SerializeField] protected Collider _bodyCollider;
    [SerializeField] protected Collider _headCollider;

    protected IWeapon _currentWeapon;
    protected IWeapon[] _weaponList = new IWeapon[(int)WeaponType.Count];
    [SerializeField]protected UnitState _state = UnitState.Idle;
    protected UnitSfxController _ufx;
    private Outline[] _outlines;

    public UnitSfxController Ufx => _ufx;

    public Base.WeaponController BaseWeapon => _currentWeapon as Base.WeaponController;

    public abstract void ChangeState(UnitState state);

    public UnitState State => _state;

    public Action<int> ScoreBoardKillNumberPlusEvent;
    public Action<int> ScoreBoardDeathNumberPlusEvent;

    public Transform FirePoint => _firePoint;

    public int GetCurrentHp => _currentHp;
    public int GetMaxHp => _maxHp;
    public int MyKill { get; private set; }
    public int MyDead { get; private set; }
    public ModelController Model { get; private set; }
    public IItem CollideItem { get; set; }


    protected virtual void Awake() {
        _outlines = new Outline[OUTLINE_NUMBER];
        for(int i = 0; i < _outlines.Length; i++) {
            _outlines[i] = GetComponentsInChildren<Outline>()[i];
        }

        _ufx = GetComponent<UnitSfxController>();
        Model = GetComponentInChildren<ModelController>();

        _weapons = Util.FindChild(gameObject, NAME_WEAPONS, false);
        _firePoint = Util.FindChild(gameObject, NAME_FIREPOINT, true).transform;

        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false).GetComponent<IWeapon>();

        _healKit = (GameObject)Managers.Resources.Load<GameObject>(ITME_HEALKIT_PATH);
        _killFeed = (GameObject)Managers.Resources.Load<GameObject>(UI_KILLFEED_PATH);

        WeaponInit();
    }

    protected void WeaponInit() {
        _currentWeapon = _weaponList[(int)WeaponType.Pistol];
        _currentWeapon.Activation(_firePoint, this);
        _currentWeapon.myObject.SetActive(true);
        foreach (var weapon in _weaponList) {
            if (_currentWeapon != weapon)
                weapon.myObject.SetActive(false);
        }
    }

    public virtual void Reload() {
        if (!Managers.GameManager.InGame())
            return;

        if (_currentWeapon.TryReload(this))
            ChangeState(UnitState.Reload);
    }

    private void DropWeapon() {
        if (_currentWeapon != _weaponList[(int)WeaponType.Pistol]) {
            GameObject weapon = Managers.Resources.Instantiate(BaseWeapon.CreateObject, null);
            weapon.transform.position = transform.position + Vector3.up;
        }
    }

    public virtual void ChangeWeapon(WeaponType type) {
        DropWeapon();
        _weaponList[(int)type].myObject.SetActive(true);
        _currentWeapon = _weaponList[(int)type];
        _currentWeapon.Activation(_firePoint, this);
        ChangeState(UnitState.Get);
        foreach (var item in _weaponList) {
            if (_currentWeapon != item)
                item.myObject.SetActive(false);
        }
        CollideItem = null;
        Model.ChangeWeapon(type);
    }

    public void SetOutlineColor(Color color) {
        foreach(var t in _outlines) {
            t.OutlineColor = color;
        }
    }

    protected void GetItem() {
        if (CollideItem == null)
            return;

        CollideItem.Pickup(this);
    }

    public void TakeDamage(int damage, Transform attackerTrans, Transform myTrans, bool headShot) {
        IsHitEvent(damage, attackerTrans, myTrans);
        int hp = SetHp(-damage);
        if (hp <= 0 && State != UnitState.Dead) {
            IsDeadEvent(attackerTrans, headShot);
        }
    }

    protected abstract void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans);

    private void SetOutline(bool trigger) {
        if(trigger) {
            foreach(var t in _outlines) {
                t.enabled = true;
            }
        }
        else {
            foreach (var t in _outlines) {
                t.enabled = false;
            }
        }
    }

    private void SetScoreBoardKillPluse() {
        MyKill++;
        ScoreBoardKillNumberPlusEvent?.Invoke(MyKill);
    }

    protected virtual void IsDeadEvent(Transform attackerTrans, bool headShot) {
        Invoke("Respawn", Managers.GameManager.RespawnTime);

        SetOutline(false);
        _ufx.PlaySfx(UnitSfx.Dead);
        _bodyCollider.enabled = false;
        _headCollider.enabled = false;
        MyDead++;

        ScoreBoardDeathNumberPlusEvent?.Invoke(MyDead);
        ChangeWeapon(WeaponType.Pistol);
        UnitBase target = attackerTrans.GetComponent<UnitBase>();
        target.SetScoreBoardKillPluse();
        Managers.GameManager.BoardSortToRank();
        Model.ResetAnimator();
        ChangeState(UnitState.Dead);

        GameObject healKit = Managers.Resources.Instantiate(_healKit, null);
        healKit.transform.position = transform.position + Vector3.up;

        UI_KillFeed feed = Managers.Resources.Instantiate(_killFeed, Managers.GameManager.KillFeedParent).GetComponent<UI_KillFeed>();
        feed.Init(BaseWeapon.Type, attackerTrans.name, name, Managers.GameManager.KillFeedParent);
        feed.transform.SetSiblingIndex(0);

        if (Managers.GameManager.KillLimit > 0) {
            if (target.MyKill >= Managers.GameManager.KillLimit) {
                Managers.GameManager.ChangeState(GameState.Gameover);
                return;
            }
        }

        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            player.ContinueKill();
        }
    }

    public virtual int SetHp(int damage) {
        _currentHp += damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, _maxHp);
        return _currentHp;
    }

    public virtual void Respawn() {
        CollideItem = null;
        SetOutline(true);
        WeaponInit();
        _currentHp = _maxHp;
        transform.position = Managers.RespawnManager.GetRespawnPosition();
        ChangeState(UnitState.Get);
    }

    public bool IsDead() {
        if (_currentHp <= 0)
            return true;
        return false;
    }
}

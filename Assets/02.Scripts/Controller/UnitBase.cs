using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public abstract class UnitBase : MonoBehaviour
{
    private const int OUTLINE_NUMBER = 6;
    protected GameObject _weapons;
    protected Transform _firePoint;
    public Transform TargetPos { get; private set; }

    [SerializeField] protected int _currentHp = 100;
    [SerializeField] protected int _maxHp = 100;
    [SerializeField] protected float _moveSpeed = 5f;
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

    public Action<int> KillNumberEvent;
    public Action<int> DeathNumberEvent;

    public Transform FirePoint => _firePoint;

    public int GetCurrentHp => _currentHp;
    public int GetMaxHp => _maxHp;
    public int MyKill { get; private set; }
    public int MyDead { get; private set; }
    public ModelController Model { get; private set; }
    public IItem CollideItem { get; set; }
    public Quaternion UnitRotate { get; protected set; }

    protected virtual void Awake() {
        _outlines = new Outline[OUTLINE_NUMBER];
        for(int i = 0; i < _outlines.Length; i++) {
            _outlines[i] = GetComponentsInChildren<Outline>()[i];
        }

        _ufx = GetComponent<UnitSfxController>();
        Model = GetComponentInChildren<ModelController>();

        _weapons = Util.FindChild(gameObject, "Weapons", false);
        _firePoint = Util.FindChild(gameObject, "FirePoint", true).transform;
        TargetPos = Util.FindChild(gameObject, "TargetPos", false).transform;

        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false).GetComponent<IWeapon>();
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false).GetComponent<IWeapon>();

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
            ItemController go = Managers.Resources.Instantiate(BaseWeapon.CreateObject, null).GetComponent<ItemController>();
            go.Init(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), true);
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

    protected virtual void IsHitEvent(int damage,Transform attackerTrans, Transform myTrans) {
        SetHp(-damage);
    }

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

    protected virtual void IsDeadEvent(Transform attackerTrans, bool headShot) {
        SetOutline(false);
        _ufx.PlaySfx(UnitSfx.Dead);
        Invoke("Init", Managers.GameManager.RespawnTime);
        _bodyCollider.enabled = false;
        _headCollider.enabled = false;
        MyDead++;
        DeathNumberEvent?.Invoke(MyDead);
        ChangeWeapon(WeaponType.Pistol);
        UnitBase target = attackerTrans.GetComponentInParent<UnitBase>();
        target.MyKill++;
        target.KillNumberEvent?.Invoke(target.MyKill);
        Managers.GameManager.BoardSortToRank();
        Model.ResetAnimator();
        ChangeState(UnitState.Dead);
        GameObject kit = Managers.Resources.Instantiate("Item/Healkit", null);
        kit.transform.position = transform.position + Vector3.up;
        UI_KillFeed feed =  Managers.Resources.Instantiate("UI/KillFeed", Managers.GameManager.KillFeedParent).GetComponent<UI_KillFeed>();
        feed.Init(target.BaseWeapon.Type, attackerTrans.gameObject.name, gameObject.name);
        feed.transform.SetSiblingIndex(0);
        if (Managers.GameManager.KillLimit > 0) {
            if (target.MyKill >= Managers.GameManager.KillLimit) {
                Managers.GameManager.ChangeState(GameState.Gameover);
                return;
            }
        }

        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            player.KillEvent.Invoke();
            if(player.IstripleKill) {
                return;
            }
            if(!player.IsKill && !player.IsDoubleKill) {
                player.IsKill = true;
                return;
            }
            if(player.IsKill && !player.IsDoubleKill) {
                player.IsKill = false;
                player.IsDoubleKill = true;

                player.DoubleKillEvent.Invoke();
                ShareSfxController.instance.SetShareSfx(ShareSfx.Dominate);
                return;
            }
            if(!player.IsKill &&  player.IsDoubleKill && !player.IstripleKill) {
                player.IsDoubleKill = false;
                player.IstripleKill = true;
                ShareSfxController.instance.SetShareSfx(ShareSfx.Rampage);
                player.TripleKillEvent.Invoke();
            }
        }
    }


    public virtual int SetHp(int damage) {
        _currentHp += damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, _maxHp);
        return _currentHp;
    }

    public virtual void Init() {
        CollideItem = null;
        SetOutline(true);
        WeaponInit();
        _currentHp = _maxHp;
        transform.position = Managers.RespawnManager.GetRespawnPosition();
        ChangeState(UnitState.Get);
        UnitRotate = Quaternion.identity;
    }

    public bool IsDead() {
        if (_currentHp <= 0)
            return true;
        return false;
    }
}

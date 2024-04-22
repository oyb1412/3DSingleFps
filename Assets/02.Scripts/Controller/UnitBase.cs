using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

public abstract class UnitBase : MonoBehaviour
{
    protected StatusBase _status;
    protected GameObject _weapons;
    protected Transform _firePoint;
    [SerializeField]protected Collider _bodyCollider;
    [SerializeField] protected Collider _headCollider;


    protected IWeapon _currentWeapon;
    protected IWeapon[] _weaponList = new IWeapon[(int)WeaponType.Count];
    [SerializeField]protected UnitState _state = UnitState.Idle;
    protected UnitSfxController _ufx;

    public UnitSfxController Ufx => _ufx;

    public Base.WeaponController BaseWeapon => _currentWeapon as Base.WeaponController;

    public abstract void ChangeState(UnitState state);

    public UnitState State => _state;

    public Action<int> KillNumberEvent;
    public Action<int> DeathNumberEvent;
    public StatusBase Status => _status;

    public Transform FirePoint => _firePoint;
    public int MyKill { get; private set; }
    public int MyDead { get; private set; }
    public ModelController Model { get; private set; }
    public IItem CollideItem { get; set; }
    public Quaternion UnitRotate { get; protected set; }




    protected virtual void Awake() {
        _ufx = GetComponent<UnitSfxController>();
        Model = GetComponentInChildren<ModelController>();
        _status = GetComponent<StatusBase>();

        _weapons = Util.FindChild(gameObject, "Weapons", false);
        _firePoint = Util.FindChild(gameObject, "FirePoint", true).transform;

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
            go.Init(new Vector3(transform.position.x, 1f, transform.position.z), true);
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

    protected void GetItem() {
        if (CollideItem == null)
            return;

        CollideItem.Pickup(this);
    }

    public void TakeDamage(int damage, Transform attackerTrans, Transform myTrans) {
        IsHitEvent(damage, attackerTrans, myTrans);
        if (_status._currentHp <= 0 && State != UnitState.Dead) {
            IsDeadEvent(attackerTrans);
        }
    }

    protected virtual void IsHitEvent(int damage,Transform attackerTrans, Transform myTrans) {
        _status._currentHp -= damage;
    }

    protected virtual void IsDeadEvent(Transform attackerTrans) {
        _ufx.PlaySfx(UnitSfx.Dead);
        Invoke("Init", Managers.GameManager.RespawnTime);
        _bodyCollider.enabled = false;
        _headCollider.enabled = false;
        MyDead++;
        DeathNumberEvent?.Invoke(MyDead);
        ChangeWeapon(WeaponType.Pistol);
        UnitBase target = attackerTrans.GetComponent<UnitBase>();
        target.MyKill++;
        target.KillNumberEvent?.Invoke(target.MyKill);
        Managers.GameManager.BoardSortToRank();
        Model.ResetAnimator();
        ChangeState(UnitState.Dead);
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

    public virtual void Init() {
       _bodyCollider.enabled = true;
        _headCollider.enabled = true;
        _status._currentHp = _status._maxHp;
        transform.position = Managers.RespawnManager.GetRespawnPosition();
        ChangeState(UnitState.Get);
    }


    public bool IsDead() {
        if (_status._currentHp <= 0)
            return true;
        return false;
    }
}

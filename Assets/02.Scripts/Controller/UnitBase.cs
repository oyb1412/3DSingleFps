using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static Define;

public class UnitBase : MonoBehaviour
{
    public ModelController Model { get; set; }
    protected StatusBase _status;
    protected GameObject _weapons;
    protected Transform _firePoint;
    [SerializeField]protected Collider _bodyCollider;
    [SerializeField] protected Collider _headCollider;
    public IItem CollideItem { get; set; }
    public Quaternion UnitRotate { get; protected set; }

    protected IWeapon _currentWeapon;
    protected IWeapon[] _weaponList = new IWeapon[(int)WeaponType.Count];
    [SerializeField]protected UnitState _state = UnitState.Idle;

    public UnitState State => _state;
    public StatusBase Status => _status;

    public float _RespawnTime { get; private set; } = 1f;
    public int MyKill { get; set; }
    public int MyDead { get; private set; }
    public int MyRank { get; set; }

    public Action<int> KillNumberEvent;
    public Action<int> DeathNumberEvent;
    protected virtual void Awake() {
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

    public void ChangeState(UnitState state, bool trigger) {
        _state = state;
        _currentWeapon.SetAnimation(state, trigger);
        Model.ChangeAnimation(state, trigger);
    }

    public void ChangeState(UnitState state) {
        _state = state;
        _currentWeapon.SetAnimation(state);
        Model.ChangeAnimation(state);
    }

    private void DropWeapon() {
        if (_currentWeapon != _weaponList[(int)WeaponType.Pistol]) {
            ItemController go = Managers.Resources.Instantiate(_currentWeapon.CreateObject, null).GetComponent<ItemController>();
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
    }

    protected void GetItem() {
        if (CollideItem == null)
            return;

        CollideItem.Pickup(this);
    }

    public void TakeDamage(int damage, Transform attackerTrans, Transform myTrans) {
        IsHitEvent(damage, attackerTrans, myTrans);
        if (_status._currentHp <= 0) {
            IsDeadEvent(attackerTrans);
        }
    }

    protected virtual void IsHitEvent(int damage,Transform attackerTrans, Transform myTrans) {
        _status._currentHp -= damage;
    }

    protected virtual void IsDeadEvent(Transform attackerTrans) {
        _bodyCollider.enabled = false;
        _headCollider.enabled = false;
        MyDead++;
        DeathNumberEvent?.Invoke(MyDead);
        DropWeapon();
        attackerTrans.GetComponent<UnitBase>().MyKill++;
        attackerTrans.GetComponent<UnitBase>().KillNumberEvent?.Invoke(attackerTrans.GetComponent<UnitBase>().MyKill);
        Managers.GameManager.UnitsSortToRank();
        ChangeState(UnitState.Dead);
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
                return;
            }
            if(!player.IsKill &&  player.IsDoubleKill && !player.IstripleKill) {
                player.IsDoubleKill = false;
                player.IstripleKill = true;

                player.TripleKillEvent.Invoke();
            }

        }
    }

    public virtual void Init() {
        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
        _status._currentHp = _status._maxHp;
        transform.position = Managers.RespawnManager.GetValidSpawnPosition();
        ChangeState(UnitState.Get);
    }


    public bool IsDead() {
        if (_status._currentHp <= 0)
            return true;
        return false;
    }
}

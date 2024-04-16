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
    public IItem CollideItem { get; set; }

    protected IWeapon _currentWeapon;
    protected IWeapon[] _weaponList = new IWeapon[(int)WeaponType.Count];
    [SerializeField]protected UnitState _state = UnitState.Idle;

    public UnitState State => _state;
    public StatusBase Status => _status;

    public int MyKill { get; set; }
    public int MyDead { get; private set; }


    protected virtual void Awake() {
        Model = GetComponentInChildren<ModelController>();
        _status = GetComponent<StatusBase>();

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
        if (state == UnitState.Reload || state == UnitState.Shot
            || state == UnitState.Get || state == UnitState.Dead)
            _currentWeapon.SetAnimation(state);
        Model.ChangeAnimation(state);
    }

    public virtual void ChangeWeapon(WeaponType type) {
        ItemController go = Managers.Resources.Instantiate(_currentWeapon.CreateObject, null).GetComponent<ItemController>();
        go.Init(new Vector3(transform.position.x, 1f, transform.position.z), true);

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

    public virtual void TakeDamage(int damage, Transform attackerTrans, Transform myTrans) {
        _status._currentHp -= damage;
        if (_status._currentHp <= 0) {
            transform.LookAt(attackerTrans);
            MyDead++;
            attackerTrans.GetComponent<UnitBase>().MyKill++;
            ChangeState(UnitState.Dead);
        }
    }

    public bool IsDead() {
        if (_status._currentHp <= 0)
            return true;
        return false;
    }
}

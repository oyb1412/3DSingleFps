using Fusion;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public abstract class UnitBase : MonoBehaviourPunCallbacks
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

    public PhotonView PV { get; private set; }

    protected virtual void Awake() {
        PV = GetComponent<PhotonView>();

        _outlines = new Outline[OUTLINE_NUMBER];
        for(int i = 0; i < _outlines.Length; i++) {
            _outlines[i] = GetComponentsInChildren<Outline>()[i];
        }

        _ufx = GetComponent<UnitSfxController>();
        Model = transform.GetComponentInChildren<ModelController>();

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
        if (!PV.IsMine)
            return;

        if (!GameManager.Instance.InGame())
            return;

        if (_currentWeapon.TryReload(this))
            ChangeState(UnitState.Reload);
    }

    private void DropWeapon() {
        if (BaseWeapon.Type == WeaponType.Pistol)
            return;

        if (_currentWeapon == _weaponList[(int)WeaponType.Pistol])
            return;

        if (!PV.IsMine)
            return;

        //아이템 2개 생성
        //아이템 픽업시 사라지지 않음
        //PV.RPC("RPC_DropWeapon", RpcTarget.AllBuffered, PV.OwnerActorNr);
        NetworkManager.Instance.PhotonCreate($"Prefabs/Item/{BaseWeapon.Type.ToString()}", transform.position + Vector3.up,
            Quaternion.identity);
    }

    [PunRPC]
    public void RPC_DropWeapon(int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        if (BaseWeapon.Type == WeaponType.Pistol)
            return;

        Debug.Log($"{PV.OwnerActorNr}번 플레이어 아이템 드랍");
        PhotonNetwork.Instantiate($"Prefabs/Item/{BaseWeapon.Type.ToString()}", transform.position + Vector3.up,
            Quaternion.identity);
    }

    private void DeadWeaponEvent() {
        DropWeapon();
        _weaponList[(int)WeaponType.Pistol].myObject.SetActive(true);
        _currentWeapon = _weaponList[(int)WeaponType.Pistol];
        _currentWeapon.Activation(_firePoint, this);
        foreach (var item in _weaponList) {
            if (_currentWeapon != item)
                item.myObject.SetActive(false);
        }
        CollideItem = null;
        Model.ChangeWeapon(WeaponType.Pistol);
    }

    [PunRPC]
    public void RPC_ChangeWeapon(int type, int actorNumber) {
        if(PV.OwnerActorNr != actorNumber)
            return;

        _weaponList[(int)type].myObject.SetActive(true);
        _currentWeapon = _weaponList[(int)type];
        _currentWeapon.Activation(_firePoint, this);
        ChangeState(UnitState.Get);
        foreach (var item in _weaponList) {
            if (_currentWeapon != item)
                item.myObject.SetActive(false);
        }
        CollideItem = null;
        Model.ChangeWeapon((WeaponType)type);
    }

    public virtual void ChangeWeapon(WeaponType type) {
        DropWeapon();

        PV.RPC("RPC_ChangeWeapon", RpcTarget.AllBuffered, (int)type, PV.OwnerActorNr);
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
        SetHp(-damage);
        if (_currentHp <= 0 && State != UnitState.Dead) {
            IsDeadEvent(attackerTrans, headShot);
        }
    }

    protected abstract void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans);

    private void SetOutline(bool trigger) {
        PV.RPC("RPC_SetOutline", RpcTarget.AllBuffered, trigger, PV.OwnerActorNr);
    }

    [PunRPC]
    public void RPC_SetOutline(bool trigger, int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        if (trigger) {
            foreach (var t in _outlines) {
                t.enabled = true;
            }
        } else {
            foreach (var t in _outlines) {
                t.enabled = false;
            }
        }
    }

    [PunRPC]
    public void RPC_FeedInit(int handle, int type, string attackName, string myName) {
        if (PV.OwnerActorNr != handle)
            return;

        UI_KillFeed feed = PhotonNetwork.Instantiate("Prefabs/UI/KillFeed", Vector3.zero, Quaternion.identity).GetComponent<UI_KillFeed>();
        feed.Init((WeaponType)type, attackName, myName, GameManager.Instance.KillFeedParent);
        feed.transform.SetSiblingIndex(0);
    }

    [PunRPC]
    public void SetScoreBoard(int attackerNumber, int myNumber) {
        if (PV.OwnerActorNr != myNumber)
            return;

        UnitBase my = Util.FindPlayerByActorNumber(myNumber);
        my.MyDead++;
        my.DeathNumberEvent?.Invoke(MyDead);

        my._bodyCollider.enabled = false;
        my._headCollider.enabled = false;
        my.Model.ResetAnimator();
        GameManager.Instance.BoardSortToRank();

        UnitBase attacker = Util.FindPlayerByActorNumber(attackerNumber);
        attacker.MyKill++;
        attacker.KillNumberEvent?.Invoke(attacker.MyKill);
    }

    [PunRPC]
    public void RPC_UnitDeadEvent(int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        _bodyCollider.enabled = false;
        _headCollider.enabled = false;
    }

    protected virtual void IsDeadEvent(Transform attackerTrans, bool headShot) {
        if (!PV.IsMine)
            return;

        SetOutline(false);
        _ufx.PlaySfx(UnitSfx.Dead);
        Invoke("Init", GameManager.Instance.RespawnTime);

        DeadWeaponEvent();
        UnitBase target = attackerTrans.GetComponent<UnitBase>();

        PV.RPC("RPC_UnitDeadEvent", RpcTarget.AllBuffered, PV.OwnerActorNr);

        PV.RPC("SetScoreBoard", RpcTarget.AllBuffered, target.PV.OwnerActorNr, PV.OwnerActorNr);

        ChangeState(UnitState.Dead);

        NetworkManager.Instance.PhotonCreate("Prefabs/Item/Healkit", transform.position + Vector3.up, Quaternion.identity);

        PV.RPC("RPC_FeedInit", RpcTarget.AllBuffered, PV.OwnerActorNr, (int)target.BaseWeapon.Type, attackerTrans.gameObject.name, gameObject.name);

        if (GameManager.Instance.KillLimit > 0) {
            if (target.MyKill >= GameManager.Instance.KillLimit) {
                GameManager.Instance.ChangeState(GameState.Gameover);
                return;
            }
        }

        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            player.KillEvent?.Invoke();
            if (player.IstripleKill) {
                return;
            }
            if (!player.IsKill && !player.IsDoubleKill) {
                player.IsKill = true;
                return;
            }
            if (player.IsKill && !player.IsDoubleKill) {
                player.IsKill = false;
                player.IsDoubleKill = true;

                player.MutilKillEvent?.Invoke(PlayerController.DOUBLE_KILL);
                ShareSfxController.instance.SetShareSfx(ShareSfx.Dominate);
                return;
            }
            if (!player.IsKill && player.IsDoubleKill && !player.IstripleKill) {

                player.IsDoubleKill = false;
                player.IstripleKill = true;
                ShareSfxController.instance.SetShareSfx(ShareSfx.Rampage);
                player.MutilKillEvent?.Invoke(PlayerController.TRIPLE_KILL);
            }
        }
    }

    protected void SetWeaponTriggerAnimation(string type, int actorNumber) {
        PV.RPC("RPC_WeaponSetTrigger", RpcTarget.AllBuffered, type, actorNumber);
    }

    protected void SetModelTriggerAnimation(string type, int actorNumber) {
        PV.RPC("RPC_ModelSetTrigger", RpcTarget.AllBuffered, type, actorNumber);
    }

    [PunRPC]
    public void RPC_WeaponSetTrigger(string type, int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        BaseWeapon.Animator.SetTrigger(type);
    }

    [PunRPC]
    public void RPC_ModelSetTrigger(string type, int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        Model.Animator.SetTrigger(type);
    }

    public virtual void SetHp(int damage) {
        _currentHp += damage;
        _currentHp = Mathf.Clamp(_currentHp, 0, _maxHp);
    }

    public virtual void Init() {
        CollideItem = null;
        SetOutline(true);
        WeaponInit();
        _currentHp = _maxHp;
        transform.position = Managers.RespawnManager.GetRespawnPosition();
        ChangeState(UnitState.Get);
        UnitRotate = Quaternion.identity;
        PV.RPC("RPC_Init", RpcTarget.AllBuffered, PV.OwnerActorNr);
    }

    [PunRPC]
    public void RPC_Init(int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
    }

    public abstract void ChangeState(UnitState state);

    public bool IsDead() {
        if (_currentHp <= 0)
            return true;
        return false;
    }
}

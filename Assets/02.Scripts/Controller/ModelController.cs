using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ModelController : MonoBehaviour
{
    private Animator _animator;
    private GameObject _weapons;
    private GameObject[] _weaponList = new GameObject[(int)WeaponType.Count];
    public PhotonView PV { get; private set; }

    private UnitBase _unit;

    public Animator Animator => _animator;
    private void Awake() {
        PV = GetComponent<PhotonView>();

        if (!PV.IsMine) {
            Util.SetLayer(gameObject, LayerType.OtherModel);
            return;
        }
    }
    private void Start() {
        _unit = transform.GetComponentInParent<UnitBase>();
        PV.OwnerActorNr = _unit.PV.OwnerActorNr;
        _animator = GetComponent<Animator>();
        _weapons = Util.FindChild(gameObject, "Weapons", true);
        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false);
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false);
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false);

        foreach(var t in _weaponList) {
            t.SetActive(false);
        }

        _weaponList[(int)WeaponType.Pistol].SetActive(true);
    }

    private void Update() {
        //transform.eulerAngles = new Vector3(0f, transform.parent.eulerAngles.y, 0f); 
    }

    [PunRPC]
    public void RPC_ChangeWeapon(int type, int actorNumber) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        foreach (var t in _weaponList) {
            t.SetActive(false);
        }

        _weaponList[(int)type].SetActive(true);
    }

    public void ChangeWeapon(WeaponType type) {
        PV.RPC("RPC_ChangeWeapon", RpcTarget.AllBuffered, (int)type, PV.OwnerActorNr);
    }

    public void ResetAnimator() {
        if (!PV.IsMine)
            return;

        _animator.gameObject.SetActive(false);
        _animator.gameObject.SetActive(true);
    }
}

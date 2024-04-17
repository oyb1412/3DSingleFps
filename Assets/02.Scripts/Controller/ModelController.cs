using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ModelController : MonoBehaviour
{
    private Animator _animator;
    private GameObject _weapons;
    private GameObject[] _weaponList = new GameObject[(int)WeaponType.Count];
    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        _weapons = Util.FindChild(gameObject, "Weapons", true);
        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false);
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false);
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false);

        foreach(var t in _weaponList) {
            t.SetActive(false);
        }

        _weaponList[(int)WeaponType.Pistol].SetActive(true);
    }

    public void ChangeWeapon(WeaponType type) {
        foreach (var t in _weaponList) {
            t.SetActive(false);
        }

        _weaponList[(int)type].SetActive(true);
    }

    public void ChangeAnimation(UnitState state, bool trigger) {
        _animator.SetBool("Move", false);
        _animator.SetBool("Shot", false);
        _animator.SetBool(state.ToString(), trigger);
    }

    public void ChangeAnimation(UnitState state) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(state.ToString()))
            return;

        _animator.SetTrigger(state.ToString());
    }

    public void ResetTrigger(string name) {
        _animator.ResetTrigger(name);
    }

}

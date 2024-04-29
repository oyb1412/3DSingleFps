using UnityEngine;
using static Define;

public class ModelController : MonoBehaviour
{
    private Animator _animator;
    private GameObject _weapons;
    private GameObject[] _weaponList = new GameObject[(int)WeaponType.Count];
    public Animator Animator => _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void Start() {
        _weapons = Util.FindChild(gameObject, NAME_WEAPONS, true);
        _weaponList[(int)WeaponType.Pistol] = Util.FindChild(_weapons, WeaponType.Pistol.ToString(), false);
        _weaponList[(int)WeaponType.Rifle] = Util.FindChild(_weapons, WeaponType.Rifle.ToString(), false);
        _weaponList[(int)WeaponType.Shotgun] = Util.FindChild(_weapons, WeaponType.Shotgun.ToString(), false);

        ChangeWeapon(WeaponType.Pistol);
    }

    public void ChangeWeapon(WeaponType type) {
        foreach (var t in _weaponList) {
            t.SetActive(false);
        }
        _weaponList[(int)type].SetActive(true);
    }

    public void ResetAnimator() {
        _animator.gameObject.SetActive(false);
        _animator.gameObject.SetActive(true);
    }
}

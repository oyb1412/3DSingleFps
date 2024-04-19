using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    Define.WeaponType Type { get; set; }
    GameObject myObject { get; }
    GameObject CreateObject { get; set; }
    int Damage { get; set; }
    void Activation(Transform firePoint = null, UnitBase unit = null);
    void Shot();
    void Reload();
    bool TryReload(UnitBase unit);
    bool TryShot(UnitBase unit);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    GameObject myObject { get; }
    void Activation(Transform firePoint = null, UnitBase unit = null);
    void Shot();
    void Reload();
    bool TryReload(UnitBase unit);
    bool TryShot(UnitBase unit);
}

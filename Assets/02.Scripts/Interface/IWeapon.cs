using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    GameObject myObject { get; }
    GameObject CreateObject { get; set; }
    int Damage { get; set; }
    void Activation(Transform firePoint = null, UnitBase unit = null);
    void Shot();
    void Reload();
    bool TryReload(UnitBase unit);
    bool TryShot(UnitBase unit);
    void SetAnimation(Define.UnitState anime, bool trigger);
    void SetAnimation(Define.UnitState anime);
    void EndAnimation(string name);
}

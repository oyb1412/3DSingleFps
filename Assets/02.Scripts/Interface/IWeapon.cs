using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    GameObject myObject { get; }
    GameObject CreateObject { get; set; }
    int Damage { get; set; }
    float BoundValue { get; set; }
    float CrossValue { get; set; }
    void Activation(Transform firePoint = null, UnitBase player = null);
    void Shot();
    void Reload();
    bool TryReload(UnitBase player);
    bool TryShot(UnitBase player);
    void SetAnimation(Define.UnitState anime, bool trigger);
    void SetAnimation(Define.UnitState anime);
    void EndAnimation(string name);
}

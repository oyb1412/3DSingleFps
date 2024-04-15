using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    GameObject myObject { get; }
    
    float BoundValue { get; set; }
    void Activation(Transform firePoint = null, PlayerController player = null);
    void Shot();
    void Reload();
    bool TryReload(PlayerController player);
    bool TryShot(PlayerController player);
    void SetAnimation(Define.PlayerState anime, bool trigger);
    void SetAnimation(Define.PlayerState anime);
    void EndAnimation(string name);
}

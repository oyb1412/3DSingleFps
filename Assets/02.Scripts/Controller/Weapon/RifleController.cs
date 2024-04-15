using UnityEngine;

public class RifleController : WeaponController
{
    protected override void Awake() {
        base.Awake();
        BoundValue = 0.05f;
        CrossValue = 150f;
        Damage = 10;
        Name = "Rifle";
        WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/RifleIcon");
        CurrentBullet = 30;
        RemainBullet = 30;
        MaxBullet = 180;
    }
    public override void Shot() {
        base.Shot();
        _player.ShotEvent.Invoke();

    }
}

using UnityEngine;

public class PistolController : WeaponController
{
    protected override void Awake() {
        base.Awake();
        BoundValue = 0.1f;
        CrossValue = 350f;
        Damage = 20;
        Name = "Pistol";
        WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/PistolIcon");
        CurrentBullet  = 12;
        RemainBullet = 12;
        MaxBullet  = 60;
    }
    public override void Shot() {
        base.Shot();
        for(int i = 0; i< 5; i++)
            _player.ShotEvent.Invoke();
    }
}

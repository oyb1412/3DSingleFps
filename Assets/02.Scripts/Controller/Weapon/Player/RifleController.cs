using UnityEngine;

namespace Player {
    public class RifleController : WeaponController {
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
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Rifle");
        }
        public override void Shot() {
            base.Shot();
            DefaultShot(transform.forward);
            _player.ShotEvent.Invoke();
        }
    }
}


using UnityEngine;
using static Define;

namespace Enemy {
    public class ShotgunController : WeaponController {
        private float _bulletNumber = 6;
        protected override void Awake() {
            base.Awake();
            Damage = 30;
            _shotDelay = 1f;
            Name = "Shotgun";
            Type = WeaponType.Shotgun;

            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/ShotgunIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Shotgun");
        }

        protected override void Enable() {
            CurrentBullet = 6;
            RemainBullet = 6;
            MaxBullet = 30;
        }
        public override void Shot() {
            base.Shot();
            for (int i = 0; i < _bulletNumber; i++) {
                DefaultShot(transform.forward);
            }
        }
    }
}


using UnityEngine;
using static Define;

namespace Enemy {
    public class ShotgunController : WeaponController {
        [SerializeField] private float _shotAngle;
        private float _bulletNumber = 8;
        protected override void Awake() {
            base.Awake();
            Damage = 15;
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
                var ran1 = Random.Range(-_shotAngle, _shotAngle);
                var ran2 = Random.Range(-_shotAngle, _shotAngle);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * transform.forward;

                DefaultShot(pelletDirection);
            }
        }
    }
}


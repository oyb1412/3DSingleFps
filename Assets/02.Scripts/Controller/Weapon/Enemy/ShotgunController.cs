using UnityEngine;
using static Define;

namespace Enemy {
    public class ShotgunController : WeaponController {
        [SerializeField] private float _bulletAngle;
        [SerializeField] private int _bulletNumber;
        protected override void Awake() {
            base.Awake();
            BoundValue = 0.1f;
            CrossValue = 450f;
            Damage = 1;
            Name = "Shotgun";
            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/ShotgunIcon");
            CurrentBullet = 6;
            RemainBullet = 6;
            MaxBullet = 30;
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Shotgun");
        }
        public override void Shot() {
            base.Shot();
            Vector3 angle = transform.forward;
            for (int i = 0; i < _bulletNumber; i++) {

                var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
                var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * angle;

                DefaultShot(pelletDirection);
            }
        }
    }
}


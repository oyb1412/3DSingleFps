using UnityEngine;
using static Define;

namespace Player {
    public class ShotgunController : WeaponController {
        [SerializeField] private float _bulletAngle;
        [SerializeField] private int _bulletNumber;
        protected override void Awake() {
            base.Awake();
            VerticalBoundValue = 5.0f;
            HorizontalBoundValue = 2f;
            _shotDelay = .8f;
            Type = Define.WeaponType.Shotgun;

            CrossValue = 450f;
            Damage = 30;
            Name = "Shotgun";
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
            Vector3 angle = transform.forward;
            for (int i = 0; i < _bulletNumber; i++) {
                Player.ShotEvent.Invoke();

                var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
                var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * angle;

                DefaultShot(pelletDirection);
            }
        }
    }

}

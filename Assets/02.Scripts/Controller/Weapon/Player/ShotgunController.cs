using UnityEngine;
using static Define;

namespace Player {
    public class ShotgunController : WeaponController {
        [SerializeField] private float _bulletAngle;
        [SerializeField] private int _bulletNumber;
        protected override void Awake() {
            base.Awake();
            VerticalBoundValue = 4.0f;
            HorizontalBoundValue = 1f;
            _shotDelay = .8f;
            Type = WeaponType.Shotgun;
            
            CrossValue = 350f;
            Damage = 15;
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

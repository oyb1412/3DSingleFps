using UnityEngine;
using static Define;

namespace Player {
    public class ShotgunController : WeaponController {
        private const float AIM_ANGLE = 1f;

        [SerializeField] private float _bulletAngle;
        [SerializeField] private int _bulletNumber;

        protected override void Awake() {
            base.Awake();

            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/ShotgunIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Shotgun");
            Name = "Shotgun";
        }

        protected override void Start() {
            base.Start();

            Player.AimEvent += SetBulletAngle;
            VerticalBoundValue = 4.0f;
            HorizontalBoundValue = 1f;
            _shotDelay = .8f;
            Type = WeaponType.Shotgun;

            CrossValue = 350f;
            Damage = 15;
        }

        public void SetBulletAngle(bool trigger) {

            _bulletAngle = trigger ? AIM_ANGLE : _bulletAngle; 
        }

        protected override void Enable() {
     
            CurrentBullet = 6;
            RemainBullet = 6;
            MaxBullet = 30;
        }
        public override void Shot() {
            base.Shot();

            for (int i = 0; i < _bulletNumber; i++) {
                Player?.ShotEvent?.Invoke();

                var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
                var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * transform.forward;

                DefaultShot(pelletDirection);
            }
        }
    }

}

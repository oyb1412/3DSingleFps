using UnityEngine;
using static Define;

namespace Player {
    public class ShotgunController : WeaponController {
        private Base.ShotgunController _shotgun = new Base.ShotgunController();
        private float _bulletAngle = SHOTGUN_ANGLE;

        protected override void Awake() {
            base.Awake();
            _shotgun.SetShotgun(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
            CameraView = SHOTGUN_CAMERAVIEW;
        }

        protected override void Start() {
            base.Start();

            Player.AimEvent += SetBulletAngle;
            VerticalBoundValue = SHOTGUN_VECTICALBOUND_VALUE;
            HorizontalBoundValue = SHOTGUN_HORIZONTALBOUND_VALUE;
            CrossValue = SHOTGUN_CROSSHAIR_VALUE;
        }

        public void SetBulletAngle(bool trigger) {
            _bulletAngle = trigger ? SHOTGUN_ANGLE : _bulletAngle; 
        }

        protected override void Enable() {
            _shotgun.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }
        public override void Shot() {
            base.Shot();

            for (int i = 0; i < SHOTGUN_PALLET_NUMBER; i++) {
                Player.ShotEvent.Invoke();

                var ranX = Random.Range(-_bulletAngle, _bulletAngle);
                var ranY = Random.Range(-_bulletAngle, _bulletAngle);

                Quaternion pelletRotation = Quaternion.Euler(ranX, ranY, 0);
                Vector3 pelletDirection = pelletRotation * transform.forward;

                DefaultShot(pelletDirection);
            }
        }
    }

}

using UnityEngine;
using static Define;

namespace Enemy {
    public class ShotgunController : WeaponController {
        private Base.ShotgunController _shotgun = new Base.ShotgunController();
        protected override void Awake() {
            base.Awake();
            _shotgun.SetShotgun(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
        }

        protected override void Enable() {
            _shotgun.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }
        public override void Shot() {
            base.Shot();

            for (int i = 0; i < SHOTGUN_PALLET_NUMBER; i++) {
                var ranX = Random.Range(-SHOTGUN_ANGLE, SHOTGUN_ANGLE);
                var ranY = Random.Range(-SHOTGUN_ANGLE, SHOTGUN_ANGLE);

                Quaternion pelletRotation = Quaternion.Euler(ranX, ranY, 0);
                Vector3 pelletDirection = pelletRotation * transform.forward;

                DefaultShot(pelletDirection);
            }
        }
    }
}


using UnityEngine;
using static Define;

namespace Enemy {
    public class RifleController : WeaponController {
        private Base.RifleController _rifle = new Base.RifleController();
        protected override void Awake() {
            base.Awake();
            _rifle.SetRifle(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
        }

        protected override void Enable() {
            _rifle.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }
        public override void Shot() {
            base.Shot();
            DefaultShot(transform.forward);
        }
    }

}

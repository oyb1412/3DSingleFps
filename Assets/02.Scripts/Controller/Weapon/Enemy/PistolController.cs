using UnityEngine;

namespace Enemy {
    public class PistolController : WeaponController {
        private Base.PistolController _pistol = new Base.PistolController();
        protected override void Awake() {
            base.Awake();

            _pistol.SetPistol(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
        }

        protected override void Enable() {
            _pistol.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }

        public override void Shot() {
            base.Shot();

            DefaultShot(transform.forward);
        }
    }

}

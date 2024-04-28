using UnityEngine;
using static Define;

namespace Player {
    public class RifleController : WeaponController {
        private Base.RifleController _rifle = new Base.RifleController();
        protected override void Awake() {
            base.Awake();
            _rifle.SetRifle(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
            CameraView = RIFLE_CAMERAVIEW;
        }

        protected override void Start() {
            base.Start();
            VerticalBoundValue = RIFLE_VECTICALBOUND_VALUE;
            HorizontalBoundValue = RIFLE_HORIZONTALBOUND_VALUE;
            CrossValue = RIFLE_CROSSHAIR_VALUE;
        }

        protected override void Enable() {
            _rifle.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }
        public override void Shot() {
            base.Shot();
    
            DefaultShot(transform.forward);
            Player.ShotEvent.Invoke();
        }
    }
}


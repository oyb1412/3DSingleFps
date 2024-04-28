using UnityEngine;
using static Define;

namespace Player {
    public class PistolController : WeaponController {
        private Base.PistolController _pistol = new Base.PistolController();
        protected override void Awake() {
            base.Awake();
            _pistol.SetPistol(ref _damage, ref _weaponType, ref _name, ref _shotDelay, ref _weaponIcon, ref _createObject);
            CameraView = PISTOL_CAMERAVIEW;
        }

        protected override void Start() {
            base.Start();

            VerticalBoundValue = PISTOL_VECTICALBOUND_VALUE;
            HorizontalBoundValue = PISTOL_HORIZONTALBOUND_VALUE;
            CrossValue = PISTOL_CROSSHAIR_VALUE;
        }

        protected override void Enable() {
            _pistol.SetEnable(ref _currentBullet, ref _remainBullet, ref _maxBullet);
        }
        public override void Shot() {
            base.Shot();

            DefaultShot(transform.forward);
            for (int i = 0; i < 5; i++)
                Player.ShotEvent.Invoke();
        }
    }
}


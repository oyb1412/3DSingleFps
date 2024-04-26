using UnityEngine;

namespace Player {
    public class RifleController : WeaponController {
        protected override void Awake() {
            base.Awake();

            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/RifleIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Rifle");
            Name = "Rifle";
        }

        protected override void Start() {
            base.Start();

            VerticalBoundValue = 1.5f;
            HorizontalBoundValue = .5f;
            _shotDelay = 0.1f;
            CrossValue = 100f;
            Damage = 27;
            Type = Define.WeaponType.Rifle;
        }

        protected override void Enable() {

            CurrentBullet = 30;
            RemainBullet = 30;
            MaxBullet = 180;
        }
        public override void Shot() {
            base.Shot();
    
            DefaultShot(transform.forward);
            Player?.ShotEvent?.Invoke();
        }
    }
}


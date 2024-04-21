using UnityEngine;

namespace Player {
    public class PistolController : WeaponController {
        protected override void Awake() {
            base.Awake();
            VerticalBoundValue = 2.5f;
            HorizontalBoundValue = 0.5f;
            _shotDelay = 0.4f;
            CrossValue = 200f;
            Type = Define.WeaponType.Pistol;
            Damage = 30;
            Name = "Pistol";
            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/PistolIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Pistol");
        }

        protected override void Enable() {
            CurrentBullet = 12;
            RemainBullet = 12;
            MaxBullet = 60;
        }
        public override void Shot() {
            base.Shot();
            DefaultShot(transform.forward);
            for (int i = 0; i < 5; i++)
                Player.ShotEvent.Invoke();
        }
    }
}


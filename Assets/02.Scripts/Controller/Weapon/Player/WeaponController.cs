using UnityEngine;
using static Define;

namespace Player {
    public abstract class WeaponController : Base.WeaponController {
        public float HorizontalBoundValue { get; set; }
        public float VerticalBoundValue { get; set; }
        public float CrossValue { get; set; }

        public PlayerController Player { get { return _unit as PlayerController; } }
        public override void Reload() {
            base.Reload();
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {

            Debug.DrawRay(_firePoint.position, angle * 100f, Color.green, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, angle, out var hit, float.MaxValue, _layerMask);

            if (!isHit)
                return;

            int layer = hit.collider.gameObject.layer;

            if (layer == (int)LayerType.Head &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != Player.gameObject) {
                Player.HeadshotEvent.Invoke();
                hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage * 3, Player.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
                return;

            } else if (layer == (int)LayerType.Body &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != Player.gameObject) {
                Player.BodyshotEvent.Invoke();
                hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage, Player.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
                return;
            }
            else if (layer == (int)LayerType.Obstacle ||
               layer == (int)LayerType.Wall) {
                GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                impact.transform.position = hit.point;
                impact.transform.LookAt(_firePoint.position);
                Destroy(impact, 1f);
                return;
            }
            else if (layer == (int)LayerType.Ground) {
                GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                impact.transform.position = hit.point;
                impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                Destroy(impact, 1f);
                return;
            }

            
        }
        public override void Shot() {
            base.Shot();
            Player.StartCoroutine(Player.COBound());
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

    }
}


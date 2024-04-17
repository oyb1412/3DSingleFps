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

            Debug.Log(hit.collider.name);
            int layer = hit.collider.gameObject.layer;

            
            if (layer == (int)LayerType.Obstacle ||
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

            if (layer == (int)LayerType.Unit) {
                hit.collider.GetComponent<ITakeDamage>().TakeDamage(Damage, Player.transform, hit.transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
            }
        }
        public override void Shot() {
            base.Shot();
            Player.StartCoroutine(Player.COBound());
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

    }
}


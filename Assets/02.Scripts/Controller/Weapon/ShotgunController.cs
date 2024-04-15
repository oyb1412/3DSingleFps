using UnityEngine;
using static Define;

public class ShotgunController : WeaponController
{
    [SerializeField] private float _bulletAngle;
    [SerializeField] private int _bulletNumber;
    protected override void Awake() {
        base.Awake();
        BoundValue = 1.5f;
    }
    public override void Shot() {
        Vector3 angle = transform.forward;
        _ejectEffect.Play();
        for (int i = 0; i < _bulletNumber; i++) {

            var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
            var ran2 = Random.Range(-_bulletAngle, _bulletAngle);
            _currentBullet--;

            var ran = Random.Range(0, 5);
            GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
            muzzle.transform.position = _firePos.position;

            Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
            Vector3 pelletDirection = pelletRotation * angle;

            Debug.Log($"{pelletDirection}방향으로 레이 발사");

            Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.red, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, pelletDirection, out var hit, float.MaxValue, _layerMask);

            if(isHit) {
                int layer = hit.collider.gameObject.layer;

                if (layer == (int)LayerType.Obstacle ||
                   layer == (int)LayerType.Ground) {
                    GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                    impact.transform.position = hit.point;
                    impact.transform.LookAt(_firePoint.position);
                    Destroy(impact, 1f);
                }
            }
            
        }
    }
}

using UnityEngine;
using static Define;

public class ShotgunController : WeaponController
{
    [SerializeField] private float _bulletAngle;
    [SerializeField] private int _bulletNumber;
    protected override void Awake() {
        base.Awake();
        BoundValue = 0.1f;
        CrossValue = 450f;
        Damage = 15;
        Name = "Shotgun";
        WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/ShotgunIcon");
        CurrentBullet = 6;
        RemainBullet = 6;
        MaxBullet = 30;
    }
    public override void Shot() {
        Vector3 angle = transform.forward;
        _ejectEffect.Play();
        CurrentBullet--;
        _player.BulletEvent.Invoke(CurrentBullet, MaxBullet);

        for (int i = 0; i < _bulletNumber; i++) {

            _player.ShotEvent.Invoke();

            var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
            var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

            var ran = Random.Range(0, 5);
            GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
            muzzle.transform.position = _firePos.position;

            Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
            Vector3 pelletDirection = pelletRotation * angle;


            Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.red, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, pelletDirection, out var hit, float.MaxValue, _layerMask);

            if(isHit) {
                int layer = hit.collider.gameObject.layer;
                Debug.Log(hit.collider.name);
                if (layer == (int)LayerType.Obstacle ||
                   layer == (int)LayerType.Wall) {
                    GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                    impact.transform.position = hit.point;
                    impact.transform.LookAt(_firePoint.position);
                    Destroy(impact, 1f);
                } else if (layer == (int)LayerType.Ground) {
                    GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                    impact.transform.position = hit.point;
                    impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                    Destroy(impact, 1f);
                }

                if (layer == (int)LayerType.Player) {
                    hit.collider.GetComponent<ITakeDamage>().TakeDamage(Damage);
                }
            }
            
        }
    }
}

using UnityEngine;
using static Define;

namespace Player {
    public class WeaponController : MonoBehaviour, IWeapon {
        public GameObject myObject { get { return gameObject; } }
        public GameObject CreateObject { get; set; }
        public int CurrentBullet { get; set; } = 30;
        public int RemainBullet { get; set; } = 30;
        public int MaxBullet { get; set; } = 180;
        public string Name { get; set; }
        public Sprite WeaponIcon { get; set; }

        protected Transform _firePos;
        protected int _layerMask;
        protected Transform _firePoint;
        protected PlayerController _player;
        protected ParticleSystem _ejectEffect;

        private Animator _animator;

        public float BoundValue { get; set; }
        public float CrossValue { get; set; }
        public int Damage { get; set; }

        

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
        }

        protected void Start() {
            _layerMask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();

        }
        public void Activation(Transform firePoint = null, UnitBase player = null) {
            if (firePoint != null && _firePoint == null) {
                _firePoint = firePoint;
            }
            if (player != null && _player == null) {
                _player = player as PlayerController;
            }
        }

        public void Reload() {
            if (MaxBullet >= RemainBullet) {
                CurrentBullet = RemainBullet;
                MaxBullet -= CurrentBullet;
            } else if (MaxBullet < RemainBullet) {
                CurrentBullet = RemainBullet - MaxBullet;
                MaxBullet = 0;
            }

            _player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

        protected void DefaultShot(Vector3 angle) {

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
                hit.collider.GetComponent<ITakeDamage>().TakeDamage(Damage, _player.transform, hit.transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
            }
        }
        public virtual void Shot() {
            CurrentBullet--;
            _player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
            _ejectEffect.Play();
            var ran = Random.Range(0, 5);
            GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
            muzzle.transform.position = _firePos.position;
            muzzle.transform.eulerAngles = _player.PlayerRotate;
        }
        public bool TryReload(UnitBase player) {
            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public bool TryShot(UnitBase player) {
            if (player.State == Define.UnitState.Reload ||
                player.State == Define.UnitState.Get)
                return false;

            if (CurrentBullet <= 0) {
                player.Reload();
                return false;
            }

            return true;
        }

        public void Dead() {

        }

        public void SetAnimation(UnitState anime, bool trigger) {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(anime.ToString()))
                return;

            _animator.SetBool(anime.ToString(), trigger);
            _player.Model.ChangeAnimation(anime, trigger);
        }

        public void SetAnimation(UnitState anime) {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(anime.ToString()))
                return;

            _animator.SetTrigger(anime.ToString());
            _player.Model.ChangeAnimation(anime);
        }

        public void EndAnimation(string name) {
            _animator.ResetTrigger(name);
            _player.Model.ResetTrigger(name);
            _player.ChangeState(UnitState.Idle);
        }
    }
}


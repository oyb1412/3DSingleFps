using UnityEditor;
using UnityEngine;
using static Define;

namespace Base {
    public abstract class WeaponController : MonoBehaviour, IWeapon {
        public GameObject myObject { get { return gameObject; } }
        public GameObject CreateObject { get; set; }
        public int CurrentBullet { get; set; }
        public int RemainBullet { get; set; }
        public int MaxBullet { get; set; }
        public string Name { get; set; }
        public Sprite WeaponIcon { get; set; }

        protected Transform _firePos;
        protected int _layerMask;
        protected Transform _firePoint;
        protected UnitBase _unit;
        protected ParticleSystem _ejectEffect;

        protected Animator _animator;

        protected abstract void Enable();

        public int Damage { get; set; }

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
        }

        protected void Start() {
            _layerMask = (1 << (int)LayerType.Head) | (1 << (int)LayerType.Body) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();

        }
        public void Activation(Transform firePoint = null, UnitBase unit = null) {
            if (firePoint != null && _firePoint == null) {
                _firePoint = firePoint;
            }
            if (unit != null && _unit == null) {
                _unit = unit;
            }
            Enable();

        }

        public virtual void Reload() {
            if (MaxBullet < RemainBullet) {
                CurrentBullet = MaxBullet;
                MaxBullet = 0;
            }
            else if (CurrentBullet < RemainBullet) {
                MaxBullet -= (RemainBullet - CurrentBullet);
                CurrentBullet = RemainBullet;
            }
            else if (MaxBullet >= RemainBullet) {
                CurrentBullet = RemainBullet;
                MaxBullet -= RemainBullet;
            }
        }

        protected abstract void DefaultShot(Vector3 angle);
        
        public virtual void Shot() {
            if(CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
            CurrentBullet--;
            _ejectEffect.Play();
            var ran = Random.Range(0, 5);
            GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
            muzzle.transform.position = _firePos.position;
            muzzle.transform.rotation = _unit.UnitRotate;

            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
        }
        public bool TryReload(UnitBase unit) {
            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public virtual bool TryShot(UnitBase unit) {
            if (unit.State == Define.UnitState.Reload ||
                unit.State == Define.UnitState.Get)
                return false;

            if (CurrentBullet <= 0) {
                unit.Reload();
                return false;
            }

            return true;
        }

        public void Respawn() {
            _unit.Init();
        }


        public void SetAnimation(UnitState anime, bool trigger) {

            _animator.SetBool(anime.ToString(), trigger);
            _unit.Model.ChangeAnimation(anime, trigger);
        }

        public void SetAnimation(UnitState anime) {

            _animator.SetTrigger(anime.ToString());
            _unit.Model.ChangeAnimation(anime);
        }

        public void EndAnimation(string name) {
            _animator.ResetTrigger(name);
            _unit.Model.ResetTrigger(name);
            _unit.ChangeState(UnitState.Idle);
        }
    }
}
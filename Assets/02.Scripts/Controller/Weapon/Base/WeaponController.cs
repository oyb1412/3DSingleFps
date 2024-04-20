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

        public float Delay { get { return _delay; }set { _delay = value; } }

        protected float _delay;
        protected float _shotDelay;

        [SerializeField]protected bool _isShot = true;

        protected Transform _firePos;
        protected int _layerMask;
        protected Transform _firePoint;
        protected UnitBase _unit;
        protected ParticleSystem _ejectEffect;

        [SerializeField] protected GameObject[] _muzzleEffect;
        [SerializeField] protected GameObject _bloodEffect;
        [SerializeField] protected GameObject _impactEffect;


        protected Animator _animator;

        public Animator Animator => _animator;
        protected abstract void Enable();

        public int Damage { get; set; }
        public WeaponType Type { get ; set ; }

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
        }

        protected void Start() {
            _layerMask = (1 << (int)LayerType.Head) | (1 << (int)LayerType.Body) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();
        }

        private void Update() {
            if (!Managers.GameManager.InGame())
                return;

            if ( _isShot ) {
                _delay += Time.deltaTime;
                if(_delay >= _shotDelay) {
                    _isShot = false;
                    _delay = 0;
                }
            }
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

            _unit.State = UnitState.Idle;
        }

        protected virtual void DefaultShot(Vector3 angle) {        }
        
        public virtual void Shot() {
            if(CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
            _isShot = true;
            CurrentBullet--;
            _ejectEffect.Play();
            var ran = Random.Range(0, 5);
            CreateEffect(_muzzleEffect[ran], _firePos.position, _unit.UnitRotate);

            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
        }

        protected GameObject CreateEffect(GameObject go, Vector3 pos, Quaternion rot = default ) {
            GameObject effect = Managers.Resources.Instantiate(go, null);
            effect.transform.position = pos;
            effect.transform.rotation = rot;
            return effect;
        }

        public bool TryReload(UnitBase unit) {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
                return false;

            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public virtual bool TryShot(UnitBase unit) {
            if (unit.State == Define.UnitState.Reload ||
                unit.State == Define.UnitState.Get ||
                unit.State == UnitState.Dead)
                return false;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
                return false;

            if (CurrentBullet <= 0) {
                unit.Reload();
                return false;
            }

            if (_isShot)
                return false;

            return true;
        }
    }
}
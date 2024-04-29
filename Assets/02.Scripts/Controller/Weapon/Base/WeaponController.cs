using UnityEngine;
using static Define;

namespace Base {
    public abstract class WeaponController : MonoBehaviour, IWeapon {

        protected int _currentBullet;
        protected int _remainBullet;
        protected int _maxBullet;
        protected float _shotDelay;
        protected int _damage;
        protected WeaponType _weaponType;
        protected string _name;
        protected Sprite _weaponIcon;
        protected GameObject _createObject;
        protected bool _isShot = true;
        protected Transform _firePos;
        protected int _layerMask;
        protected Transform _firePoint;
        protected UnitBase _unit;
        protected ParticleSystem _ejectEffect;
        protected GameObject[] _muzzleEffect;
        protected GameObject _bloodEffect;
        protected GameObject _impactEffect;
        private UnitSfxController _sfx;
        protected GameObject _bullet;

        public WeaponType Type => _weaponType;
        public int Damage => _damage;
        public GameObject CreateObject => _createObject;
        public int CurrentBullet => _currentBullet;
        public int RemainBullet => _remainBullet;
        public int MaxBullet => _maxBullet;
        public string Name => _name;
        public Sprite WeaponIcon => _weaponIcon;
        public float Delay { get; set; }
        public Animator Animator { get; private set; }
        public GameObject myObject { get { return gameObject; } }

        protected virtual void Awake() {
            Animator = GetComponent<Animator>();
            _bullet = (GameObject)Managers.Resources.Load<GameObject>(BULLET_OBJECT_PATH);
            _impactEffect = (GameObject)Managers.Resources.Load<GameObject>(IMPACT_EFFECT_PATH);
            _bloodEffect = (GameObject)Managers.Resources.Load<GameObject>(BLOOD_EFFECT_PATH);
            _muzzleEffect = new GameObject[5];

            for(int i = 0; i< _muzzleEffect.Length; i++) {
                _muzzleEffect[i] = (GameObject)Managers.Resources.Load<GameObject>(MUZZEL_EFFECT_PATH[i]);
            }
        }

        protected virtual void Start() {
            _unit = GetComponentInParent<UnitBase>();
            _layerMask = (1 << (int)LayerType.Head) | (1 << (int)LayerType.Body) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, NAME_FIREPOS, true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, NAME_EJECT, false).GetComponent<ParticleSystem>();
            _sfx = _unit.GetComponent<UnitSfxController>();
        }

        private void Update() {
            if (!Managers.GameManager.InGame())
                return;

            if (_unit.IsDead())
                return;

            if ( _isShot ) {
                Delay += Time.deltaTime;
                if(Delay >= _shotDelay) {
                    _isShot = false;
                    Delay = 0;
                }
            }
        }

        protected abstract void Enable();

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
                _currentBullet = MaxBullet;
                _maxBullet = 0;
            }
            else if (CurrentBullet < RemainBullet) {
                _maxBullet -= (RemainBullet - CurrentBullet);
                _currentBullet = RemainBullet;
            }
            else if (MaxBullet >= RemainBullet) {
                _currentBullet = RemainBullet;
                _maxBullet -= RemainBullet;
            }

            _unit.ChangeState(UnitState.Idle);
        }

        protected virtual void DefaultShot(Vector3 angle) { }

        private void HitAttack(RaycastHit hit, UnitBase unit, bool headShot) {
             int damage = headShot ? Damage * HEADSHOT_VALUE : Damage;
             hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(damage, unit.transform, hit.collider.GetComponentInParent<UnitBase>().transform, headShot);
             GameObject blood = CreateEffect(_bloodEffect, hit.point, Vector3.zero);
             blood.transform.LookAt(_firePoint.position);
             if (unit.TryGetComponent<PlayerController>(out var player)) {
                player.HurtShot(headShot);
            }
            Managers.Instance.DestoryCoroutine(blood, EFFECT_DESTORY_TIME);
        }

        private void HitObstacle(RaycastHit hit, Vector3 rot) {
            GameObject impact = CreateEffect(_impactEffect, hit.point, rot);
            impact.transform.LookAt(_firePoint.position);
            Managers.Instance.DestoryCoroutine(impact, EFFECT_DESTORY_TIME);
        }

        protected void DefaultShot(bool isHit, RaycastHit hit, UnitBase unit) {
           
            if (!isHit)
                return;

            int layer = hit.collider.gameObject.layer;
            if (layer == (int)LayerType.Head &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != unit.gameObject) {
                HitAttack(hit, unit, true);
                
            } else if (layer == (int)LayerType.Body &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != unit.gameObject) {
                HitAttack(hit, unit, false);

            } else if (layer == (int)LayerType.Obstacle ||
                 layer == (int)LayerType.Wall) {
                HitObstacle(hit, -_firePoint.eulerAngles);
            } else if (layer == (int)LayerType.Ground) {
                HitObstacle(hit, new Vector3(GROUND_EULERANGLES, 0f, 0f));
            }
        }

        public virtual void Shot() {
            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
            _isShot = true;
            _currentBullet--;
            _ejectEffect.Play();
        }

        protected GameObject CreateEffect(GameObject go, Vector3 pos, Vector3 rot) {
            GameObject effect = Managers.Resources.Instantiate(go, null);
            effect.transform.position = pos;
            effect.transform.eulerAngles = rot;
            return effect;
        }

        public bool TryReload(UnitBase unit) {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATOR_PARAMETER_RELOAD))
                return false;

            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public virtual bool TryShot(UnitBase unit) {
           
            if (unit.State == UnitState.Reload ||
                unit.State == UnitState.Get ||
                unit.State == UnitState.Dead)
                return false;

            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATOR_PARAMETER_DEAD))
                return false;

            if (_isShot)
                return false;

            if (CurrentBullet <= 0) {
                unit.Reload();
                return false;
            }

            return true;
        }

        public void SetWalkSfx() {
            int ran = Random.Range((int)UnitSfx.Run1, (int)UnitSfx.Run7);
            _sfx.PlaySfx((UnitSfx)ran);
        }

        public void SetSfx(int index) {
            _sfx.PlaySfx((UnitSfx)index);
        }
    }
}
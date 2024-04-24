using Photon.Pun;
using UnityEditor;
using UnityEngine;
using static Define;

namespace Base {
    public abstract class WeaponController : MonoBehaviourPunCallbacks, IWeapon {
        public WeaponType Type { get; protected set; }
        public int Damage { get; protected set; }
        public GameObject CreateObject { get; protected set; }
        public int CurrentBullet { get; protected set; }
        public int RemainBullet { get; protected set; }
        public int MaxBullet { get; protected set; }
        public string Name { get; protected set; }
        public Sprite WeaponIcon { get; protected set; }
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

        private UnitSfxController _sfx;

        protected Animator _animator;
        public Animator Animator => _animator;
        protected abstract void Enable();
        public PhotonView PV { get; private set; }

        public GameObject myObject { get { return gameObject; } }

        protected virtual void Awake() {
            PV = GetComponent<PhotonView>();
            if (!PV.IsMine) {
                Util.SetLayer(gameObject, LayerType.OtherHand);
                return;
            }

            _animator = GetComponent<Animator>();
        }

        protected virtual void Start() {

            

            _unit = GetComponentInParent<UnitBase>();
            PV.OwnerActorNr = _unit.PV.OwnerActorNr;
            _layerMask = (1 << (int)LayerType.Head) | (1 << (int)LayerType.Body) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();
            _sfx = _unit.GetComponent<UnitSfxController>();

        }

        private void Update() {
            if (!PV.IsMine)
                return;

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
            if (!PV.IsMine)
                return;

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

            _unit.ChangeState(UnitState.Idle);
        }

        protected virtual void DefaultShot(Vector3 angle) { }

        protected void DefaultShot(bool isHit, RaycastHit hit, UnitBase unit) {
            if (!PV.IsMine)
                return;

            if (!isHit)
                return;


            int layer = hit.collider.gameObject.layer;
            if (layer == (int)LayerType.Head &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != unit.gameObject) {
                PV.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, Damage * 3, PV.OwnerActorNr, hit.collider.GetComponentInParent<UnitBase>().PV.OwnerActorNr, true);
                //hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage * 3, unit.TargetPos, hit.collider.GetComponentInParent<UnitBase>().transform, true);
                GameObject blood = CreateEffect(_bloodEffect, hit.point);
                blood.transform.LookAt(_firePoint.position);
                if(unit.TryGetComponent<PlayerController>(out var player)) {
                    player.HeadshotEvent.Invoke();
                }
                Destroy(blood, 1f);
            } else if (layer == (int)LayerType.Body &&
                hit.collider.GetComponentInParent<UnitBase>().gameObject != unit.gameObject) {
                hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage, unit.TargetPos, hit.collider.GetComponentInParent<UnitBase>().transform, false);
                GameObject blood = CreateEffect(_bloodEffect, hit.point);
                blood.transform.LookAt(_firePoint.position);
                if (unit.TryGetComponent<PlayerController>(out var player)) {
                    player.BodyshotEvent.Invoke();
                }
                Destroy(blood, 1f);
            } else if (layer == (int)LayerType.Obstacle ||
                 layer == (int)LayerType.Wall) {
                GameObject impact = CreateEffect(_impactEffect, hit.point);
                impact.transform.LookAt(_firePoint.position);
                Destroy(impact, 1f);
            } else if (layer == (int)LayerType.Ground) {
                GameObject impact = CreateEffect(_impactEffect, hit.point);
                impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                Destroy(impact, 1f);
            }
        }

        [PunRPC]
        public void RPC_TakeDamage(int damage, int attackerHandle, int myHandle, bool headShot) {
            UnitBase attacker = Util.FindPlayerByActorNumber(attackerHandle);
            UnitBase me = Util.FindPlayerByActorNumber(myHandle);
            Debug.Log($"공격자 번호 {attacker.PV.OwnerActorNr}, 피격자 번호 {me.PV.OwnerActorNr}");
            me.TakeDamage(damage, attacker.transform, me.transform, headShot);
        }

        public virtual void Shot() {
            if (!PV.IsMine)
                return;

            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
            _isShot = true;
            CurrentBullet--;
            _ejectEffect.Play();
        }



        protected GameObject CreateEffect(GameObject go, Vector3 pos, Quaternion rot = default ) {
            if (!PV.IsMine)
                return null;

            GameObject effect = Managers.Resources.Instantiate(go, null);
            effect.transform.position = pos;
            effect.transform.rotation = rot;
            return effect;
        }

        public bool TryReload(UnitBase unit) {
            if (!PV.IsMine)
                return false;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
                return false;

            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public virtual bool TryShot(UnitBase unit) {
            if (!PV.IsMine)
                return false;

            if (unit.State == Define.UnitState.Reload ||
                unit.State == Define.UnitState.Get ||
                unit.State == UnitState.Dead)
                return false;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
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
            if (!PV.IsMine)
                return;

            int ran = Random.Range((int)UnitSfx.Run1, (int)UnitSfx.Run7);
            _sfx.PlaySfx((UnitSfx)ran);
        }

        public void SetSfx(int index) {
            if (!PV.IsMine)
                return;

            _sfx.PlaySfx((UnitSfx)index);
        }
    }
}
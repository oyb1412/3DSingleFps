using UnityEngine;
using static Define;

public class WeaponController : MonoBehaviour, IWeapon
{
    public GameObject myObject { get { return gameObject; } }
    protected int _currentBullet = 30;
    protected int _remainBullet = 30;
    protected int _maxBullet = 180;

    protected Transform _firePos;
    protected int _layerMask;
    protected Transform _firePoint;
    protected PlayerController _player;
    protected ParticleSystem _ejectEffect;

    private Animator _animator;

    public float BoundValue { get; set; }
    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
    }

    protected void Start() {
        _layerMask = (1 << (int)LayerType.Player) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground);
        _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
        _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();

    }
    public void Activation(Transform firePoint = null, PlayerController player = null) {
        if (firePoint != null && _firePoint == null) {
            _firePoint = firePoint;
        }
        if(player != null && _player == null) {
            _player = player;
        }
    }

    public void Reload() {
        if(_maxBullet >= _remainBullet) {
            _currentBullet = _remainBullet;
            _maxBullet -= _currentBullet;
        }
        else if(_maxBullet < _remainBullet) {
            _currentBullet = _remainBullet - _maxBullet;
            _maxBullet = 0;
        }

        Debug.Log($"남은 총알 {_currentBullet}");
    }
    public virtual void Shot() {
        _currentBullet--;
        _ejectEffect.Play();
        var ran = Random.Range(0, 5);
        GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
        muzzle.transform.position = _firePos.position;
        Debug.DrawRay(_firePoint.position, _firePoint.forward * 100f, Color.green, 1f);
        bool isHit = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, float.MaxValue, _layerMask);

        if (!isHit)
            return;

        int layer = hit.collider.gameObject.layer;

        if (layer == (int)LayerType.Obstacle ||
           layer == (int)LayerType.Ground) {
            GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
            impact.transform.position = hit.point;
            impact.transform.LookAt(_firePoint.position);
            Destroy(impact, 1f);
            return;
        }

        if (layer == (int)LayerType.Player) {
        }
    }
    public bool TryReload(PlayerController player) {
        if (_maxBullet <= 0)
            return false;

        if (_currentBullet == _remainBullet)
            return false;

        return true;
    }

    public bool TryShot(PlayerController player) {
        if (player.State == Define.PlayerState.Reload ||
            player.State == Define.PlayerState.Get)
            return false;

        if (_currentBullet <= 0) {
            player.Reload();
            return false;
        }

        return true;
    }

    public void SetAnimation(PlayerState anime, bool trigger) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(anime.ToString()))
            return;

        _animator.SetBool(anime.ToString(), trigger);
        _player.Model.ChangeAnimation(anime, trigger);
    }

    public void SetAnimation(PlayerState anime) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(anime.ToString()))
            return;

        _animator.SetTrigger(anime.ToString());
        _player.Model.ChangeAnimation(anime);
    }

    public void EndAnimation(string name) {
        Debug.Log($"{name}상태로 초기화");
        _animator.ResetTrigger(name);
        _player.Model.ResetTrigger(name);
        _player.ChangeState(PlayerState.Idle);
    }
}

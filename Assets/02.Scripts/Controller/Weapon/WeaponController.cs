using UnityEngine;
using static Define;

public class WeaponController : MonoBehaviour, IWeapon
{
    public GameObject myObject { get { return gameObject; } }

    protected int _currentBullet = 30;
    protected int _remainBullet = 30;
    protected int _maxBullet = 180;
    private int _layerMask;
    protected Transform _firePoint;
    protected PlayerController _player;

    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        _layerMask = (1 << (int)Define.LayerType.Player) | (1 << (int)Define.LayerType.Obstacle);
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
    }
    public virtual void Shot() {
        _currentBullet--;

        Debug.DrawRay(_firePoint.position, _firePoint.forward * 100f, Color.green, 1f);
        bool isHit = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, float.MaxValue, _layerMask);

        if (!isHit)
            return;

        if (hit.collider.gameObject.layer == (int)LayerType.Obstacle) {
            return;
        }

        if (hit.collider.gameObject.layer == (int)LayerType.Player) {
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
    }

    public void SetAnimation(PlayerState anime) {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(anime.ToString()))
            return;

        _animator.SetTrigger(anime.ToString());
    }

    public void EndAnimation(string name) {
        _animator.ResetTrigger(name);
        _player.ChangeState(PlayerState.Idle);
    }
}

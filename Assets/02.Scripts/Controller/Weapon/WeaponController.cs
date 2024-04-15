using UnityEngine;
using static Define;

public class WeaponController : MonoBehaviour, IWeapon
{
    public GameObject myObject { get { return gameObject; } }
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
    public float CrossValue { get ; set ; }
    public int Damage { get ; set ; }

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
    }

    protected void Start() {
        _layerMask = (1 << (int)LayerType.Player) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
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
        if(MaxBullet >= RemainBullet) {
            CurrentBullet = RemainBullet;
            MaxBullet -= CurrentBullet;
        }
        else if(MaxBullet < RemainBullet) {
            CurrentBullet = RemainBullet - MaxBullet;
            MaxBullet = 0;
        }

        _player.BulletEvent.Invoke(CurrentBullet, MaxBullet);
    }
    public virtual void Shot() {
        CurrentBullet--;
        _player.BulletEvent.Invoke(CurrentBullet, MaxBullet);
        _ejectEffect.Play();
        var ran = Random.Range(0, 5);
        GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
        muzzle.transform.position = _firePos.position;
        muzzle.transform.eulerAngles = _player.PlayerRotate;
        Debug.DrawRay(_firePoint.position, _firePoint.forward * 100f, Color.green, 1f);
        bool isHit = Physics.Raycast(_firePoint.position, _firePoint.forward, out var hit, float.MaxValue, _layerMask);

        if (!isHit)
            return;

        int layer = hit.collider.gameObject.layer;
        Debug.Log(hit.collider.name);
        if (layer == (int)LayerType.Obstacle ||
           layer == (int)LayerType.Wall) {
            GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
            impact.transform.position = hit.point;
            impact.transform.LookAt(_firePoint.position);
            Destroy(impact, 1f);
            return;
        }
        else if(layer == (int)LayerType.Ground) {
            GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
            impact.transform.position = hit.point;
            impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            Destroy(impact, 1f);
            return;
        }

        if (layer == (int)LayerType.Player) {
            hit.collider.GetComponent<ITakeDamage>().TakeDamage(Damage);
        }
    }
    public bool TryReload(PlayerController player) {
        if (MaxBullet <= 0)
            return false;

        if (CurrentBullet == RemainBullet)
            return false;

        return true;
    }

    public bool TryShot(PlayerController player) {
        if (player.State == Define.PlayerState.Reload ||
            player.State == Define.PlayerState.Get)
            return false;

        if (CurrentBullet <= 0) {
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

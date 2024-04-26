using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using static Define;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;
    [SerializeField] private float _searchRange;
    public int Level { get; private set; } = (int)EnemyLevel.Middle;
    public string Name { get; private set; }
    [field:SerializeField] public UnitBase TargetUnit { get; private set; }
    private Collider _collider;
    private bool _isTraceItem;
    [field: SerializeField] public bool IsShotState { get; set; }
    [SerializeField] float _viewRange;

    public Enemy.WeaponController EnemyWeapon { get { return _currentWeapon as Enemy.WeaponController; } }

    [PunRPC]
    public void EnemyInit(string name, int actorNumber, int level) {
        if (PV.OwnerActorNr != actorNumber)
            return;

        Level = level;
        this.name = $"{name} {actorNumber}";

        var uiScoreBoard = UI_Scoreboard.Instance.gameObject;
        Transform _scoreBoardTransform = Util.FindChild(uiScoreBoard, "Scoreboard", true).transform;

        UI_Scoreboard_Child child = Managers.Resources.Instantiate("UI/ScoreboardChild", _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();

        child.transform.SetParent(_scoreBoardTransform);
        child.Init(name, this, Color.gray);
        GameManager.Instance.UnitsList.Add(this);
        GameManager.Instance.BoardChild.Add(child);

        _scoreBoardTransform.parent.gameObject.SetActive(false);
    }

    protected override void Awake() {
        base.Awake();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _agent.enabled = false;
        PV.OwnerActorNr = PV.ViewID;

    }

    private void Start() {
        Invoke("StartMove", GameManager.Instance.WaitTime);
    }

    private void StartMove() {
        _agent.enabled = true;
        PV.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    private void Update() {
        if (!GameManager.Instance.InGame())
            return;

        if (State == UnitState.Dead)
            return;


        UnitRotate = transform.rotation;

        if (!TargetUnit && SearchUnit()) {
            TargetUnit = SearchUnit();
            IsShotState = true;
        }

        if (TargetUnit && !TargetUnit.IsDead() && SearchUnit()) {
            IsShotState = true;
        }

        if (TargetUnit && TargetUnit.IsDead()) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }
        //move && shotstate
        if (TargetUnit && !SearchUnit()/* && State == UnitState.Shot*/) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }

        if(IsShotState) {
            if(!TargetUnit || TargetUnit.IsDead()) {
                IsShotState = false;
                StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
                return;
            }
            if(_state != UnitState.Reload) {
                if (_currentWeapon.TryShot(this)) {
                    _agent.SetDestination(transform.position);
                    ChangeState(UnitState.Shot);
                }
            }
        }
    }

    private UnitBase SearchUnit() {

        var units = GameManager.Instance.UnitsList;
        Debug.Log($"ÃÑ À¯´Ö ¼ö{units.Count}");
        foreach(var unit in units) {
            if (unit == null || unit.IsDead()) continue;
            Vector3 dir = (unit.transform.position - transform.position).normalized;
            float product = Vector3.Dot(transform.forward, dir);
            float angle = Mathf.Cos(_viewRange * 0.5f * Mathf.Deg2Rad);
            if(product >= angle) {
                int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Wall);
                Debug.DrawRay(FirePoint.position, dir * 100f, Color.red);
                bool hit = Physics.Raycast(FirePoint.position, dir, out var target, float.MaxValue, mask);

                if (!hit)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Obstacle ||
                    target.collider.gameObject.layer == (int)LayerType.Wall)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Unit) {
                    TargetUnit = unit;
                    return TargetUnit;
                }
            }
        }
        return null;
    }

    public IEnumerator CoMove(Vector3 pos) {
        if (_agent.enabled == false)
            _agent.enabled = true;

            _agent.SetDestination(pos);
            BaseWeapon.Animator.ResetTrigger("Shot");
            _state = UnitState.Move;
            IsShotState = false;
            ChangeState(UnitState.Move);
            while (true) {
                float dir = (new Vector3(pos.x, 0f, pos.z)
                    - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

                if (TargetUnit) {
                    _agent.SetDestination(transform.position);
                    transform.LookAt(TargetUnit.TargetPos);
                    IsShotState = true;
                    StopAllCoroutines();
                    break;
                }

                if (IsDead()) {
                    _agent.SetDestination(transform.position);
                    ChangeState(UnitState.Dead);
                    StopAllCoroutines();
                    break;
                }

                if (CollideItem != null && !_isTraceItem) {
                    if (!_isTraceItem) {
                        _isTraceItem = true;
                        StartCoroutine(CoMove(CollideItem.MyTransform.position));
                        break;
                    }
                }

                if (dir < 0.2f) {
                    if (_isTraceItem && CollideItem != null) {
                        CollideItem.Pickup(this);
                        _isTraceItem = false;
                    }
                    StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
                    break;
                }
                yield return null;
            }
        
    }

    protected override void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans) {
        if (TargetUnit == null) {
            TargetUnit = attackerTrans.GetComponent<UnitBase>();
            transform.LookAt(TargetUnit.TargetPos);
            IsShotState = true;
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans, bool headShot) {
        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            ShareSfxController.instance.SetShareSfx(ShareSfx.KillSound);
            DirType dir = Util.DirectionCalculation(attackerTrans, transform);
            player.KillAndDeadEvent?.Invoke(dir, name, true, headShot);
        }
        transform.LookAt(attackerTrans);
        base.IsDeadEvent(attackerTrans, headShot);
        TargetUnit = null;
        
        _collider.enabled = false;
    }

    
    public override void Init() {
        base.Init();

        _collider.enabled = true;

        StopAllCoroutines();
        ChangeState(UnitState.Idle);
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    public override void ChangeState(UnitState state) {
        switch (state) {
            case UnitState.Idle:
                if (_state == UnitState.Dead || _state == UnitState.Shot)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                Model.Animator.SetBool("Move", true);
                BaseWeapon.Animator.SetBool("Move", true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);

                SetWeaponTriggerAnimation("Shot", PV.OwnerActorNr);
                SetModelTriggerAnimation($"Shot{BaseWeapon.Type.ToString()}", PV.OwnerActorNr);
                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);

                SetWeaponTriggerAnimation("Reload", PV.OwnerActorNr);
                SetModelTriggerAnimation("Reload", PV.OwnerActorNr);
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);

                SetWeaponTriggerAnimation("Dead", PV.OwnerActorNr);
                SetModelTriggerAnimation("Dead", PV.OwnerActorNr);
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);

                SetWeaponTriggerAnimation("Get", PV.OwnerActorNr);
                SetModelTriggerAnimation("Get", PV.OwnerActorNr);
                break;
        }
        _state = state;
    }

    protected void SetWeaponTriggerAnimation(string type) {
        PV.RPC("RPC_WeaponSetTrigger", RpcTarget.AllBuffered, type);
    }

    protected void SetModelTriggerAnimation(string type) {
        PV.RPC("RPC_ModelSetTrigger", RpcTarget.AllBuffered, type);
    }

    [PunRPC]
    public void RPC_WeaponSetTrigger(string type) {
        
        BaseWeapon.Animator.SetTrigger(type);
    }

    [PunRPC]
    public void RPC_ModelSetTrigger(string type) {
        
        Model.Animator.SetTrigger(type);
    }
}

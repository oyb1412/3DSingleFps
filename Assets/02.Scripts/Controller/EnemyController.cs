using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;
    public UnitBase TargetUnit { get; private set; }
    private Transform _movePoint;
    private Rigidbody _rigid;
    private EnemyFov _fov;
    private Collider _collider;
    private bool _isTraceItem;
    public EnemyStatus MyStatus { get { return _status as EnemyStatus; } set { _status = value; } }

    protected override void Awake() {
        base.Awake();
        _rigid = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _fov = GetComponent<EnemyFov>();
        _agent.speed = MyStatus._moveSpeed;
        _movePoint = GameObject.Find("@SpawnPoints").transform;
    }
    
    private void Start() {
        StartCoroutine(CoMove(RandomPosition()));
    }

    private void Update() {
        if (!Managers.GameManager.InGame())
            return;

        if (State == UnitState.Dead)
            return;

        if (!TargetUnit && _fov.isTracePlayer()) {
            TargetUnit = _fov.isTracePlayer();
        }

        if(TargetUnit && TargetUnit.IsDead()) {
            TargetUnit = null;
            ChangeState(UnitState.Move, false);
            StartCoroutine(CoMove(RandomPosition()));
        }

        if (TargetUnit && !_fov.isTracePlayer() && State == UnitState.Shot) {
            TargetUnit = null;
            ChangeState(UnitState.Move, false);
            StartCoroutine(CoMove(RandomPosition()));
        }
    }

    public IEnumerator CoMove(Vector3 pos) {
        _agent.SetDestination(pos);
        ChangeState(UnitState.Move, true);
        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                Debug.Log("유닛 발견");
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.transform.position);
                ChangeState(UnitState.Shot, true);
                StopAllCoroutines();
                break;
            }

            if(CollideItem != null && !_isTraceItem) {
                if (!_isTraceItem) {
                    Debug.Log("아이템 추적");
                    _isTraceItem = true;
                    StartCoroutine(CoMove(CollideItem.MyTransform.position));
                    break;
                }
            }
            if (dir < 0.2f) {
                if(_isTraceItem) {
                    Debug.Log("아이템 습득");
                    CollideItem.Pickup(this);
                    _isTraceItem = false;
                }
                Debug.Log("다음장소 추적");
                StartCoroutine(CoMove(RandomPosition()));
                break;
            }
            yield return null;
        }
    }

    private Vector3 RandomPosition() {
        return _movePoint.GetChild(Random.Range(0, _movePoint.childCount - 1)).position;
    }

    public override void TakeDamage(int damage, Transform attackerTrans, Transform myTrans) {
        base.TakeDamage(damage, attackerTrans, myTrans);
        if (_status._currentHp <= 0) {
            _collider.enabled = false;
            _rigid.isKinematic = true;
        }
    }
}

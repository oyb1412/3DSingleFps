using UnityEngine;
using static Define;

public class EnemySoundChecker : MonoBehaviour
{
    private EnemyController _enemy;

    private void Start() {
        _enemy = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider c) {
        if (!Managers.GameManager.InGame())
            return;

        if (_enemy.TargetUnit != null)
            return;

        if (c.CompareTag(TAG_UNIT)) {
            _enemy.transform.LookAt(c.transform);
            return;
        }
    }
}

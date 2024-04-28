using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EnemyItemChecker : MonoBehaviour
{
    private EnemyController _enemy;

    private void Start() {
        _enemy = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider c) {
        if (!Managers.GameManager.InGame())
            return;

        if (_enemy.CollideItem != null)
            return;

        if (!c.CompareTag(TAG_ITEM))
            return;

        if (!_enemy.IsDefaultWeapon())
            return;

       _enemy.CollideItem = c.GetComponent<IItem>();
    }
}

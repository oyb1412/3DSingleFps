using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemChecker : MonoBehaviour
{
    private EnemyController _enemy;
    private void Start() {
        _enemy = GetComponentInParent<EnemyController>();
    }

    private void Update() {
    }
    private void OnTriggerEnter(Collider c) {
        if (_enemy.CollideItem != null)
            return;

        if (!c.CompareTag("Item"))
            return;

        if (c.GetComponent<IItem>().IsMine)
            return;

        Debug.Log("1");
       _enemy.CollideItem = c.GetComponent<IItem>();
    }
}

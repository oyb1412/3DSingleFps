using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class EnemyItemChecker : MonoBehaviour
{
    private EnemyController _enemy;

    private void Start() {
        _enemy = GetComponentInParent<EnemyController>();
        //if (Managers.Scene.CurrentScene == SceneType.Port) {
        //    gameObject.SetActive(false);
        //}
    }

    private void OnTriggerEnter(Collider c) {
        if (!GameManager.Instance.InGame())
            return;

        if (_enemy.CollideItem != null)
            return;

        if (!c.CompareTag("Item"))
            return;

        if (c.GetComponent<IItem>().IsMine)
            return;

       _enemy.CollideItem = c.GetComponent<IItem>();
    }
}

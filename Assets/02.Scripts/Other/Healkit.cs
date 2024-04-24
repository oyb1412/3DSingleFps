using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healkit : MonoBehaviour
{
    private const float ITEM_ROTATE_SPEED = 0.2f;
    private const int HEALKIT_VALUE = 15;
    private const float DESTORY_TIME = 8f;

    private void Start() {
        GameObject.Destroy(gameObject, DESTORY_TIME);
    }
    private void Update() {
        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }
    private void OnTriggerEnter(Collider c) {
        if (!c.CompareTag("Unit"))
            return;

        c.GetComponent<UnitBase>().SetHp(HEALKIT_VALUE);
        if(c.TryGetComponent<PlayerController>(out var player)) {
            ShareSfxController.instance.SetShareSfx(Define.ShareSfx.Medikit);
        }
        Managers.Resources.Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour, IItem {
    private const float ITEM_ROTATE_SPEED = 0.2f;
    public Transform MyTransform { get { return transform; }  }
    public bool IsMine { get; private set; }

    public virtual void Pickup(UnitBase unit) {
        unit.CollideItem = null;
        if (gameObject == null)
            return;

        Managers.Resources.Destroy(gameObject);
    }

    public void Init(Vector3 pos, bool trigger) {
        transform.position = pos;
        IsMine = trigger;
    }

    private void Update() {
        if (!Managers.GameManager.InGame())
            return;

        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }
}

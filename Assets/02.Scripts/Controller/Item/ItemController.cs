using UnityEngine;
using static Define;

public abstract class ItemController : MonoBehaviour, IItem {
    public Transform MyTransform { get { return transform; }  }

    public virtual void Pickup(UnitBase unit) {
        Managers.Resources.Destroy(gameObject);
    }

    private void Update() {
        if (!Managers.GameManager.InGame())
            return;

        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }
}

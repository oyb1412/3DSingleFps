using UnityEngine;
using static Define;
public class Item_Healkit : ItemController
{
    private void OnEnable() {
        Managers.Instance.DestoryCoroutine(gameObject, HEALKIT_DESTORY_TIME);
    }

    private void Update() {
        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }

    public override void Pickup(UnitBase unit) {
        unit.SetHp(HEALKIT_VALUE);

        if(unit is PlayerController)
            PersonalSfxController.instance.SetShareSfx(ShareSfx.Medikit);

        base.Pickup(unit);
    }

    private void OnTriggerEnter(Collider c) {
        if (!c.CompareTag(TAG_UNIT))
            return;

        UnitBase unit = c.GetComponent<UnitBase>();
        Pickup(unit);
    }
}

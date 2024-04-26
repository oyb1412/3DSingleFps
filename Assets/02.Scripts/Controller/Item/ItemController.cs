using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour, IItem {
    private const float ITEM_ROTATE_SPEED = 0.2f;
    public Transform MyTransform { get { return transform; }  }
    public bool IsMine { get; private set; }
    public PhotonView PV { get; private set; }

    private void Awake() {
        PV = GetComponent<PhotonView>();
        IsMine = true;
    }

    public virtual void Pickup(UnitBase unit) {
        //if (!PV.IsMine)
        //    return;


        unit.CollideItem = null;

        NetworkManager.Instance.PhotonDestroy(0f, PV.ViewID);
    }

    public void Init(bool trigger) {
        IsMine = trigger;
    }

    private void Update() {
        if (!GameManager.Instance.InGame())
            return;

        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }
}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healkit : MonoBehaviour
{
    private const float ITEM_ROTATE_SPEED = 0.2f;
    private const int HEALKIT_VALUE = 15;
    private const float DESTORY_TIME = 8f;
    private Collider _col;
    public PhotonView PV { get; private set; }

    private void Awake() {
        PV = GetComponent<PhotonView>();
        _col = GetComponent<Collider>();
    }
    private void Start() {
        if (!PV.IsMine)
            return;

        NetworkManager.Instance.PhotonDestroy(DESTORY_TIME, PV.ViewID);
    }
    private void Update() {
        if (!PV.IsMine)
            return;

        transform.Rotate(0f, ITEM_ROTATE_SPEED, 0f);
    }
    private void OnTriggerEnter(Collider c) {
        if (!c.CompareTag("Unit"))
            return;

        c.GetComponent<UnitBase>().SetHp(HEALKIT_VALUE);

        _col.enabled = false;

        Debug.Log($"{c.GetComponent<UnitBase>().PV.OwnerActorNr}번 플레이어가{name}습득");
        NetworkManager.Instance.PhotonDestroy(0f, PV.ViewID);
    }
}

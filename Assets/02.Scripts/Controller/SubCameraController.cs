using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    [SerializeField]private Vector3 _defaultPos;
    [SerializeField]private float _cameraSpeed;
    [SerializeField] private Transform _playerTrans;
    public PhotonView PV { get; private set; }

    private void Start() {
    }

    private void OnEnable() {
        if (!PV.IsMine)
            return;

        transform.position = _defaultPos + _playerTrans.position;
    }

    void Update()
    {
        if (!PV.IsMine)
            return;

        if (!Managers.GameManager.InGame())
            return;
        transform.position += Vector3.up * _cameraSpeed * Time.deltaTime;
    }

    public void SetCameraView(int number) {
        PV = GetComponent<PhotonView>();
        PV.OwnerActorNr = number;
    }
}

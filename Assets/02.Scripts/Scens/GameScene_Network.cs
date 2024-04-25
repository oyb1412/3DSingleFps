using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene_Network : BaseScene
{
    private bool _start;
    public override void Clear() {
    }

    protected override void Start() {
        PhotonNetwork.SendRate = 60;

        PhotonNetwork.SerializationRate = 30;

        NetworkManager.Instance.Test(null);
    }

    private void CreatePlayer() {
        StartCoroutine(Co_CreatePlayer());
        
    }

    private void Update() {
        if (_start)
            return;

        if (PhotonNetwork.PlayerList.Length > 1) {
            _start = true;
            CreatePlayer();
        }
    }

    private IEnumerator Co_CreatePlayer() {
        PlayerController player = PhotonNetwork.Instantiate("Prefabs/Unit/Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();

        yield return new WaitUntil(() => player != null);

        GameObject ui = Managers.Resources.Instantiate("UI/@UI", null);

        foreach (Transform t in ui.transform) {
            t.GetComponent<UI_Base>().SetPlayer(player);
        }


        yield return new WaitUntil(() => ui != null);

        GameManager.Instance.SetPlayer(player);

        player.PV.RPC("PlayerInit", RpcTarget.AllBuffered, "Player", player.PV.OwnerActorNr);


        Managers.Instance.IngameInit();
    }

}

using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene_Network : BaseScene
{
    public override void Clear() {
    }

    protected override void Start() {
        PhotonNetwork.SendRate = 60;

        PhotonNetwork.SerializationRate = 30;

        NetworkManager.Instance.Test(CreatePlayer);
    }

    private void CreatePlayer() {

        for (int i = 0; i < 1; i++) {
            PlayerController player = PhotonNetwork.Instantiate("Prefabs/Unit/Player", Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
            GameObject ui = Managers.Resources.Instantiate("UI/@UI",null);
            Managers.GameManager.SetPlayer(player);
            foreach (Transform t in ui.transform) {
                t.GetComponent<UI_Base>().SetPlayer(player);
            }
        }

        Managers.Instance.IngameInit();

    }

}

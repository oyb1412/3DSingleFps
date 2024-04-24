using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override void Clear()
    {
    }

    protected override void Start() {
        base.Start();
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        GameObject ui = Managers.Resources.Instantiate("UI/@UI", null);
        foreach (Transform t in ui.transform) {
            t.GetComponent<UI_Base>().SetPlayer(player);
        }

        Managers.Scene.SetScene();
        Managers.Instance.IngameInit();
        Managers.GameManager.SetPlayer();

        BgmController.instance.SetBgm(Define.Bgm.Ingame, true);
    }
}

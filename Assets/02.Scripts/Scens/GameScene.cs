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

        Managers.Scene.SetScene();
        Managers.Instance.IngameInit();

        BgmController.instance.SetBgm(Define.Bgm.Ingame, true);
    }
}

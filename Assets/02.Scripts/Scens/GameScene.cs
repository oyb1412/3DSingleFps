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
        Managers.Instance.IngameInit();
        Managers.Scene.SetScene();

        BgmController.instance.SetBgm(Define.Bgm.Ingame, true);
    }
}

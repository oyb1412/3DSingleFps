using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : BaseScene
{
    public override void Clear() {
    }

    protected override void Start() {
        base.Start();
        Managers.Instance.StartInit();
        Managers.Scene.SetScene();

        
    }
}

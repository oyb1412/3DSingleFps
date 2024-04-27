using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType { get; protected set; } = Define.SceneType.None;

    public abstract void Clear();

    private void Awake() {
        Managers.Init();
    }

    public void Init()
    {
        var obj = GameObject.FindFirstObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resources.Instantiate("UI/EventSystem", null).name = "@EventSystem";
    }

    protected virtual void Start()
    {
        Init();
    }

}

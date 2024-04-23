using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    public Define.SceneType CurrentScene = Define.SceneType.Startup;
    public BaseScene CurrentSceneManager => GameObject.FindObjectOfType<BaseScene>();

    private UI_Fade _fade;

    public void Init() {
        if(_fade == null)
            _fade = GameObject.Find("UI_Fade").GetComponent<UI_Fade>();
    }

    public void LoadScene(Define.SceneType type) {
        var tween = _fade.SetFade(true);
        if (type == Define.SceneType.Exit) {
            tween.OnComplete(QuitGame);
        } else
            tween.OnComplete(() => DoNextScene(type));
    }

    public void SetScene() {
        _fade.SetFade(false);
    }

    private void QuitGame() {
        Util.QuitGame();
    }

    private void DoNextScene(Define.SceneType type) {
        SceneManager.LoadScene(type.ToString());
        CurrentScene = type;
    }

}

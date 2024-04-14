using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEX
{
    public BaseScene CurrentScene => GameObject.FindFirstObjectByType(typeof(BaseScene)).GetComponent<BaseScene>();

    public void LoadScene(Define.SceneType type)
    {
        SceneManager.LoadScene(GetSceneName(type));
    }

    private string GetSceneName(Define.SceneType type)
    {
       return Enum.GetName(typeof(Define.SceneType), type);
    }
}

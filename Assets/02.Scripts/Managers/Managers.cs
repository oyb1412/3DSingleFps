using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance {
        get {
            Init();
            return _instance;
        }
    }

    private PoolManager _pool = new PoolManager();
    private InputManager _input = new InputManager();
    private ResourcesManager _resources = new ResourcesManager();
    private SceneManagerEX _scene = new SceneManagerEX();

    public static PoolManager Pool => _instance._pool;
    public static SceneManagerEX Scene => _instance._scene;
    public static InputManager Input => Instance._input;
    public static ResourcesManager Resources => Instance._resources;
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Input.OnUpdate();
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject managers = GameObject.Find("@Managers");
            if (managers == null)
            {
                managers = new GameObject("@Managers");
                managers.AddComponent<Managers>();
            }
            
            DontDestroyOnLoad(managers);
            _instance = managers.GetComponent<Managers>();
            
            Pool.Init();
        }
    }
}

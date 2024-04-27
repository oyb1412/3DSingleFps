using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    public static Managers Instance {
        get {
            return _instance;
        }
    }
    private GameManager _gameManager = new GameManager();
    private RespawnManager _respawnManager = new RespawnManager();
    private PoolManager _pool = new PoolManager();
    private InputManager _input = new InputManager();
    private ResourcesManager _resources = new ResourcesManager();
    private SceneManagerEX _scene = new SceneManagerEX();

    public static GameManager GameManager => _instance._gameManager;
    public static RespawnManager RespawnManager => _instance._respawnManager;
    public static PoolManager Pool => _instance._pool;
    public static SceneManagerEX Scene => _instance._scene;
    public static InputManager Input => Instance._input;
    public static ResourcesManager Resources => Instance._resources;
    
    private void Awake()
    {
        Init();
    }

    public void StartInit() {
        Scene.Init();
    }

    public void IngameInit() {
        RespawnManager.Init();
        GameManager.Init();
    }

    public void Ingameclear() {
        RespawnManager.Clear();
        GameManager.Clear();
    }

    private void Start() {
    }

    private void Update()
    {
        Input.OnUpdate();
        GameManager.Update();
    }

    public static void Init()
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

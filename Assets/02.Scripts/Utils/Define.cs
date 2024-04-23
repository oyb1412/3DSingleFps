using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public enum GameState {
        None,
        WaitFight,
        StartFight,
        Menu,
        Setting,
        Gameover,
    }
    public enum LayerType {
        Ground = 6,
        Player = 9,
        Obstacle,
        Wall,
        Item,
        Enemy,
        Head = 15,
        Body,
        Unit,
    }

    public enum DirType {
        Left,
        Back,
        Right,
        Front,
    }
    public enum WeaponType {
        Pistol,
        Rifle,
        Shotgun,
        Count,
    }

    public enum UnitState {
        Idle,
        Move,
        Jump,
        Shot,
        Reload,
        Dead,
        Get,
        Run,
    }
    public enum MouseEventType
    {
        None,
        LeftMouseDown,
        RightMouseDown,
        LeftMouseUp,
        RightMouseUp,
        LeftMouse,
        RightMouse,
        Enter,
        Drag,
    }

    public enum EnemyLevel {
        Low,
        Middle,
        High
    }

    public enum UnitSfx {
        GetPistol,
        GetRifle,
        GetShotgun,
        Run1,
        Run2,
        Run3,
        Run4,
        Run5,
        Run6,
        Run7,
        Jump1,
        Jump2,
        Jump3,
        RifleReload,
        PistolReload,
        ShotgunReload,
        RifleShot,
        PistolShot,
        ShotgunShot,
        Dead,
        Count,
    }

    public enum Bgm {
        Startup,
        Ingame,
        Count,
    }

    public enum ShareSfx {
        Dominate,
        Rampage,
        Three,
        Two,
        One,
        Fight,
        Gameover,
        KillSound,
        Button,
        Count,
    }

    public enum SceneType
    {
        None,
        Startup,
        WareHouse,
        Port,
        Exit,
    }
}

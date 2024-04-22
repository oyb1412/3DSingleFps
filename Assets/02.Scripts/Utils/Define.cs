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
        Obstacle = 10,
        Player = 9,
        Ground = 6,
        Wall = 11,
        Item = 12,
        Enemy = 13,
        Unit = 14,
        Head,
        Body,
        RightArm,
        LeftArm,
        RightLeg,
        LeftLeg,
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

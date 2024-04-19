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

    public enum SceneType
    {
        None,
        InGame,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public enum LayerType {
        Obstacle = 10,
        Player = 9,
        Ground = 6,
        Wall = 11,
    }
    public enum WeaponType {
        Pistol,
        Rifle,
        Shotgun,
        Count,
    }

    public enum PlayerState {
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

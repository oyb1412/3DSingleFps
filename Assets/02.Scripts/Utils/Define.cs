using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    #region string
    public static readonly string PLAYERPREFS_ENEMYNUMBER = "EnemyNumber";
    public static readonly string PLAYERPREFS_ENEMYLEVEL = "EnemyLevel";
    public static readonly string PLAYERPREFS_TIMELIMIT = "TimeLimit";
    public static readonly string PLAYERPREFS_RESPAWNTIME = "RespawnTime";
    public static readonly string PLAYERPREFS_KILLLIMIT = "KillLimit";

    public static readonly string MENT_KILL = "Killer the {0} ({1}, {2})";
    public static readonly string MENT_DEAD = "Killed by that {0} ({1}, {2})";

    public static readonly string MENT_PLAYERS = "PLAYERS ({0})";

    public static readonly string WINNERLOGO = "Winner is {0}";

    public static readonly string LOGO_DOUBLEKILL = "DOUBLE KILL!!";
    public static readonly string LOGO_TRIPLEKILL = "TRIPLE KILL!!!";
    public static readonly string LOGO_START = "START!";

    public static readonly string LOGO_HEADSHOT = "HeadShot";
    public static readonly string LOGO_BODYSHOT = "BodyShot";

    public static readonly string TAG_UNIT = "Unit";
    public static readonly string TAG_OBSTACLE = "Obstacle";
    public static readonly string TAG_WALL = "Wall";
    public static readonly string TAG_ITEM = "Item";
    public static readonly string NAME_RESTARTBG = "RestartBG";
    public static readonly string NAME_SFXPLAYER = "SfxPlayer";
    public static readonly string NAME_PLAYER = "Player";
    public static readonly string NAME_FIREPOS = "FirePos";
    public static readonly string NAME_EJECT = "Eject";
    public static readonly string NAME_PISTOL = "Pistol";
    public static readonly string NAME_RIFLE = "Rifle";
    public static readonly string NAME_SHOTGUN = "Shotgun";
    public static readonly string NAME_AIMFIREPOS = "AimFirePos";
    public static readonly string NAME_WEAPON = "Weapon";
    public static readonly string NAME_WEAPONS = "Weapons";
    public static readonly string NAME_FIREPOINT = "FirePoint";
    public static readonly string NAME_TARGETPOS = "TargetPos";
    public static readonly string NAME_KILLFEEDS = "KillFeeds";
    public static readonly string NAME_UI_FAED = "UI_Fade";
    public static readonly string NAME_UI_SCOREBOARD = "UI_Scoreboard";
    public static readonly string NAME_UI_SCOREBOARDCHILD = "UI/ScoreboardChild";
    public static readonly string NAME_SCOREBOARD = "Scoreboard";
    public static readonly string NAME_MANAGERS = "@Managers";
    public static readonly string NAME_SPAWNPOINT = "@SpawnPoints";

    public static readonly string ANIMATOR_PARAMETER_RELOAD = "Reload";
    public static readonly string ANIMATOR_PARAMETER_DEAD = "Dead";
    public static readonly string ANIMATOR_PARAMETER_RESET = "Reset";
    public static readonly string ANIMATOR_PARAMETER_SHOT = "Shot";
    public static readonly string ANIMATOR_PARAMETER_MOVE = "Move";

    public static readonly string[] WAREHOUSE_OPTIONS = new string[] { "5", "6", "7", "8" };
    public static readonly string[] PORT_OPTIONS = new string[] { "8", "9", "10", "11" };

    public static readonly string[] ENEMY_NAME = { "James", "Aaron", "Peyton", "London", "Daniel", "Aiden", "Jackson", "Lucas", "Samuel", "Luke", "Kim" };

    public static readonly string UI_PATH = "UI/@UI";

    public static readonly string ENEMY_PATH = "Unit/Enemy";

    public static readonly string ITME_HEALKIT_PATH = "Prefabs/Item/Healkit";
    public static readonly string UI_KILLFEED_PATH = "Prefabs/UI/KillFeed";

    public static readonly string PISTOL_ICON_PATH = "Texture/PistolIcon";
    public static readonly string PISTOL_OBJECT_PATH = "Prefabs/Item/Pistol"; 
    
    public static readonly string RIFLE_ICON_PATH = "Texture/RifleIcon";
    public static readonly string RIFLE_OBJECT_PATH = "Prefabs/Item/Rifle";   
    
    public static readonly string SHOTGUN_ICON_PATH = "Texture/ShotgunIcon";
    public static readonly string SHOTGUN_OBJECT_PATH = "Prefabs/Item/Shotgun";

    public static readonly string BULLET_OBJECT_PATH = "Prefabs/Other/Bullet";

    public static readonly string BLOOD_EFFECT_PATH = "Prefabs/Effect/Blood";
    public static readonly string IMPACT_EFFECT_PATH = "Prefabs/Effect/Impact";
    public static readonly string[] MUZZEL_EFFECT_PATH = { "Prefabs/Effect/muzzelFlash0", "Prefabs/Effect/muzzelFlash1", "Prefabs/Effect/muzzelFlash2", 
        "Prefabs/Effect/muzzelFlash3" ,"Prefabs/Effect/muzzelFlash4"};

    #endregion

    #region float
    public static readonly float WARDHOUSE_ALLOW_RESPAWN_RANGE = 10f;
    public static readonly float PORT_ALLOW_RESPAWN_RANGE = 20f;

    public static readonly float[] HURTIMAGE_ROTATE = { 90f, 180f, 270f, 0f };

    public static readonly float CROSSHAIR_LIMIT_VALUE = 100f;
    public static readonly float CROSSHAIR_TIME = 0.1f;

    public static readonly float FADE_TIME = 1f;

    public static readonly float DEFAULT_GAMETIME = 60f;
    public static readonly float DEFAULT_RESPAWNTIME = 3f;

    public static readonly float DEFAULT_GAMESTARTTIME = 3f;

    public static readonly float PISTOL_VECTICALBOUND_VALUE = 1.8f;
    public static readonly float PISTOL_HORIZONTALBOUND_VALUE = 0.5f;
    public static readonly float PISTOL_CROSSHAIR_VALUE = 100f;
    public static readonly float PISTOL_SHOT_DELAY = 0.4f;

    public static readonly float RIFLE_VECTICALBOUND_VALUE = 1.5f;
    public static readonly float RIFLE_HORIZONTALBOUND_VALUE = 0.5f;
    public static readonly float RIFLE_CROSSHAIR_VALUE = 100f;
    public static readonly float RIFLE_SHOT_DELAY = 0.1f;

    public static readonly float SHOTGUN_VECTICALBOUND_VALUE = 4f;
    public static readonly float SHOTGUN_HORIZONTALBOUND_VALUE = 1f;
    public static readonly float SHOTGUN_CROSSHAIR_VALUE = 350f;
    public static readonly float SHOTGUN_ANGLE = 3f;
    public static readonly float SHOTGUN_SHOT_DELAY = 0.8f;

    public static readonly float MAINCAMERA_FADETIME = 0.3f;
    public static readonly float SUBCAMERA_MOVESPEED = 1f;

    public static readonly float ITEM_ROTATE_SPEED = 0.5f;
    public static readonly float HEALKIT_DESTORY_TIME = 5f;
    public static readonly float DEFAULT_VOLUME = 0.5f;
    public static readonly float SOUND_2DMODE = 0f;
    public static readonly float SOUND_3DMODE = 1f;

    public static readonly float SOUND_UNIT_3D_MINDISTANCE = 3f;
    public static readonly float SOUND_UNIT_3D_MAXDISTANCE = 15f;

    public static readonly float SOUND_BULLET_3D_MINDISTANCE = .5f;
    public static readonly float SOUND_BULLET_3D_MAXDISTANCE = 3f;

    public static readonly float EFFECT_DESTORY_TIME = 1f;

    public static readonly float GROUND_EULERANGLES = -90f;

    public static readonly float ENEMY_DEFAULT_ATTACK_CHANCE = 50f;
    public static readonly float ENEMY_HIGH_ATTACK_CHANCE = 75f;
    public static readonly float ENEMY_LOW_ATTACK_CHANCE = 40f;
    public static readonly float ENEMY_DEFAULT_VIEWRANGE = 90f;

    public static readonly float ENEMY_ATTACK_CHANCE_MINUS_RANGE = 5f;
    public static readonly float ENEMY_MOVE_ALLOW_RANGE = 0.2f;
    public static readonly float ENEMY_ATTACK_CHANCE_MINUS = 5f;
    public static readonly float ENEMY_ROTATE_SPEED = 30f;

    public static readonly float PLAYER_LIMIT_ROTATE_UP = -40f;
    public static readonly float PLAYER_LIMIT_ROTATE_DOWN = 20f;
    public static readonly float PLAYER_CONTINUE_KILL_TIME = 3f;
    public static readonly float PLAYER_INVINCIBILITY_TIME = 2f;

    public static readonly float PLAYER_GRAVITY = -9.81f;
    public static readonly float PLAYER_DEFAULT_SENSITYVY = 5f;

    public static readonly float PLAYER_DEFAULT_ROTATE_SPEED = 200f;
    public static readonly float PLAYER_DEFAULT_JUMP_VALUE = 1.5f;
    public static readonly float PLAYER_DEFAULT_BOUND_TIME = 0.1f;
    public static readonly float PLAYER_DEFAULT_RUN_SPEED = 3f;

    public static readonly float PLAYER_SHOT_RANDOMBOUND_VALUE = 0.2f;
    public static readonly float PLAYER_FORWARDCHECK_LENTHS = 2f;
    public static readonly float PLAYER_GROUNDCHECK_LENTHS = 1.3f;
    public static readonly float UNIT_DEFAULT_MOVESPEED = 5f;

    public static readonly float EFFECT_MUZZLE_DESTROY_TIME = 0.05f;

    #endregion

    #region int
    public static readonly int OUTLINE_NUMBER = 6;

    public static readonly int PISTOL_DEFAULT_BULLET = 12;
    public static readonly int PISTOL_MAX_BULLET = 60;
    public static readonly int PISTOL_DAMAGE = 34;
    public static readonly int PISTOL_CAMERAVIEW = 30;

    public static readonly int RIFLE_DEFAULT_BULLET = 30;
    public static readonly int RIFLE_MAX_BULLET = 150;
    public static readonly int RIFLE_DAMAGE = 23;
    public static readonly int RIFLE_CAMERAVIEW = 30;


    public static readonly int SHOTGUN_DEFAULT_BULLET = 6;
    public static readonly int SHOTGUN_MAX_BULLET = 30;
    public static readonly int SHOTGUN_DAMAGE = 27;
    public static readonly int SHOTGUN_CAMERAVIEW = 35;
    public static readonly int SHOTGUN_PALLET_NUMBER = 8;


    public static readonly int HEALKIT_VALUE = 15;
    public static readonly int HEADSHOT_VALUE = 2;


    public static readonly int UNIT_DEFAULT_HP = 100;

    #endregion
    #region Vector3
    public static readonly Vector3 SUBCAMERA_DEFAULT_POSITION = new Vector3(0f, 3.5f, 0f);
    public static readonly Vector3 AIM_CAMERA_POSITION = new Vector3(0f, 1.71f, -0.12f);

    public static readonly Vector3 UI_ENTER_SIZE = new Vector3(.95f, .95f, .95f);

    public static readonly Vector3 UI_SYSTEMSETTING_SIZE = new Vector3(1.05f, 1.05f, 1.05f);

    public static readonly Color UI_ENTER_COLOR = new Color(0f, 0f, 0f, .5f);
    public static readonly Color UI_START_COLOR = new Color(.6f, .8f, 1f, 1f);

    #endregion
    public enum GameState {
        None,
        WaitFight,
        StartFight,
        Menu,
        Setting,
        Gameover,
    }

    public enum EnemyState {
        None,
        Idle,
        Patrol,
        Search,
    }
    public enum LayerType {
        Ground = 6,
        Player = 9,
        Obstacle,
        Wall,
        Item,
        Enemy,
        OtherModel,
        Head = 15,
        Body,
        Unit,
        OtherHand,
    }

    public enum OutlineColorType {
        Red,
        Yellow,
        Green,
    }

    public enum CrosshairType {
        Line,
        Point,
        PointAndLine,
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

    public enum PersonalSfx {
        Dominate,
        Rampage,
        Three,
        Two,
        One,
        Fight,
        Gameover,
        KillSound,
        Button,
        Hurt,
        Medikit,
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

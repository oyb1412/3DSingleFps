# [C# & UNITY] Combat Quarter

## **핵심 기술**

### ・FSM(유한 상태 머신) 시스템

**🤔WHY?**

AI의 복잡한 행동들을 모두 AI쪽에서 일관성없이 제어해, 조건문이 길어지고 행동이 추가될 때 마다 조건문이 추가되는 등 가시성, 유지보수 모두 최악인 구조를 탈피하기 위해 사용했습니다.

**🤔HOW?**

 관련 코드

- EnemyStateMachine
    
    ```csharp
    using static Define;
    
    public class EnemyStateMachine {
    public EnemyStateMachine(EnemyController enemy) {
    	_enemy = enemy;
    	ChangeState(EnemyState.Idle); // FSM이 생성되면, 자동으로 IDLE상태로 돌입.
    }
    
    private EnemyState _currentEnemyState = EnemyState.None; 
    private EnemyController _enemy;
    
    public void ChangeState(EnemyState state) {
        if (_currentEnemyState == state) // 현재 상태와, 변경할 상태가 같다면 상태 변경을 취소한다.
            return;
    
        switch(state) { // 상태 변경 시, 자동으로 정의해둔 행동을 실행.
            case EnemyState.Idle:
                _enemy.EnemyStart();
                break;
            case EnemyState.Patrol:
                _enemy.StartPatrol();
                break;
            case EnemyState.Search:
                _enemy.SearchUnit(true);
                break;
        }
        _currentEnemyState = state;
    }
    
    public void Update() {
        if (!Managers.GameManager.InGame())
            return;
    
        if (_enemy.State == UnitState.Dead)
            return;
    
        switch (_currentEnemyState) { // AI의 현재 상태에 맞는 행동을 실행.
            case EnemyState.Patrol:
                _enemy.Patrol();
                break;
            case EnemyState.Search:
                _enemy.Attack();
                break;
        }
    }
    ```
    
- EnemyController
    
    ```csharp
    public class EnemyController : UnitBase, ITakeDamage {
        private EnemyStateMachine _stateMachine; //FSM을 컴포넌트 패턴으로 취급
        
        private void Start() {
            _stateMachine = new EnemyStateMachine(this); //AI가 생성되면, FSM 가동
        }
        
        public void EnemyStart() {
            Invoke("StartMove", Managers.GameManager.WaitTime); //FSM에 의해 호출
        }
    
        private void StartMove() {  //Invoke에 의해 호출
            _agent.enabled = true;
            _stateMachine.ChangeState(EnemyState.Patrol); 
        }
        
        private void Update() {
            _stateMachine.Update();  //현재 AI의 상태에 알맞는 행동 호출
        }
    ```
    

**🤓Result!**

 AI의 행동을 제어하는 머신을 컴포넌트 패턴으로 취급하여 가시성 및 유지보수 용이성 확보


### ・Unity의 충돌 감지 시스템을 이용하지 않고 AI의 시야내 적 감지 시스템

**🤔WHY?**

  Collider을 사용한 유니티의 충돌 감지 시스템을 이용해 AI의 적 감지 시스템을 구현했지만, 비교적으로 높은 비용을 지니고 있는 충돌 감지 시스템에 의해 퍼포먼스가 하락하는 문제가 발생하였고, 조금 더 최적화를 하기 위해 사용했습니다.

**🤔HOW?**

 관련 코드

- EnemyController
    
    ```csharp
    private UnitBase SearchUnit() {
            var units = Managers.GameManager.UnitsList; // 모든 유닛의 정보를 저장.
            foreach(var unit in units) {
                    if (unit.IsDead()) continue; // 유닛이 사망한 상태면, 다음 유닛을 검색.
                Vector3 dir = (unit.transform.position - transform.position).normalized;
                float product = Vector3.Dot(transform.forward, dir); //방향벡터와 대상을 가르키는 벡터를 내적
                float angle = Mathf.Cos(_viewRange * 0.5f * Mathf.Deg2Rad); // 시야각의 코사인 값 도출
                if(product >= angle) { // 코사인 값이 내적 값보다 적다면, 즉 적이 이 유닛의 시야각 내에 있다면
                    int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Wall);
                    Debug.DrawRay(FirePoint.position, dir * float.MaxValue, Color.red);
                    bool hit = Physics.Raycast(FirePoint.position, dir, out var target, float.MaxValue, mask);
    
                    if (!hit)
                        continue;
    
    								//시야 내에 적이 존재해도, 다른 물체에 가로막힌 상태라면 발견하지 못함
                    if (target.collider.gameObject.layer == (int)LayerType.Obstacle ||
                        target.collider.gameObject.layer == (int)LayerType.Wall)
                        continue;
    
                    if (target.collider.gameObject.layer == (int)LayerType.Unit) {
                        return unit;
                    }
                }
            }
            return null;
        }
    ```
    

**🤓Result!**

  유니티의 충돌 감지 시스템이 아닌, 내적을 이용한 데이터의 계산으로 AI의 시야내 적 감지 시스템을 구현해, 보다 높은 퍼포먼스를 유지할 수 있게 되었습니다.
  

### ・옵저버 패턴을 이용한 UI 시스템

**🤔WHY?**

 데이터의 변경이 없음에도 주기적으로 UI에 데이터를 동기화해, 필요 없는 작업이 지속적으로 반복되어 결과적으로 퍼포먼스가 하락되었기 때문에, 최적화를 위해 사용했습니다.

**🤔HOW?**

 관련 코드

- PlayerController
    
    ```csharp
    public class PlayerController : UnitBase, ITakeDamage
    {
    	#region Event // 특정 상황에 호출되어야 하는 이벤트들을 정의
        public Action ShotEvent;
        public Action<float> CrossValueEvent;
        public Action<int, int, int> HpEvent;
        public Action<int, int, int> BulletEvent;
        public Action<Player.WeaponController> ChangeEvent;
        public Action<Transform, Transform> HurtEvent;
        public Action DeadEvent;
        public Action<int> KillEvent;
        public Action RespawnEvent;
        public Action<bool> HurtShotEvent;
        public Action<bool> ShowScoreboardEvent;
        public Action ShowMenuEvent;
        public Action ShowSettingEvent;
        public Action<bool> CollideItemEvent;
        public Action<bool> SetAimEvent;
        public Action<int> ChangeCrosshairEvent;
        public Action<DirType, string, bool, bool> ShowKillAndDeadTextEvent;
        #endregion
        
        public override void ChangeWeapon(WeaponType type) {
            
            base.ChangeWeapon(type);
            CrossValueEvent.Invoke(CurrentWeapon.CrossValue); // 특정 행동에 맞춰 이벤트 발동
            ChangeEvent.Invoke(CurrentWeapon); // 특정 행동에 맞춰 이벤트 발동
        }
     }
    ```
    
- UI_Base
    
    ```csharp
    public class UI_Base : MonoBehaviour {
        protected PlayerController _player; //자식 클래스에서 사용할 수 있는 플레이어 변수 선언
        public void SetPlayer(PlayerController playerController) {
            _player = playerController; //플레이어를 생성함과 동시에 플레이어 변수에 할당
        }
        private void Start() {
            Init();
        }
        protected virtual void Init() { }
    }
    ```
    
- UI_
    
    ```csharp
    public class UI_Crosshair : UI_Base
    {
    		protected override void Init() {
            base.Init();
            //부모 오브젝트에서 할당한 플레이어 변수에 이벤트 구독
            _player.CrossValueEvent -= SetCross; // 모든 UI 컴포넌트에서 이벤트 구독
            _player.CrossValueEvent += SetCross;
        }
     }
    ```
    

**🤓Result!**

게임이 시작될 때, 플레이어의 이벤트에 UI기능을 구독시켜, 반복적인 UI의 Update가 아닌 데이터의 변경이 발생했을 때만 UI를 Update시켜, 퍼포먼스가 상승했습니다.


### ・컴포넌트 패턴과 전략 패턴을 이용한 무기 관리 시스템

**🤔WHY?**

무기의 종류가 많아지고 각 무기를 각각 구현해 중복된 코드를 작성하는 일이 잦아지고, 무기의 종류가 늘어나거나 줄어들 때 마다 직접적으로 플레이어 쪽 코드의 수정이 필요하게 되어 유지보수가  굉장히 힘든 문제를 해결하기 위해 사용했습니다.

**🤔HOW?**

 관련 코드

- UnitBase
    
    ```csharp
    public abstract class UnitBase : MonoBehaviour
    {
        protected IWeapon _currentWeapon; // 무기 선언
        
        public virtual void Reload() {
            if (_currentWeapon.TryReload(this)) // 무기의 종류와 상관없이, 추상화된 행동만을 호출한다.
                ChangeState(UnitState.Reload);
        }
        
         public virtual void ChangeWeapon(WeaponType type) { // 무기의 변경 
         DropWeapon();
         _weaponList[(int)type].myObject.SetActive(true);
         _currentWeapon = _weaponList[(int)type]; 
         _currentWeapon.Activation(_firePoint, this);
         ChangeState(UnitState.Get);
         foreach (var item in _weaponList) {
             if (_currentWeapon != item)
                 item.myObject.SetActive(false);
         }
         CollideItem = null;
         Model.ChangeWeapon(type);
     }
    ```
    
- IWeapon
    
    ```csharp
    using UnityEngine;
    
    public interface IWeapon
    {
        GameObject myObject { get; }
        void Activation(Transform firePoint = null, UnitBase unit = null);
        void Shot(); // 각 무기들의 공통된 기능들을 추상화
        void Reload();
        bool TryReload(UnitBase unit);
        bool TryShot(UnitBase unit);
    }
    
    ```
    

**🤓Result!**

무기의 공통된 기능들을 추상화해, 플레이어는 추상화된 무기의 기능을 호출하는 것 만으로 실질적인 무기의 기능을 사용할 수 있게 되어, 무기의 기능에 변화가 생겨도 플레이어 쪽 코드의 수정이 불필요하게 되어 유지보수가 보다 쉬워졌습니다.


### ・애니메이터와 파라미터를 이용한 유닛 애니메이션 시스템

**🤔WHY?**

Play() 등 단순한 애니메이션 호출 메서드로 원할 때 애니메이션을 호출할 수는 있었지만, 애니메이션이 자연스럽게 이어지는 것이 아닌 뚝뚝 끊기는 연출이 반복되는 문제를 해결하기 위해 사용했습니다.

**🤔HOW?**

 관련 이미지 및 코드

- Unit Animator
    
    ![animator.png](https://prod-files-secure.s3.us-west-2.amazonaws.com/7becc665-a2c0-47ed-8a12-3582bd47e71e/3608a2ee-31e7-4256-8bee-0debc728392e/animator.png)
    
- EnemyController
    
    ```csharp
        //각각 bool, trigger 전환을 관리
        private void SetAnimationBool(string parameter, bool trigger) {
            Model.Animator.SetBool(parameter, trigger);
            BaseWeapon.Animator.SetBool(parameter, trigger);
        }
    
        private void SetAnimationTrigger(string parameter) {
            Model.Animator.SetTrigger(parameter);
            BaseWeapon.Animator.SetTrigger(parameter);
        }
    ```
    

**🤓Result!**

애니메이션을 단발성으로 실행하는 것이 아닌, 파라미터의 상태에 맞게 자연스럽게 애니메이션이 전환되도록 변경, 뚝뚝 끊기는 애니메이션이 아닌 자연스러운 전환을 연출할 수 있었습니다.


### ・씬 전환 페이드 시스템

**🤔WHY?**

씬 전환시 아무 연출없이 즉각적으로 화면이 전환되어 화면이 갈아끼워지는듯한 느낌을 받는다는 피드백을 받아, 보다 극적인 연출을 위해 사용하였습니다.

**🤔HOW?**

 관련 코드

- UI_Fade
    
    ```csharp
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using static Define;
    
    public class UI_Fade : MonoBehaviour {
        public static UI_Fade Instance; //어디에서든 호출할 수 있게 싱글턴 객체로 관리
        private Image _fadeImage;
    
        private void Awake() {
            if(Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if( Instance != this) {
                Destroy(gameObject);
            }
            _fadeImage = GetComponentInChildren<Image>();
            _fadeImage.color = Color.black;
    
        }
    
    		//Scene이 시작될 떄, false매개변수로 호출된다.
        public Tween SetFade(bool trigger) { 
            if (trigger) {
                return _fadeImage.DOFade(1f, FADE_TIME);
            } else
                return _fadeImage.DOFade(0, FADE_TIME);
        }
    }
    ```
    
- SceneManagerEX
    
    ```csharp
    public class SceneManagerEX
    {
        
        public void LoadScene(SceneType type) {
            var tween = _fade.SetFade(true);
            if (type == SceneType.Exit) {
                tween.OnComplete(QuitGame);
            } else {
            //시작씬으로 돌아가는 경우, 모든 정보를 초기화 후 씬 이동
            //tweening의 콜백을 이용해, 페이드 완료 후 씬 이동
                if(type == SceneType.Startup) {
                    tween.OnComplete(() => ClearLoadScene(type));
                } else {
                    tween.OnComplete(() => DoNextScene(type));
                }
            }
        }
    
        private void ClearLoadScene(SceneType type) {
            Managers.Instance.Ingameclear();
            DoNextScene(type);
        }
    
    		// 모든 씬이 시작될 때 호출
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
    ```
    

**🤓Result!**

  씬 전환 시 즉각적인 전환이 아닌, 화면이 가려지고 씬이 전환되고 화면이 밝아지고 게임이 시작되는 등 페이드 연출을 추가해, 사용자가 어색한 느낌을 받지 않도록 하였습니다.
  

### ・BGM, Personal SFX, 3D SFX 사운드 시스템

**🤔WHY?**

모든 사운드가 리스너와의 거리에 관계없이 어디에서든 동일하게 들려, 사운드에서 오는 게임의 정보를 획득하는것이 불가능함과 동시다발적으로 무분별하게 들리는 사운드로 게임에 몰입할 수 없는 문제가 발생했기에 이를 타개하고자 사용했습니다.

**🤔HOW?**

 관련 코드

- BgmController
    
    ```csharp
    using UnityEngine;
    using static Define;
    public class BgmController : MonoBehaviour
    {
        public static BgmController instance; //어디서든 호출할 수 있게 싱글턴 객체로 관리
        [SerializeField] private AudioClip[] _clips; // 출력할 오디오 클립들
        private AudioSource _sources; 
        private void Awake() {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }else {
                Managers.Resources.Destroy(gameObject);
            }
            BgmInit();
        }
    
        public void ChangeVolume(float volume) {
            _sources.volume = volume;
        }
    
        private void BgmInit() { // 오디오 소스를 BGM환경에 맞게 설정
            _sources = new AudioSource();
            _sources = gameObject.AddComponent<AudioSource>();
            _sources.clip = _clips[(int)Bgm.Startup];
            _sources.loop = true;
            _sources.playOnAwake = false;
            _sources.volume = DEFAULT_VOLUME;
            _sources.spatialBlend = 0.0f;
        }
    
        public void SetBgm(Define.Bgm type, bool trigger) { // BGM출력 or 정지
            _sources.Stop();
    
            if (trigger) {
                _sources.clip = _clips[(int)type];
                _sources.Play();
            }
        }
    }
    ```
    
- PersonalSfxController
    
    ```csharp
    using UnityEngine;
    using static Define;
    
    public class PersonalSfxController : MonoBehaviour
    {
        public static PersonalSfxController instance; //어디서든 호출할 수 있게 싱글턴 객체로 관리
        private AudioSource[] _audioSource; // 동시에 여러 클립을 재생할 수 있게 다수의 오디오소스 이용
        [SerializeField] private AudioClip[] _audioClips; // 출력할 오디오 클립들
        private const int SFX_CHANNELS = 5; // 동시에 재생 가능한 사운드의 제한 수
        private void Awake() {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }
            ShareSfxInit();
        }
    
        public void ChangeVolume(float volume) {
            foreach(var t in _audioSource) {
                t.volume = volume * DEFAULT_VOLUME;
            }
        }
    
        protected void ShareSfxInit() { // 오디오 소스를 거리에 관계받지 않는 2D모드로 설정
            GameObject sfxObject = new GameObject(NAME_SFXPLAYER); 
            sfxObject.transform.parent = transform;
            sfxObject.transform.position = transform.position;
            _audioSource = new AudioSource[SFX_CHANNELS]; 
    
            for (int i = 0; i < _audioSource.Length; i++) {
                _audioSource[i] = sfxObject.AddComponent<AudioSource>();  
                _audioSource[i].playOnAwake = false;  
                _audioSource[i].volume = DEFAULT_VOLUME;  
                _audioSource[i].spatialBlend = SOUND_2DMODE;
                _audioSource[i].loop = false;
            }
        }
        public void SetShareSfx(ShareSfx type) { // 사운드 출력
            for (int i = 0; i < _audioSource.Length; i++) {
                if (_audioSource[i].isPlaying)  
                    continue;
    
                _audioSource[i].clip = _audioClips[(int)type];  
                _audioSource[i].Play();
                break;
            }
        }
    }
    ```
    
- UnitSfxController
    
    ```csharp
    using UnityEngine;
    using static Define;
    
    //3D사운드를 재생할 모든 오브젝트에 컴포넌트로 추가
    public class UnitSfxController : MonoBehaviour
    {
        private const int SFX_CHANNELS = 10; // 동시에 출력 가능한 사운드의 제한 수
        [SerializeField]private AudioClip[] sfxClips; // 출력할 오디오 클립들
        private AudioSource[] sfxPlayers; // 동시에 여러 클립을 재생할 수 있게 다수의 오디오소스 이용
    
        private void Awake() {
            InitSfx();
        }
    
        public void ChangeVolume(float volume) {
            foreach(var t in sfxPlayers) {
                t.volume = volume;
            }
        }
    
        void InitSfx() { // 오디오 소스를 3D 환경에 맞게 설정
            GameObject sfxObject = new GameObject(NAME_SFXPLAYER);  
            sfxObject.transform.parent = transform; 
            sfxObject.transform.position = transform.position;
            sfxPlayers = new AudioSource[SFX_CHANNELS];
    
            for (int i = 0; i < sfxPlayers.Length; i++) {
                sfxPlayers[i] = sfxObject.AddComponent<AudioSource>(); 
                sfxPlayers[i].playOnAwake = false; 
                sfxPlayers[i].volume = DEFAULT_VOLUME;  
                sfxPlayers[i].spatialBlend = SOUND_3DMODE;
                sfxPlayers[i].rolloffMode = AudioRolloffMode.Logarithmic;
                sfxPlayers[i].minDistance = SOUND_UNIT_3D_MINDISTANCE;
                sfxPlayers[i].maxDistance = SOUND_UNIT_3D_MAXDISTANCE;
            }
        }
    
        public void PlaySfx(UnitSfx sfx) { // 오디오 재생
            for (int i = 0; i < sfxPlayers.Length; i++) {
                if (sfxPlayers[i].isPlaying)  
                    continue;
    
                sfxPlayers[i].clip = sfxClips[(int)sfx];  
                sfxPlayers[i].Play();
                break;
            }
        }
    }
    ```
    

**🤓Result!**

BGM 및 리스너와의 거리에 관계없이 동일하게 출력되는 PersonalSfx, 소리를 내는 객체와 리스너와의 거리에 비례해 소리의 볼륨 등 설정이 변화하는 3D SFX 및 반복적으로 출력되는 BGM을 개별적으로 관리해 플레이어에게 자연스러운 사운드 경험 제공할 수 있었습니다.


### ・풀링 오브젝트 시스템

**🤔WHY?**

각종 오브젝트를 필요할 때 마다 생성, 필요가 없어지면 제거해 짧은 시간 내에 다량의 객체를 생성하고 제거하는 상황이 반복되 퍼포먼스가 크게 하락하였기에 사용했습니다.

**🤔HOW?**

 관련 코드

- PoolManager
    
    ```csharp
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;
    
    public class PoolManager
    {
        class Pool // 풀링 객체의 종류당 하나씩 생성되는 클래스
        {
            public GameObject Original { get; private set; } // 풀링 객체의 종류
            public Transform Root { get; private set; } // 풀링 객체들의 부모 오브젝트
            private Stack<Poolable> _poolStack = new Stack<Poolable>(); //풀링 객체를 관리할 스택
    
    				
            public void Init(GameObject original, int count = 5)
            {
                Original = original;
                Root = new GameObject().transform;
                Root.name = original.name + "_root";
    
                for (int i = 0; i < count; i++)
                {
                    Release(Create());
                }
            }
    
            
    
            private Poolable Create()
            {
                GameObject go = Object.Instantiate(Original);
                go.name = Original.name;
                return go.GetOrAddComponent<Poolable>();
            }
    
            public void Release(Poolable poolable)
            {
                if (poolable == null)
                    return;
    
                poolable.transform.parent = Root;
                poolable.gameObject.SetActive(false);
                _poolStack.Push(poolable);
            }
    
            public Poolable Activation()
            {
                Poolable poolable;
    
                if (_poolStack.Count > 0)
                    poolable = _poolStack.Pop();
                else
                    poolable = Create();
                
                poolable.gameObject.SetActive(true);
    
                poolable.transform.parent = Managers.Scene.CurrentSceneManager.transform;
    
                poolable.transform.parent = Root;
    
                return poolable;
            }
    
        }
        
        private Transform _root;
        private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();
    
        public void Init()
        {
            if (_root == null)
            {
                _root = new GameObject("@Pool_root").transform;
                Object.DontDestroyOnLoad(_root.GameObject());
            }
        }
    
        public void Release(Poolable poolable)
        {
            string name = poolable.gameObject.name;
    
            if (_pools.ContainsKey(name) == false)
            {
                Object.Destroy(poolable.gameObject);
                return;
            }
            
            _pools[name].Release(poolable);
        }
    
        public void Clear() {
            foreach(Transform t in _root) {
                Managers.Resources.Destroy(t.gameObject);
            }
            _root = null;
            _pools.Clear();
        }
    
        public Poolable Activation(GameObject original)
        {
            if(_pools.ContainsKey(original.name) == false)
                CreatePool(original);
    
            return _pools[original.name].Activation();
        }
    
        private void CreatePool(GameObject original, int count = 5)
        {
            Pool pool = new Pool();
            pool.Init(original,count);
            pool.Root.parent = _root;
            
            _pools.Add(original.name, pool);
        }
    
        public GameObject GetOriginal(string name)
        {
            if (_pools.ContainsKey(name) == false)
                return null;
    
            return _pools[name].Original;
        }
    }
    ```
    

**🤓Result!**

  시스템에 큰 부하를 주는 객체의 직접적인 생성 및 파괴를 최대한 피하고 풀링 시스템을 이용, 이미 생성된 객체를 재사용하는 과정을 통해 객체의 생성에 들어가는 비용을 줄여 퍼포먼스가 크게 상승했습니다.
  

## 📈보완점

**-문제점**

런타임시 평균 60프레임 가량의 낮은 퍼포먼스로 게임 플레이동안 큰 이질감을 지속적으로 느꼈습니다.

**-문제의 원인**

모든 오브젝트의 라이팅 및 그림자 계산이 런타임에 지속적으로 이루어져 지속적으로 CPU에 과한 연산을 발생시키고 있었습니다.

**-해결방안**

게임 내 요소의 대부분을 차지하는 정적 오브젝트들을 베이킹해 라이팅 및 그림자 계산을 사전에 완료 및 정적 오브젝트들을 static오브젝트로 설정해 드로우 콜을 감소시켰고, 오클루전 컬링에 필요한 계산도 사전에 완료해, 평균 100프레임 이상의 높은 퍼포먼스를 회복해 최적화에 성공했습니다.


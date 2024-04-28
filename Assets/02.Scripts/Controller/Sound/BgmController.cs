using UnityEngine;
using static Define;

public class BgmController : MonoBehaviour
{
    public static BgmController instance;
    [SerializeField] private AudioClip[] _clips;
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

    private void BgmInit() {
        _sources = new AudioSource();
        _sources = gameObject.AddComponent<AudioSource>();
        _sources.clip = _clips[(int)Bgm.Startup];
        _sources.loop = true;
        _sources.playOnAwake = false;
        _sources.volume = DEFAULT_VOLUME;
        _sources.spatialBlend = 0.0f;
    }

    public void SetBgm(Define.Bgm type, bool trigger) {
        _sources.Stop();

        if (trigger) {
            _sources.clip = _clips[(int)type];
            _sources.Play();
        }
    }
    
}

using UnityEngine;
using static Define;

public class PersonalSfxController : MonoBehaviour
{
    public static PersonalSfxController instance;
    private AudioSource[] _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
    private const int SFX_CHANNELS = 5;  
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        PersonalSfxInit();
    }

    public void ChangeVolume(float volume) {
        foreach(var t in _audioSource) {
            t.volume = volume * DEFAULT_VOLUME;
        }
    }

    protected void PersonalSfxInit() {
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
    public void SetPersonalSfx(PersonalSfx type) {
        for (int i = 0; i < _audioSource.Length; i++) {
            if (_audioSource[i].isPlaying)  
                continue;

            _audioSource[i].clip = _audioClips[(int)type];  
            _audioSource[i].Play();
            break;
        }
    }
}

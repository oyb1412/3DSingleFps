using UnityEngine;
using static Define;

public class UnitSfxController : MonoBehaviour
{
    private const int SFX_CHANNELS = 10; 
    [SerializeField]private AudioClip[] sfxClips;
    private AudioSource[] sfxPlayers;

    private void Awake() {
        InitSfx();
    }

    public void ChangeVolume(float volume) {
        foreach(var t in sfxPlayers) {
            t.volume = volume;
        }
    }

    void InitSfx() {
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

    public void PlaySfx(UnitSfx sfx) {
        for (int i = 0; i < sfxPlayers.Length; i++) {
            if (sfxPlayers[i].isPlaying)  
                continue;

            sfxPlayers[i].clip = sfxClips[(int)sfx];  
            sfxPlayers[i].Play();
            break;
        }
    }
}

using UnityEngine;
using static Define;

public class BulletSfxController : MonoBehaviour
{
    [SerializeField]private AudioClip[] sfxClips;
    private AudioSource sfxPlayers;

    private void Awake() {
        InitSfx();
    }

   

    

    public void ChangeVolume(float volume) {
        sfxPlayers.volume = volume;
    }

    void InitSfx() {
        GameObject sfxObject = new GameObject(NAME_SFXPLAYER);  
        sfxObject.transform.parent = transform; 
        sfxObject.transform.position = transform.position;
        sfxPlayers = new AudioSource();
        sfxPlayers = sfxObject.AddComponent<AudioSource>();
        sfxPlayers.clip = sfxClips[Random.Range(0, sfxClips.Length)];
        sfxPlayers.playOnAwake = true;
        sfxPlayers.volume = DEFAULT_VOLUME;
        sfxPlayers.spatialBlend = SOUND_3DMODE;
        sfxPlayers.rolloffMode = AudioRolloffMode.Logarithmic;
        sfxPlayers.minDistance = SOUND_BULLET_3D_MINDISTANCE;
        sfxPlayers.maxDistance = SOUND_BULLET_3D_MAXDISTANCE;

        sfxPlayers.Play();
    }

}

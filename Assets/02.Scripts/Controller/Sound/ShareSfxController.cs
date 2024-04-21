using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareSfxController : MonoBehaviour
{
    public static ShareSfxController instance;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips;
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
        _audioSource.volume = volume;
    }

    protected void ShareSfxInit() {
        _audioSource = new AudioSource();
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _audioClips[(int)Define.ShareSfx.Dominate];
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
        _audioSource.volume = .5f;
        _audioSource.spatialBlend = 0.0f;
    }
    public void SetShareSfx(Define.ShareSfx type) {
        _audioSource.Stop();
        _audioSource.clip = _audioClips[(int)type];
        _audioSource.Play();
    }
}

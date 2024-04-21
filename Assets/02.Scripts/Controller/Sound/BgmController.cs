using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    public static BgmController instance;

    [SerializeField] protected AudioClip[] _clips;
    protected AudioSource _sources;
    private void Awake() {


        if(instance == null) {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }else {
            GameObject.Destroy(gameObject);
        }
        BgmInit();
    }

    public void ChangeVolume(float volume) {
        _sources.volume = volume;
    }

    protected void BgmInit() {
        _sources = new AudioSource();
        _sources = gameObject.AddComponent<AudioSource>();
        _sources.clip = _clips[(int)Define.Bgm.Startup];
        _sources.loop = true;
        _sources.playOnAwake = false;
        _sources.volume = .5f;
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

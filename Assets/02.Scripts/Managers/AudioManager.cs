using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]protected AudioClip[] _clips;
    protected AudioSource[] _sources;

    protected void SfxInit() {
        _sources = new AudioSource[_clips.Length];
        for (int i = 0; i < _clips.Length; i++) {
            _sources[i] = gameObject.AddComponent<AudioSource>();
            _sources[i].clip = _clips[i];
            _sources[i].loop = false;
            _sources[i].playOnAwake = false;
            _sources[i].volume = .5f;
            _sources[i].spatialBlend = 1.0f;
            _sources[i].rolloffMode = AudioRolloffMode.Logarithmic;
            _sources[i].minDistance = 5f;
            _sources[i].maxDistance = 10f;
        }
    }

    protected void BgmInit() {
        _sources = new AudioSource[_clips.Length];
        for (int i = 0; i < _clips.Length; i++) {
            _sources[i] = gameObject.AddComponent<AudioSource>();
            _sources[i].clip = _clips[i];
            _sources[i].loop = true;
            _sources[i].playOnAwake = false;
            _sources[i].volume = .5f;
            _sources[i].spatialBlend = 0.0f;
        }
    }

}

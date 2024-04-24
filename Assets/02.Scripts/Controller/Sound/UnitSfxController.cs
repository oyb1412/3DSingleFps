using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSfxController : MonoBehaviour
{
    private const int SFX_CHANNELS = 10;  //한번에 최대 출력 가능한 효과음 수
    [SerializeField]private AudioClip[] sfxClips;  //게임에 존재하는 모든 sfx 목록
    private const float DefaultSfxVolume = 0.5f;  //sfx기본 볼륨
    AudioSource[] sfxPlayers;  //여러 sfx를 동시에 출력하기 위한 배열

    private void Start() {
        InitSfx();  //sfx 초기화
    }

    public void ChangeVolume(float volume) {
        foreach(var t in sfxPlayers) {
            t.volume = volume;
        }
    }

    /// <summary>
    /// sfx 초기화
    /// </summary>
    void InitSfx() {
        GameObject sfxObject = new GameObject("SfxPlayer");  //sfx player 생성
        sfxObject.transform.parent = transform; //부모를 manager로 지정
        sfxObject.transform.position = transform.position;
        sfxPlayers = new AudioSource[SFX_CHANNELS];  //채널 수 만큼 player 생성

        for (int i = 0; i < sfxPlayers.Length; i++) {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();  //각 player에 오디오 소스 추가
            sfxPlayers[i].playOnAwake = false;  //즉시 재생 해제
            sfxPlayers[i].volume = DefaultSfxVolume;  //기본 볼륨 설정
            sfxPlayers[i].spatialBlend = 1.0f;
            sfxPlayers[i].rolloffMode = AudioRolloffMode.Logarithmic;
            sfxPlayers[i].minDistance = 3f;
            sfxPlayers[i].maxDistance = 15f;
        }
    }

    /// <summary>
    /// sfx 재생
    /// </summary>
    /// <param name="sfx">재생할 sfx 타입</param>
    public void PlaySfx(Define.UnitSfx sfx) {
        for (int i = 0; i < sfxPlayers.Length; i++) {
            if (sfxPlayers[i].isPlaying)  //플레이어중 사용 가능한 플레이어를 서치
                continue;

            sfxPlayers[i].clip = sfxClips[(int)sfx];  //사용가능한 플레이어를 서치하면, 클립 설정 후 플레이
            sfxPlayers[i].Play();
            break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSfxController : MonoBehaviour
{
    private const int SFX_CHANNELS = 10;  //�ѹ��� �ִ� ��� ������ ȿ���� ��
    [SerializeField]private AudioClip[] sfxClips;  //���ӿ� �����ϴ� ��� sfx ���
    private const float DefaultSfxVolume = 0.5f;  //sfx�⺻ ����
    AudioSource[] sfxPlayers;  //���� sfx�� ���ÿ� ����ϱ� ���� �迭

    private void Start() {
        InitSfx();  //sfx �ʱ�ȭ
    }

    public void ChangeVolume(float volume) {
        foreach(var t in sfxPlayers) {
            t.volume = volume;
        }
    }

    /// <summary>
    /// sfx �ʱ�ȭ
    /// </summary>
    void InitSfx() {
        GameObject sfxObject = new GameObject("SfxPlayer");  //sfx player ����
        sfxObject.transform.parent = transform; //�θ� manager�� ����
        sfxObject.transform.position = transform.position;
        sfxPlayers = new AudioSource[SFX_CHANNELS];  //ä�� �� ��ŭ player ����

        for (int i = 0; i < sfxPlayers.Length; i++) {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();  //�� player�� ����� �ҽ� �߰�
            sfxPlayers[i].playOnAwake = false;  //��� ��� ����
            sfxPlayers[i].volume = DefaultSfxVolume;  //�⺻ ���� ����
            sfxPlayers[i].spatialBlend = 1.0f;
            sfxPlayers[i].rolloffMode = AudioRolloffMode.Logarithmic;
            sfxPlayers[i].minDistance = 3f;
            sfxPlayers[i].maxDistance = 15f;
        }
    }

    /// <summary>
    /// sfx ���
    /// </summary>
    /// <param name="sfx">����� sfx Ÿ��</param>
    public void PlaySfx(Define.UnitSfx sfx) {
        for (int i = 0; i < sfxPlayers.Length; i++) {
            if (sfxPlayers[i].isPlaying)  //�÷��̾��� ��� ������ �÷��̾ ��ġ
                continue;

            sfxPlayers[i].clip = sfxClips[(int)sfx];  //��밡���� �÷��̾ ��ġ�ϸ�, Ŭ�� ���� �� �÷���
            sfxPlayers[i].Play();
            break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

//�� �ڵ�� Unity ���ӿ��� ���带 �����ϴ� ��ũ��Ʈ�Դϴ�
//. ��� ���ǰ� ȿ������ ó���ϸ�,
//���� �ε�� ������ �ش� ���� �´� ��� ������ ����մϴ�.
//���� ���� ���� �� ȿ���� ��� ����� �����Ǿ� �ֽ��ϴ�.
//�ڵ�� Singleton ������ ����Ͽ� �ϳ��� �ν��Ͻ��� �����ǵ��� �Ǿ� �ְ�,
//���� �ٲ� ������ ��� ������ �ڵ����� �����մϴ�.
//���� ����� �ͼ��� Ȱ���Ͽ� ������ �����մϴ�.
//ȿ������ �������� ������ GameObject�� AudioSource�� �߰��Ͽ� ����ϰ� ���� �ð� �Ŀ� �ı��˴ϴ�.

public class SoundManager : MonoBehaviour
{
    public AudioSource bgSound;
    public AudioMixer mixer;
    public AudioClip[] bglist;
    public AudioClip[] sfxlist;
    public AudioClip[] txtlist;
    private static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    // �̷��� �ϸ� ������ ������..
    // ��������� �渶�� �ϴ� �ȵ�!
    // �׷��� ������������ ���� �Ŵ����� �������� �����ϴ�..����..
    // �Ƹ� �� ��ȣ�� ���� ��������߰���! �� ��..
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        for(int i = 0; i < bglist.Length; i++)
        {
            if (scene.name == bglist[i].name)
            {
                BGSoundSave(bglist[i]);
            }
            //���ڵ�� else if()  
        }
    }
    private float LinearToDecibel(float linearValue)
    {
        if (linearValue == 0)
        {
            return -80f; // -80 dB is usually considered as silent
        }
        else
        {
            return 20f * Mathf.Log10(linearValue);
        }
    }

    public void BGSoundVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f); // ���� �׻� 0�� 1 ���̿� �ֵ��� ����
        mixer.SetFloat("BGSound", LinearToDecibel(val));
    }

    public void SFXSoundVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f); // ���� �׻� 0�� 1 ���̿� �ֵ��� ����
        mixer.SetFloat("SFXSound", LinearToDecibel(val));
    }

    // soundManager.SFXPlay(string,int);
    public void SFXPlay(string sfxName, int sfxNum)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();

        audioSource.clip = sfxlist[sfxNum];
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.volume = 0.1f;
        audioSource.Play();
        Destroy(go, sfxlist[sfxNum].length);
    }
    public void SFXTextPlay(string textName, int textNum)
    {
        GameObject go = new GameObject(textName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();

        audioSource.clip = txtlist[textNum];
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.volume = 0.1f;
        audioSource.Play();
        Destroy(go, txtlist[textNum].length);
    }
    public void BGSoundSave(AudioClip clip)
    {
        bgSound.clip = clip;
    }
    public void BGSoundPlay()
    {
        if (bgSound.clip != null)
        {
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
            bgSound.loop = true;
            bgSound.volume = 0.1f;
            bgSound.Play();
        }
    }
    public void StopBGSound()
    {
        if (bgSound.clip != null)
         bgSound.Stop();
    }
}

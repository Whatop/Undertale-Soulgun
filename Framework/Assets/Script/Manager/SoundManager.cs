using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgSound;
    public AudioMixer mixer;
    public AudioClip[] bglist;
    public AudioClip[] sfxlist;
    public AudioClip[] txtlist;
    private static SoundManager instance;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // SFX Ǯ
    public int initialPoolSize = 10; // �ʱ� Ǯ ũ��

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // �ʱ� SFX Ǯ ����
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject go = new GameObject("SFXSource");
                AudioSource audioSource = go.AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
                go.transform.SetParent(transform);
                go.SetActive(false);
                sfxPool.Enqueue(audioSource);
            }
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
            return instance;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (scene.name == bglist[i].name)
            {
                BGSoundSave(bglist[i]);
            }
        }
    }

    public void SFXPlay(string sfxName, int sfxNum)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = sfxlist[sfxNum];
        audioSource.volume = 0.1f;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }

    public void SFXTextPlay(string textName, int textNum)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = txtlist[textNum];
        audioSource.volume = 0.1f;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }

    private AudioSource GetAudioSourceFromPool()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource source = sfxPool.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            GameObject go = new GameObject("SFXSource");
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            go.transform.SetParent(transform);
            return audioSource;
        }
    }

    private IEnumerator DeactivateAfterPlay(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.gameObject.SetActive(false);
        sfxPool.Enqueue(audioSource);
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

    public void BGSoundPlay(int textNum)
    {
        if (bgSound.clip != null)
        {
            bgSound.clip = bglist[textNum];
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
            bgSound.loop = true;
            bgSound.volume = 0.1f;
            bgSound.Play();
        }
    }

    public void BGSoundPlayDelayed(int bgNum, float delay)
    {
        StartCoroutine(PlayBGAfterDelay(bgNum, delay));
    }

    private IEnumerator PlayBGAfterDelay(int bgNum, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bgNum >= 0 && bgNum < bglist.Length)
        {
            bgSound.clip = bglist[bgNum];
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

    // ����� ���� ����
    public void BGSoundVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f); // ���� �׻� 0�� 1 ���̿� �ֵ��� ����
        mixer.SetFloat("BGSound", LinearToDecibel(val));
    }

    // ȿ���� ���� ����
    public void SFXSoundVolume(float val)
    {
        val = Mathf.Clamp(val, 0f, 1f); // ���� �׻� 0�� 1 ���̿� �ֵ��� ����
        mixer.SetFloat("SFXSound", LinearToDecibel(val));
    }

    private float LinearToDecibel(float linearValue)
    {
        if (linearValue == 0)
        {
            return -80f; // -80 dB�� �������� ����
        }
        else
        {
            return 20f * Mathf.Log10(linearValue);
        }
    }

    // ����� �Ͻ� ���� �� �簳
    public void PauseBGSound()
    {
        if (bgSound.isPlaying)
            bgSound.Pause();
    }

    public void ResumeBGSound()
    {
        if (!bgSound.isPlaying)
            bgSound.UnPause();
    }

    public void FadeOutBGSound(float duration)
    {
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float currentTime = 0;
        float startVolume;

        // ���� ���� ���� �����ɴϴ� (���ú� ��)
        mixer.GetFloat("BGSound", out startVolume);
        // ���ú� ���� ���� ��(0~1)���� ��ȯ�մϴ�.
        startVolume = Mathf.Pow(10, startVolume / 20);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // ������ ���������� ���ҽ�ŵ�ϴ�.
            float newVolume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
            BGSoundVolume(newVolume);
            yield return null;
        }

        // ������ ������ 0���� �����ϰ� ������� �����մϴ�.
        BGSoundVolume(0f);
        StopBGSound();
    }

}

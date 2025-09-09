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

    [System.Serializable]
    public class SFXInfo
    {
        public int id;           // ��: 63, 226 ��
        public AudioClip clip;
        // ������ �߰� (�־ �ǰ� ��� �˴ϴ�)
        public SFXInfo(int _id, AudioClip _clip)
        {
            id = _id;
            clip = _clip;
        }
    }
    [SerializeField]
    private SFXInfo[] sfxList;   // Inspector���� ���� ID�� AudioClip ����

    // ��� ���� looping SFX�� ID���� �����ϱ� ���� Dictionary
    private Dictionary<int, AudioSource> loopingSources = new Dictionary<int, AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

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
            // sfxList ���̸� sfxClips.Length��ŭ ���, �� ĭ�� �ʱ�ȭ
            sfxList = new SFXInfo[sfxlist.Length];
            for (int i = 0; i < sfxlist.Length; i++)
            {
                // ���1: ������ ���
                sfxList[i] = new SFXInfo(i, sfxlist[i]);
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

    private void InitializeSFXPool()
    {
        // �ʱ� Ǯ ����: AudioSource ������Ʈ�� initialPoolSize��ŭ ���� ť�� �־��
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject go = new GameObject("SFXSource");
            go.transform.SetParent(transform);
            AudioSource a = go.AddComponent<AudioSource>();
            a.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            a.playOnAwake = false;
            go.SetActive(false);
            sfxPool.Enqueue(a);
        }
    }

    /// <summary>
    /// 1ȸ�� SFX ��� (Ǯ���� AudioSource ������ Play �� ������ Ǯ�� ����)
    /// </summary>
    public void SFXPlay(string sfxName, int sfxNum)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = sfxlist[sfxNum];
        audioSource.volume = 1f;
        audioSource.loop = false;
        audioSource.gameObject.SetActive(true);
        audioSource.priority = 130;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }

    /// <summary>
    /// 1ȸ�� SFX ���(���� ���� ����)
    /// </summary>
    public void SFXPlay(string sfxName, int sfxNum, float volume)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = sfxlist[sfxNum];
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.gameObject.SetActive(true);
        audioSource.priority = 130;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }
    public void SFXPlayDelayed(string sfxName, int sfxNum, float delaySeconds, float volume = 1f, bool unscaled = false)
    {
        StartCoroutine(PlaySFXAfterDelay(sfxNum, delaySeconds, volume, unscaled));
    }

    private IEnumerator PlaySFXAfterDelay(int sfxNum, float delaySeconds, float volume, bool unscaled)
    {
        if (!IsValidSfxIndex(sfxNum)) yield break;

        if (unscaled) yield return new WaitForSecondsRealtime(delaySeconds);
        else yield return new WaitForSeconds(delaySeconds);

        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = sfxlist[sfxNum];
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.gameObject.SetActive(true);
        audioSource.priority = 130;
        audioSource.Play();

        StartCoroutine(DeactivateAfterPlay(audioSource));
    }

    private bool IsValidSfxIndex(int idx)
    {
        return idx >= 0 && idx < sfxlist.Length;
    }
    /// <summary>
    /// �ؽ�Ʈ(�����̼� ��) ����� ��, 1ȸ������ ���
    /// </summary>
    public void SFXTextPlay(string textName, int textNum, float volume = 1)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = txtlist[textNum];
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.gameObject.SetActive(true);
        audioSource.priority = 150;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }
    public void SFXErrorTextPlay(string textName, int textNum, float volume = 1)
    {
        AudioSource audioSource = GetAudioSourceFromPool();
        audioSource.clip = txtlist[textNum];
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.gameObject.SetActive(true);
        audioSource.priority = 130;
        audioSource.Play();
        StartCoroutine(DeactivateAfterPlay(audioSource));
    }
    /// <summary>
    /// 1ȸ�� SFX�� AudioSource�� Ǯ���� �����ų�, Ǯ�� �����ִ� �� ������ ���� ����
    /// </summary>
    private AudioSource GetAudioSourceFromPool()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource source = sfxPool.Dequeue();
            return source;
        }
        else
        {
            GameObject go = new GameObject("SFXSource");
            go.transform.SetParent(transform);
            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            audioSource.playOnAwake = false;
            return audioSource;
        }
    }
    /// <summary>
    /// ���� ���� ��� ���� (ID�� �ĺ�) 
    /// �̹� ��� ���̶�� ����
    /// </summary>
    public void SFXPlayLoop(int sfxNum, float volume = 1f)
    {
        // �̹� ��� ���̸� �ߺ� ��� ����
        if (loopingSources.ContainsKey(sfxNum))
            return;

        // Ǯ���� AudioSource ������
        AudioSource source = GetAudioSourceFromPool();
        source.clip = sfxlist[sfxNum];
        source.volume = volume;
        source.loop = true;
        source.priority = 130;
        source.gameObject.SetActive(true);
        source.Play();

        loopingSources.Add(sfxNum, source);
    }

    /// <summary>
    /// ���� ���� ���� (ID�� �ĺ��Ͽ� �ش� AudioSource�� Ǯ�� ��ȯ)
    /// </summary>
    public void SFXStopLoop(int sfxNum)
    {
        if (!loopingSources.ContainsKey(sfxNum))
            return;

        AudioSource source = loopingSources[sfxNum];
        source.Stop();
        source.loop = false;
        source.gameObject.SetActive(false);

        // Ǯ�� �ǵ�����
        sfxPool.Enqueue(source);
        loopingSources.Remove(sfxNum);
    }
    /// <summary>
    /// Ŭ���� ������ AudioSource�� ���� Ǯ�� ����
    /// </summary>
    private IEnumerator DeactivateAfterPlay(AudioSource audioSource)
    {
        // Ŭ�� ���̸�ŭ ���
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
        sfxPool.Enqueue(audioSource);
    }
    public void BGSoundSave(AudioClip clip)
    {
        bgSound.clip = clip;
    }

    // ��� ���� ���
    public void BGSoundPlay(int bgNum)
    {
        if (bgNum < 0 || bgNum >= bglist.Length)
        {
            Debug.LogWarning("[SoundManager] �߸��� bgNum �ε����Դϴ�: " + bgNum);
            return;
        }
        bgSound.clip = bglist[bgNum];
        if (bgSound.clip != null)
        {
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
            bgSound.loop = true;
            bgSound.volume = 0.6f;
            bgSound.Play();
        }
    }

    public void BGSoundPlay(int bgNum, float volume = 0.6f)
    {
        if (bgNum < 0 || bgNum >= bglist.Length)
        {
            Debug.LogWarning("[SoundManager] �߸��� bgNum �ε����Դϴ�: " + bgNum);
            return;
        }
        bgSound.clip = bglist[bgNum];
        if (bgSound.clip != null)
        {
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
            bgSound.loop = true;
            bgSound.volume = volume;
            bgSound.Play();
        }
    }

    public void BGSoundPlayDelayed(int bgNum, float delay, float volume = 0.6f)
    {
        StartCoroutine(PlayBGAfterDelay(bgNum, delay, volume));
    }

    private IEnumerator PlayBGAfterDelay(int bgNum, float delay, float volume = 0.6f)
    {
        yield return new WaitForSeconds(delay);

        if (bgNum >= 0 && bgNum < bglist.Length)
        {
            bgSound.clip = bglist[bgNum];
            bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
            bgSound.loop = true;
            bgSound.volume = volume;
            bgSound.Play();
        }
    }

    public void StopBGSound()
    {
        if (bgSound.clip != null)
            bgSound.Stop();
        Debug.Log("���⼭ �����!!!!!!!!!!!!!");
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
        Debug.Log("����� ����");
        if (bgSound.isPlaying)
            bgSound.Pause();
    }

    public void ResumeBGSound()
    {
        if (!bgSound.isPlaying)
        {
            Debug.Log("����� �簳");
            bgSound.UnPause();
        }
    }

    // ����� ���̵� �ƿ�
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

    // ��� ������ ������ �����ϴ� �޼���
    public void SlowDownMusic()
    {
        if (bgSound != null)
        {
            bgSound.pitch = 0.5f; // ������ ������ ���� (�⺻ 1.0���� 0.5�� ����)
            Debug.Log("���� �ӵ��� ������ �����߽��ϴ�.");
        }
    }

    // ��� ������ �����ϴ� �޼���
    public void PlayMusic(string musicName)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (bglist[i].name == musicName)
            {
                BGSoundPlay(i);
                Debug.Log($"{musicName} ������ ����մϴ�.");
                break;
            }
        }
    }
}
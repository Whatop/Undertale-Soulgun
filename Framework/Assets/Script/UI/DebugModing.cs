using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DebugModing : MonoBehaviour
{
    [SerializeField]
    bool isDebugMode = true; // ����� ��� Ȱ��ȭ ����
    int debugMode = 0;

    public TextMeshProUGUI debug_text;

    public GameObject[] hideObjects;

    private void Start()
    {
        HideObjects(); // ���� ��ü ó��
    }

    void Update()
    {
        GetInputDebug(); // ����� ��� �Է� ó��
    }

    private void GetInputDebug()
    {
        if (!isDebugMode)
            return;

        if (Input.GetKeyDown(KeyCode.F1))
            debugMode = 1;
        else if (Input.GetKeyDown(KeyCode.F2))
            debugMode = 2;
        else if (Input.GetKeyDown(KeyCode.F3))
            debugMode = 3;
        else if (Input.GetKeyDown(KeyCode.F4))
            debugMode = 4;
        else if (Input.GetKeyDown(KeyCode.F5))
            debugMode = 5;

        HandleDebugMode();
    }

    private void HandleDebugMode()
    {
        switch (debugMode)
        {
            case 1:
                //LoadScene("IntroScene");
                GameManager.Instance.ClearPlayerPrefs();
                LoadScene("TestScene");
                Debug.Log("�ʱ�ȭ �� �ٽ� �ҷ�����");
                break;
            case 2:
                GameManager.Instance.ClearPlayerPrefs();
                LoadScene("GameScene");
                //LoadScene("ProduceScene");

                break;
            case 3:
                // LoadScene("GameScene");
                // Soul ��� ó�� �׽�Ʈ �뵵�Դϴ�
                bool issoulactive = GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().isSoulActive;
                issoulactive = !issoulactive; // Soul ��� ��ȯ

                if (issoulactive)
                {
                    GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().tutorialDontShot = false;
                    GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().EnableSoul();
                }
                else
                {
                    GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().DisableSoul();
                }
                Debug.Log("��� ���� ");
                debugMode = 0;
                break;
            case 4:
                SoundManager.Instance.BGSoundPlay(5, 0.6f);

                debugMode = 0;
                break;
            case 5:
                SoundManager.Instance.BGSoundPlay(2, 0.6f);
                // ���� Ȯ�� ����
                debugMode = 0;
                break;
            default:
                break;
        }
    }

   private void LoadScene(string sceneName)
   {
       SceneManager.LoadScene(sceneName);
       debugMode = 0;
   }

    private void HideObjects()
    {
        foreach (var obj in hideObjects)
        {
            if (obj.TryGetComponent(out SpriteRenderer renderer))
            {
                renderer.color = Color.clear;
            }
        }
    }
}

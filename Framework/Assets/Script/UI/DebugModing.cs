using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugModing : MonoBehaviour
{
    [SerializeField]
    bool isDebugMode = true; // ����� ��� Ȱ��ȭ ����
    int debugMode = 0;

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
                LoadScene("IntroScene");
                break;
            case 2:
                LoadScene("ProduceScene");
                break;
            case 3:
                LoadScene("GameScene");
                break;
            case 4:
                if (SceneManager.GetActiveScene().name == "GameScene")
                {
                    // UI_Item_Event_Manager.Instance.Init();
                    // UI_Item_Event_Manager.Instance.IngameStart();
                }
                debugMode = 0;
                break;
            case 5:
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

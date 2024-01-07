using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class DebugModing : MonoBehaviour
{
    //ġƮ `���߿� �ٲ���Ұ�`
    [SerializeField]
    bool isDebugMode = true;
    int debugMode = 0;
   

    void Update()
    {
        GetInputDebug();
    }

    private void GetInputDebug()
    {
        if (!isDebugMode)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            debugMode = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            debugMode = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            debugMode = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            debugMode = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            debugMode = 5;

        // ����� ��忡 ���� ó��
        switch (debugMode)
        {
            case 1:
                // ����� ��� 1 ��Ʈ�� ȭ��
                SceneManager.LoadScene("IntroScene");
                debugMode = 0;
                break;
            case 2:
                // ����� ��� 2 �г��� ȭ��
                SceneManager.LoadScene("ProduceScene");
                debugMode = 0;
                break;
            case 3:
                // ����� ��� 3 ���� OverWolrd
                SceneManager.LoadScene("GameScene");
               
                debugMode = 0;
                break;
            case 4:
                // ����� ��� 4 ���� InGame
                if (SceneManager.GetActiveScene().name == "GameScene")
                {
                    //UI_Item_Event_Manager.Instance.Init();
                    //UI_Item_Event_Manager.Instance.IngameStart();
                }

                debugMode = 0;
                break;
            case 5:
                // ����� ��� 5 ó�� 
                
                debugMode = 0;
                break;
            default:
                // ����� ��尡 �������� ���� ��� ó��
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    private string dialogueFolderPath = "Assets/Dialogues/"; // ��ȭ ������ ����� ���� ���

    public void StartDialogue(int npcID)
    {
        string filePath = dialogueFolderPath + "NPC_" + npcID + ".txt"; // NPC�� ��ȭ ���� ���
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                Debug.Log(line); // ��ȭ ���� ���
            }
        }
        else
        {
            Debug.LogWarning("NPC_" + npcID + ".txt ������ ã�� �� �����ϴ�.");
        }
    }
}

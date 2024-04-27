using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    public DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager.StartDialogue(npcID); // NPC�� ��ȭ ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogueManager.StartDialogue(npcID); // �����̽� �ٸ� ������ ��ȭ ����
        }
    }
}

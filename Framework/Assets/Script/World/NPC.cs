using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    public DialogueManager dialogueManager;
    private bool isTalking = false; // ��ȭ�� ���� ������ ����

    void Start()
    {
        StartDialogue(); // NPC�� ��ȭ ����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTalking)
            {
                dialogueManager.DisplayNextSentence(); // ��ȭ ����
            }
            else
            {
                StartDialogue(); // ��ȭ ����
            }
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialogueManager.StartDialogue(npcID); // NPC�� ��ȭ ����
    }

    public void EndDialogue()
    {
        isTalking = false; // ��ȭ ����
    }
}

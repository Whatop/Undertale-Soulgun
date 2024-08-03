using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences; // ��� ť
    private NPC currentNPC; // ���� ��ȭ ���� NPC

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(int npcID)
    {
        // ��縦 �ʱ�ȭ�ϰ�, ��縦 ť�� �߰��ϴ� �ڵ�
        // ����:
        sentences.Clear();
        sentences.Enqueue("Hello, I am NPC " + npcID);
        sentences.Enqueue("How are you today?");
        sentences.Enqueue("Goodbye!");

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log(sentence); // ���� ���ӿ����� ��縦 UI�� ǥ���ϴ� �ڵ尡 �ʿ��մϴ�.
    }

    void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue(); // NPC���� ��ȭ ���Ḧ �˸�
        }
    }

    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }
}

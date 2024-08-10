using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private NPC currentNPC;
    public TypeEffect typeEffect;
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(int npcID)
    {
        sentences.Clear();
        switch (npcID)
        {
            case 0:
                sentences.Enqueue("�ȳ�, ���� �׽�Ʈ NPC " + npcID);
                sentences.Enqueue("���� �Ϸ�� �?");
                sentences.Enqueue("�߰�~");
                break;
        }

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
        typeEffect.SetMsg(sentence, OnSentenceComplete);
    }

    private void OnSentenceComplete()
    {
        // ������ ������ ǥ�õ� �� �߰��� ������ ������ ���⿡ �߰��� �� �ֽ��ϴ�.
        Debug.Log("������ �Ϸ�Ǿ����ϴ�.");
    }

    void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
            UIManager.Instance.CloseTextbar();
            GameManager.Instance.GetPlayerData().isStop = false;

        }
    }

    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }
}

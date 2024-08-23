using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private NPC currentNPC;
    public TypeEffect typeEffect;

    public static DialogueManager Instance; // �̱��� ����

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(int npcID)
    {
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        switch (npcID)
        {
            case 0:
                sentences.Enqueue("�ȳ�, ���� �׽�Ʈ NPC " + npcID);
                sentences.Enqueue("���� �Ϸ�� �?");
                sentences.Enqueue("�߰�~");
                break;

            case 1001:
                sentences.Enqueue("�ȳ�, �� �̸��� �ö���");
                sentences.Enqueue("�̷�, ���� ���� �� ����");
                sentences.Enqueue("���� ���� �����ٰ�.");
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

    public void StartForcedEvent(int eventNumber)
    {
        // EventManager�� ���� ���� �̺�Ʈ ����
        EventManager.Instance.TriggerEvent(eventNumber);
    }
}

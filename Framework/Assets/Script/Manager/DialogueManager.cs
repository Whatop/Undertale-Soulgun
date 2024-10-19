using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DialogueData
{
    public int npcID;
    public bool isEvent;
    public string[] sentences;
}

[System.Serializable]
public class GameOverDialogueData
{
    public int npcID;
    public string[] sentences;
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues;
    public GameOverDialogueData[] gameOverDialogues;
}

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private Queue<string> gameover_sentences;
    private NPC currentNPC;
    public TypeEffect typeEffect;
    public TypeEffect gameOvertypeEffect;

    private DialogueDatabase dialogueDatabase;
    public static DialogueManager Instance { get; private set; }
    public Sprite[] npcFaces;
    private int npcID;

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
        gameover_sentences = new Queue<string>();

        LoadDialogueData();
    }

    private void ConfigureDialogueUI(bool isEvent = false, int eventID = -1)
    {
        bool showFace = isEvent;
        int faceIndex = -1;
        Vector2 textPosition = new Vector2(isEvent ? 160f : -160f, UIManager.Instance.text.gameObject.transform.localPosition.y);
        // �� �̹����� �������� ���� ����
        UIManager.Instance.npcFaceImage.gameObject.SetActive(showFace);

        // isEvent�� true�� �� eventID�� ���� �� �̹��� ����
        if (isEvent)
        {
            switch (eventID)
            {
                case 100:
                    faceIndex = 0; // ��: eventID�� 1000�� �� 0�� �� �̹��� ���
                    SoundManager.Instance.StopBGSound();
                    SoundManager.Instance.BGSoundPlay(3);
                    break;
                case 101:
                    faceIndex = 1; // ��: eventID�� 1001�� �� 1�� �� �̹��� ���
                    break;
                case 1000:
                    faceIndex = -1; // ��: eventID�� 1001�� �� 1�� �� �̹��� ���
                    SoundManager.Instance.SFXPlay("heal_sound", 123);
                    break;
                // �߰� �̺�Ʈ ID ó��
                default:
                    faceIndex = -1; // �⺻�� (�� �̹����� �������� ����)
                    break;
            }
        }

        // �� �̹����� ���̰� �����ϰ�, ��ȿ�� �ε����� ���� ����
        if (showFace && faceIndex >= 0 && faceIndex < npcFaces.Length)
        {
            UIManager.Instance.npcFaceImage.sprite = npcFaces[faceIndex];
        }
            UIManager.Instance.text.gameObject.transform.localPosition = textPosition;
    }

    private void LoadDialogueData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("dialogues");
        if (jsonFile != null)
        {
            dialogueDatabase = JsonUtility.FromJson<DialogueDatabase>(jsonFile.text);
        }
        else
        {
            Debug.LogError("Failed to load dialogue data.");
        }
    }

    public void StartDialogue(int id, bool isEvent = false)
    {
        npcID = id;
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        // JSON���� ��� ������ ��������
        DialogueData dialogue = FindDialogue(npcID, isEvent);

        ConfigureDialogueUI(isEvent, id);
        currentNPC.isEvent = isEvent;
        if (dialogue != null)
        {
            foreach (var sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }
        }
        else
        {
            Debug.LogWarning("No dialogue found for NPC ID: " + npcID);
        }

        UIManager.Instance.TextBarOpen();
        DisplayNextSentence(id);
    }

    private DialogueData FindDialogue(int id, bool isEvent)
    {
        foreach (var dialogue in dialogueDatabase.dialogues)
        {
            if (dialogue.npcID == id && dialogue.isEvent == isEvent)
            {
                return dialogue;
            }
        }
        return null;
    }


#region Game Over
public void StartGameOverDialogue(int npcID)
    {
        gameover_sentences.Clear();

        GameOverDialogueData gameOverDialogue = FindGameOverDialogue(npcID);
        if (gameOverDialogue != null)
        {
            foreach (var sentence in gameOverDialogue.sentences)
            {
                gameover_sentences.Enqueue(sentence.Replace("[PLAYER_NAME]", GameManager.Instance.GetPlayerData().player_Name));
            }
        }
        else
        {
            Debug.LogWarning("No game over dialogue found for NPC ID: " + npcID);
        }

        DisplayNextGameOver();
    }
    public void DisplayNextGameOver()
    {
        if (gameover_sentences.Count == 0)
        {
            return;
        }

        string sentence = gameover_sentences.Dequeue();
        gameOvertypeEffect.SetMsg(sentence, OnGameOverComplete);
    }
    private GameOverDialogueData FindGameOverDialogue(int id)
    {
        foreach (var dialogue in dialogueDatabase.gameOverDialogues)
        {
            if (dialogue.npcID == id)
            {
                return dialogue;
            }
        }
        return null;
    }

    private void OnGameOverComplete()
    {
        Debug.Log("�ӽ� ����, ���ӿ��� ��ȭ");
        StartCoroutine(TimeToLate());
    }

    IEnumerator TimeToLate()
    {
        yield return new WaitForSeconds(0.5f);
        string sentence = gameover_sentences.Dequeue();
        gameOvertypeEffect.SetMsg(sentence, End_And_LoadComplete);
    }
    #endregion

    public void DisplayNextSentence(int eventNumber)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        
        typeEffect.SetMsg(sentence, OnSentenceComplete, eventNumber);
    }
    private void OnSentenceComplete()
    {
        Debug.Log("������ �Ϸ�Ǿ����ϴ�.");
    }

    private void End_And_LoadComplete()
    {
        Debug.Log("���ӿ��� �� -> Save�� �Ѿ");
        UIManager.Instance.End_And_Load();
    }

    void EndDialogue(int eventNumber = 0)
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();
            UIManager.Instance.OnPlayerUI();
        }

        GameManager.Instance.GetPlayerData().isStop = false;
        UIManager.Instance.CloseTextbar();

        switch (npcID)
        {
            case 1000:
                UIManager.Instance.SaveOpen();
                break;
            case 1002:
                // Add actions if necessary
                break;
        }
    }

    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }
}

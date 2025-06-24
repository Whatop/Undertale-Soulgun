using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SentenceData
{
    public string text;
    public string expression;
}

[System.Serializable]
public class DialogueData
{
    public int npcID;
    public bool isEvent;
    public SentenceData[] sentences;
}
[System.Serializable]
public class GameOverDialogueData
{
    public int npcID;
    public SentenceData[] sentences;
}
[System.Serializable]
public class ItemDatabase
{
    public List<Item> items;
}

[System.Serializable]
public class DialogueDatabase
{
    public DialogueData[] dialogues;
    public GameOverDialogueData[] gameOverDialogues;
}

public class DialogueManager : MonoBehaviour
{
    public Queue<SentenceData> sentences;
    public Queue<SentenceData> gameover_sentences;
    public ItemDatabase itemDatabase { get; private set; }

    public NPC currentNPC;
    public TypeEffect typeEffect;
    public TypeEffect gameOvertypeEffect;

    [SerializeField]
    private DialogueDatabase dialogueDatabase;
    public static DialogueManager Instance { get; private set; }
    public Sprite[] npcFaces;
    private int npcID;
    [SerializeField]
    private NPC uiNpc;
    [SerializeField]
    private NPC gameoverNpc;
    public TypeEffect currentTypeEffect;


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
        sentences = new Queue<SentenceData>();
        gameover_sentences = new Queue<SentenceData>();
        itemDatabase = new ItemDatabase();
        LoadDialogueData();
        LoadItemData();
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
    public void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("items"); // 'items.json' ������ �ҷ���
        if (jsonFile != null)
        {
            itemDatabase = JsonUtility.FromJson<ItemDatabase>(jsonFile.text);

            if (itemDatabase != null && itemDatabase.items != null)
            {
                Debug.Log($"[LoadItemData] ������ {itemDatabase.items.Count}�� �ε� �Ϸ�");

                foreach (var item in itemDatabase.items)
                {
                    Debug.Log($"[LoadItemData] id: {item.id}, �̸�: {item.itemName}, Ÿ��: {item.itemType}");
                }
            }
            else
            {
                Debug.LogError("[LoadItemData] itemDatabase�� itemDatabase.items�� null�Դϴ�.");
            }
        }
        else
        {
            Debug.LogError("[LoadItemData] items.json ���� �ε� ����");
        }
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

    public bool IsEffecting()
    {
        return currentTypeEffect != null && currentTypeEffect.IsEffecting();
    }

    public void SkipTypeEffect()
    {
        if (currentTypeEffect != null && currentTypeEffect.IsEffecting())
        {
            currentTypeEffect.Skip();
        }
    }
    private void ConfigureDialogueUI(bool isEvent = false, int eventID = -1)
    {
        bool showFace = isEvent; // isEvent�� true�� ���� �� �̹����� ���̰� ��
        int faceIndex = -1;
        Vector2 textPosition = new Vector2(isEvent ? 160f : -160f, UIManager.Instance.text.gameObject.transform.localPosition.y);

        // �� �̹����� ���̰� ���� ���θ� ����
        UIManager.Instance.npcFaceImage.gameObject.SetActive(showFace);

        // isEvent�� true�� ���� eventID�� ���� �� �̹��� ����
        if (isEvent)
        {
            switch (eventID)
            {
                case 100:
                    faceIndex = 0; // ��: eventID�� 100�� �� 0�� �� �̹��� ���
                    SoundManager.Instance.StopBGSound();
                    SoundManager.Instance.BGSoundPlay(3);
                    break;
                case 101:
                    faceIndex = 1; // ��: eventID�� 101�� �� 1�� �� �̹��� ���
                    break;
                case 1000:
                case 1001:
                case 1002:
                    faceIndex = -1; // ��: eventID�� 1001�� �� 1�� �� �̹��� ���
                    SoundManager.Instance.SFXPlay("heal_sound", 123);
                    break;
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



    public void StartDialogue(int id, bool isEvent = false)
    {
        npcID = id;
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;

        currentNPC.isEvent = isEvent;
        DialogueData dialogue = FindDialogue(npcID, isEvent);

        // ��ȭ �����Ͱ� ���� ��� �α� ���
        if (dialogue == null)
        {
            Debug.LogWarning($"No dialogue found for NPC ID: {npcID}, isEvent: {isEvent}");
            return;
        }

        ConfigureDialogueUI(isEvent, id);

        foreach (var sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        UIManager.Instance.TextBarOpen();
        DisplayNextSentence(id);
    }

    public void StartItemDialogue(Item item)
    {
        // ��ȭ ť �ʱ�ȭ
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        SetUINPC(); // �̺�Ʈ ���ó�� ó��

        currentNPC.isEvent = true;
        string message = "";
        ConfigureDialogueUI(false, -1);

        // ������ Ÿ�Կ� ���� �޽��� ����
        switch (item.itemType)
        {
            case ItemType.HealingItem:
                message = $"* {item.itemName}�� �Ծ���.";

                // ȸ�� ������ ��� �� ü�� üũ
                if (GameManager.Instance.GetPlayerData().health == GameManager.Instance.GetPlayerData().Maxhealth)
                {
                    message += "\n* ����� HP�� ���� á��.";
                }
                break;

            case ItemType.Weapon:
            case ItemType.Armor:
                message = $"* {item.itemName}��(��) �����ߴ�.";
                break;

            default:
                message = "* �� �������� ����ص� �ƹ� �ϵ� �Ͼ�� �ʾҴ�.";
                break;
        }

        // ��縦 ť�� �߰�
        sentences.Enqueue(new SentenceData
        {
            text = message,
            expression = "Default"
        });

        // UI ���� �� ù ��° ��� ���
        UIManager.Instance.TextBarOpen();
        DisplayNextSentence();
    }
    public void StartInfoDialogue(Item item)
    {
        // ��ȭ ť �ʱ�ȭ
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        SetUINPC(); // �̺�Ʈ ���ó�� ó��

        currentNPC.isEvent = true;
        string message = "";
        ConfigureDialogueUI(false, -1);
        string item_Description = item.description;
        message = item_Description;

        // ��縦 ť�� �߰�
        sentences.Enqueue(new SentenceData
        {
            text = message,
            expression = "Default"
        });

        // UI ���� �� ù ��° ��� ���
        UIManager.Instance.TextBarOpen();
        DisplayNextSentence();
    }public void StartDropDialogue(Item item)
    {
        // ��ȭ ť �ʱ�ȭ
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        SetUINPC(); // �̺�Ʈ ���ó�� ó��
        string message = $"* {item.itemName} ��(��)\n    ��������.";

        currentNPC.isEvent = true;
        ConfigureDialogueUI(false, -1);

        // ��縦 ť�� �߰�
        sentences.Enqueue(new SentenceData
        {
            text = message,
            expression = "Default"
        });

        // UI ���� �� ù ��° ��� ���
        UIManager.Instance.TextBarOpen();
        DisplayNextSentence();
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
        SetOverNPC();
        GameOverDialogueData gameOverDialogue = FindGameOverDialogue(npcID);
        if (gameOverDialogue != null)
        {
            foreach (var sentence in gameOverDialogue.sentences)
            {
                // ���� ���� ��翡 ���� �ؽ�Ʈ�� ǥ���� ��� ť�� �߰�
                gameover_sentences.Enqueue(new SentenceData
                {
                    text = sentence.text.Replace("[PLAYER_NAME]", GameManager.Instance.GetPlayerData().player_Name),
                    expression = sentence.expression
                });
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
            End_And_LoadComplete();
            return;
        }

        SentenceData sentence = gameover_sentences.Dequeue();
        gameOvertypeEffect.SetMsg(sentence.text, OnGameOverComplete, 0);
    }


    private void OnGameOverComplete()
    {
        currentNPC.OnDialogueEffectComplete(); // ���� NPC�� ��ȭ �Ϸ� ó�� ȣ��

        Debug.Log(" ���ӿ��� ��ȭ");
    }

    #endregion

    public void DisplayNextSentence(int eventNumber = -1)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        SentenceData sentenceData = sentences.Dequeue();

        // �ؽ�Ʈ ���
        typeEffect.SetMsg(sentenceData.text, OnSentenceComplete, eventNumber, sentenceData.expression);
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

    private void EndDialogue(int eventNumber = 0)
    {
        if (currentNPC != null)
        {
            currentNPC.ResetToDefaultExpression(); // �⺻ ǥ������ ����
            currentNPC.EndDialogue();
        }
        GameManager.Instance.GetPlayerData().isStop = false;
        UIManager.Instance.CloseTextbar();

        switch (npcID)
        {
            case 1000:
            case 1001:
            case 1002:
                UIManager.Instance.SaveOpen();
                break;
        }

        npcID = -1; // npcID �ʱ�ȭ
    }


    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }
    public void SetUINPC()
    {
        currentNPC = uiNpc;
    }
    public void SetOverNPC()
    {
        currentNPC = gameoverNpc;
    }
}

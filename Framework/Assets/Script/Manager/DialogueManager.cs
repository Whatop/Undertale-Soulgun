using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private Queue<string> gameover_sentences;
    private NPC currentNPC;
    public TypeEffect typeEffect;
    public TypeEffect gameOvertypeEffect;

    public static DialogueManager instance;
    public Sprite[] npcFaces;
    private int npcID;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DialogueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("DialogueManager");
                    instance = obj.AddComponent<DialogueManager>();
                }
            }
            return instance;
        }
    }

    void Start()
    {
        sentences = new Queue<string>();
        gameover_sentences = new Queue<string>();
    }

    public void StartDialogue(int id)
    {
        npcID = id;
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        switch (npcID)
        {
            case 0:
                UIManager.Instance.npcFaceImage.gameObject.SetActive(false);
                UIManager.Instance.text.gameObject.transform.localPosition = new Vector2(-140, UIManager.Instance.text.gameObject.transform.localPosition.y);
                sentences.Enqueue("�׽�Ʈ NPC " + npcID);
                break;

            case 100:
                UIManager.Instance.npcFaceImage.gameObject.SetActive(true);
                UIManager.Instance.npcFaceImage.sprite = npcFaces[0];
                UIManager.Instance.text.gameObject.transform.localPosition = new Vector2(160, UIManager.Instance.text.gameObject.transform.localPosition.y);

                sentences.Enqueue("���?");
                sentences.Enqueue("Don't say that.");
                break;

            case 1000:
                UIManager.Instance.npcFaceImage.gameObject.SetActive(false);
                UIManager.Instance.text.gameObject.transform.localPosition = new Vector2(-140, UIManager.Instance.text.gameObject.transform.localPosition.y);

                SoundManager.Instance.SFXPlay("heal_sound", 123);
                sentences.Enqueue("* ����� �������� �����ϸ�..");
                sentences.Enqueue("* ����� ���ǰ� �游������.");
                break;


            case 1001:
                SoundManager.Instance.SFXPlay("heal_sound", 123);
                sentences.Enqueue("* ����� ���� �������� ����..");
                sentences.Enqueue("* ����� ���ǰ� �游������.");
                break;
        }

        UIManager.Instance.TextBarOpen();
        DisplayNextSentence();
    }
    public void StartEventDialogue(int id)
    {
        npcID = id;
        sentences.Clear();
        GameManager.Instance.GetPlayerData().isStop = true;
        switch (npcID)
        {
            case 0:
                UIManager.Instance.npcFaceImage.gameObject.SetActive(false);
                UIManager.Instance.text.gameObject.transform.localPosition = new Vector2(-140, UIManager.Instance.text.gameObject.transform.localPosition.y);
                sentences.Enqueue("�׽�Ʈ NPC " + npcID);
                break;

            case 100:
                UIManager.Instance.npcFaceImage.gameObject.SetActive(true);
                UIManager.Instance.npcFaceImage.sprite = npcFaces[0];
                UIManager.Instance.text.gameObject.transform.localPosition = new Vector2(160, UIManager.Instance.text.gameObject.transform.localPosition.y);
                
                sentences.Enqueue(".  .  .");
                sentences.Enqueue("�ȳ�, �� �̸��� �ö���");
                sentences.Enqueue("�̷�, ���� ���� �� ����");
                sentences.Enqueue("���� ���� �����ٰ�.");
                SoundManager.Instance.StopBGSound();
                SoundManager.Instance.BGSoundPlay(3);
                break;
        }
        currentNPC.isEvent = true;
        UIManager.Instance.TextBarOpen();
        UIManager.Instance.OffPlayerUI();
        DisplayNextSentence(npcID);
    }

    #region gameover
    public void StartGameOverDialogue(int npcID)
    {
        gameover_sentences.Clear();
        switch (npcID)
        {
            case 0:
                gameover_sentences.Enqueue("������� \n�����ؼ� �� �ȴ� . . .");
                gameover_sentences.Enqueue(GameManager.Instance.GetPlayerData().player_Name + "!" + " \n������ �����Ŷ� . . .");
                break;

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
    #endregion
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
    public void DisplayNextSentence(int eventNumber)
    {
        if (sentences.Count == 0)
        {
            EndDialogue(eventNumber);
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
    void EndDialogue()
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();

        }
        UIManager.Instance.CloseTextbar();
        GameManager.Instance.GetPlayerData().isStop = false;
        UIManager.Instance.OnPlayerUI();

        switch (npcID)
        {
            case 1000: // Save
                SoundManager.Instance.SFXPlay("save_sound", 171);
                UIManager.Instance.SaveOpen();

                break;
            case 1002: // Chest
                break;
        }
    }
    void EndDialogue(int eventNumber)
    {
        if (currentNPC != null)
        {
            currentNPC.EndDialogue();

        }
        UIManager.Instance.CloseTextbar();
        GameManager.Instance.GetPlayerData().isStop = false;
        UIManager.Instance.OnPlayerUI();
        switch (eventNumber)
        {
            case 100:
                BattleManager.Instance.BattleStart(eventNumber);
                break;
        }
    }
    public void SetCurrentNPC(NPC npc)
    {
        currentNPC = npc;
    }
}

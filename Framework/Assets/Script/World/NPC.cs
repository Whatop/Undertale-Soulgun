using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    public DialogueManager dialogueManager;
    [SerializeField]
    private bool isTalking = false;
    [SerializeField]
    private bool isFirstInteraction = true; // ó�� ��ȭ���� Ȯ��
    private GameObject outlineObject; // �ܰ��� ȿ���� ���� ������Ʈ
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer outlineSpriteRenderer;
    public Material outlineMaterial; // �ܰ��� Material

    public bool isEvent = false;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutline(); // ���̶���Ʈ�� �ܰ��� ������Ʈ ����
    }

    void Update()
    {
        if (IsPlayerNearby() && !isEvent) // �̺�Ʈ ���� �ƴ� �� ��ȣ�ۿ� ����
        {
            Highlight(true); // �÷��̾ ������ ������ ���̶���Ʈ ǥ��

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTalking)
                {
                    dialogueManager.DisplayNextSentence();
                }
                else
                {
                    StartDialogue();
                }
            }
        }
        else
        {
            Highlight(false); // �÷��̾ �־����� ���̶���Ʈ ����
        }

        // ���� �̺�Ʈ ���� �� ��ȭ ó��
        if (isEvent && isFirstInteraction)
        {
            StartDialogue();
            isFirstInteraction = false;
        }

        if (isEvent && Input.GetKeyDown(KeyCode.Space) && isTalking)
        {
            dialogueManager.DisplayNextSentence();
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialogueManager.SetCurrentNPC(this);
        dialogueManager.StartDialogue(npcID);
    }

    public void EndDialogue()
    {
        isTalking = false;
        isEvent = false;
        isFirstInteraction = true;
    }

    bool IsPlayerNearby()
    {
        float distance = Vector3.Distance(transform.position, GameManager.Instance.GetPlayerData().position);
        return distance <= 3f; // ���ϴ� �Ÿ��� ����
    }

    // �ܰ��� ������Ʈ ����
    void CreateOutline()
    {
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * 1.1f; // ���� ũ�⺸�� �ణ ũ�� ����

        outlineSpriteRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        outlineSpriteRenderer.material = outlineMaterial; // Material�� �ܰ��� Material�� ����
        outlineSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // NPC���� �ڿ� ǥ�õǵ��� ���� ���� ����

        outlineObject.SetActive(false); // ó������ ��Ȱ��ȭ
    }

    // ���̶���Ʈ ȿ�� ����
    void Highlight(bool isHighlighted)
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(isHighlighted);
        }
    }
}

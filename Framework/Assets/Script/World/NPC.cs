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
    private Animator animator;

    public bool isEvent = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        CreateOutline(); // ���̶���Ʈ�� �ܰ��� ������Ʈ ����
    }

    void Update()
    {
        HandleInteraction();
    }

    // ��ȣ�ۿ� ������ �޼ҵ�� �и�
    private void HandleInteraction()
    {
        bool playerNearby = IsPlayerNearby();

        if (playerNearby && !isEvent && !UIManager.Instance.isSaveDelay && !UIManager.Instance.isInventroy)
        {
            Highlight(true); // �÷��̾ ������ ������ ���̶���Ʈ ǥ��

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                if (isTalking)
                {
                    dialogueManager.DisplayNextSentence(npcID);
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

        // �̺�Ʈ ��� ó��
        HandleEventDialogue();
    }

    private void HandleEventDialogue()
    {
        if (isEvent && isFirstInteraction)
        {
            isTalking = true;
            dialogueManager.SetCurrentNPC(this);
            isFirstInteraction = false;
        }

        if (isEvent && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && isTalking)
        {
            dialogueManager.DisplayNextSentence(npcID);
        }
    }

    // ǥ���� �����ϴ� �޼ҵ�
    public void SetExpression(string expression)
    {
        if (animator != null)
        {
            animator.SetTrigger(expression);
        }
    }

    // �⺻ ǥ������ ���ư��� �޼ҵ�
    public void ResetToDefaultExpression()
    {
        if (animator != null)
        {
            animator.SetTrigger("Default");
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
        return distance <= 3f;
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

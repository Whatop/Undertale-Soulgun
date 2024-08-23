using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    public DialogueManager dialogueManager;
    private bool isTalking = false; // ��ȭ�� ���� ������ ����
    private GameObject outlineObject; // �ܰ��� ȿ���� ���� ������Ʈ
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer outlineSpriteRenderer;
    public Material outlineMaterial; // �ܰ��� Material

    bool isFirst = true;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutline();
    }

    void Update()
    {
        if (IsPlayerNearby() && !EventManager.Instance.isEvent) // �̺�Ʈ ���� �ƴ� ���� ��ȣ�ۿ� ����
        {
            Highlight(true);

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
        else
        {
            Highlight(false);
        }
        if (EventManager.Instance.isEvent) // ���� �̺�Ʈ
        {
            if (isFirst)
            {
                StartDialogue(); // ��ȭ ����
                isFirst = false;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTalking)
                {
                    dialogueManager.DisplayNextSentence(); // ��ȭ ����
                }
                
            }
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        UIManager.Instance.TextUpdate();
        dialogueManager.SetCurrentNPC(this); // NPC �ڽ��� DialogueManager�� �˸�
        dialogueManager.StartDialogue(npcID); // NPC�� ��ȭ ����
    }

    public void EndDialogue()
    {
        isTalking = false; // ��ȭ ����
        isFirst = false; // ó��
    }

    // �÷��̾ ������ �ִ��� Ȯ��
    bool IsPlayerNearby()
    {
        // �÷��̾���� �Ÿ� ��� (����: 3���� �̳�)
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
        outlineSpriteRenderer.sortingOrder = 2; // ���� ���� 2�� ����

        outlineObject.SetActive(false);
    }

    // ���̶���Ʈ ȿ�� ����
    void Highlight(bool isHighlighted)
    {
        outlineObject.SetActive(isHighlighted);
    }
}

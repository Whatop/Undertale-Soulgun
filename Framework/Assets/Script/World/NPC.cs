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
    public GameObject TextBar; //�Ƹ��� �������� �ҵ� ���߿� �迭

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CreateOutline();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsPlayerNearby())
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
        TextBar.SetActive(true);
        dialogueManager.SetCurrentNPC(this); // NPC �ڽ��� DialogueManager�� �˸�
        dialogueManager.StartDialogue(npcID); // NPC�� ��ȭ ����
    }

    public void EndDialogue()
    {
        isTalking = false; // ��ȭ ����
    }

    // �÷��̾ ������ �ִ��� Ȯ��
    bool IsPlayerNearby()
    {
        // �÷��̾���� �Ÿ� ��� (����: 2���� �̳�)
        float distance = Vector3.Distance(transform.position, GameManager.Instance.GetPlayerData().position);
        return distance <= 2.0f;
    }

    // Ʈ���� ���� �̺�Ʈ
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Highlight(true);
        }
    }

    // Ʈ���� ����Ʈ �̺�Ʈ
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Highlight(false);
        }
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

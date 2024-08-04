using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    public DialogueManager dialogueManager;
    private bool isTalking = false; // ��ȭ�� ���� ������ ����
    private Renderer npcRenderer; // NPC�� Renderer
    private Color originalColor; // NPC�� ���� ����

    void Start()
    {
        npcRenderer = GetComponent<Renderer>();
        originalColor = npcRenderer.material.color;
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

    // ���̶���Ʈ ȿ�� ����
    void Highlight(bool isHighlighted)
    {
        if (isHighlighted)
        {
            npcRenderer.material.color = Color.white; // ���̶���Ʈ �������� ����
            npcRenderer.material.SetColor("_OutlineColor", Color.white); // ��� �׵θ� �߰�
        }
        else
        {
            npcRenderer.material.color = originalColor; // ���� �������� ����
            npcRenderer.material.SetColor("_OutlineColor", Color.clear); // �׵θ� ����
        }
    }
}

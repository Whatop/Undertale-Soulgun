using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPC : MonoBehaviour
{
    public int npcID; // NPC�� ID
    [SerializeField]
    private DialogueManager dialogueManager;
    
    public bool isTalking = false;
    [SerializeField]
    private bool isFirstInteraction = true; // ó�� ��ȭ���� Ȯ��
    private GameObject outlineObject; // �ܰ��� ȿ���� ���� ������Ʈ
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer outlineSpriteRenderer;
    public Material outlineMaterial; // �ܰ��� Material
    private Animator animator;
    private Animator TextBar_animator;

    private bool canAdvanceDialogue = false; // ��ȭ ���� ���� ���θ� �����ϴ� �÷���

    public bool isEvent = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        TextBar_animator = UIManager.Instance.npcFaceImage.GetComponent<Animator>();
        dialogueManager = DialogueManager.Instance;
        CreateOutline(); // ���̶���Ʈ�� �ܰ��� ������Ʈ ����

        // �ʼ� ���� �ʱ�ȭ
        isTalking = false;
        isFirstInteraction = true;
        isEvent = false;

        Debug.Log($"NPC {npcID} �ʱ�ȭ �Ϸ�: isFirstInteraction={isFirstInteraction}, isTalking={isTalking}");   
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }


    void Update()
    {
        HandleInteraction();
    }
    private void HandleInteraction()
    {
        bool playerNearby = IsPlayerNearby();

        if (isTalking)
        {
            // ��ȭ �߿��� ��ȣ�ۿ� ����
            UIManager.Instance.isInventroy = false;

            // Ÿ���� ȿ�� ���� ���
            if (dialogueManager.IsEffecting())
            {
                if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
                {
                    dialogueManager.SkipTypeEffect();
                    UIManager.Instance.isSaveDelay = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                dialogueManager.DisplayNextSentence(npcID);
            }

            return; // ��ȭ ���̹Ƿ� ������ ��ȣ�ۿ��� ó������ ����
        }

        if (playerNearby && !isEvent && isFirstInteraction && !UIManager.Instance.isSaveDelay &&
            !UIManager.Instance.isInventroy && !GameManager.Instance.GetPlayerData().isDie &&
            !UIManager.Instance.isUserInterface)
        {
            if(npcID != 1002 && npcID != 1000 && npcID != 1001)
            Highlight(true);

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space))
            {
                StartDialogue();
            }
        }
        else
        {
            Highlight(false);
        }

        if (GameManager.Instance.GetPlayerData().isDie)
        {
            HandleOverDialogue();
        }
        else
            HandleEventDialogue();

    }


    public void EndDialogue()
    {
        isTalking = false;
        isEvent = false;
        isFirstInteraction = true;
        // ��ȭ�� ������ ��� ��ȣ�ۿ� ����
        UIManager.Instance.isSaveDelay = true; // ��ȣ�ۿ� �����ϵ��� ����
        StartCoroutine(InteractionDelay());

        // ��ȭ�� ������ �κ��丮 ��ȣ�ۿ� ���
        UIManager.Instance.isInventroy = true;
        UIManager.Instance.ChangeInventroy();
    
        if(npcID == 100)
        {
            PortalManager.Instance.OnPortalTeleport(999);
            BattleManager.Instance.BattleStart(2);
        }
    }

    public IEnumerator InteractionDelay()
    {
        yield return new WaitForSeconds(0.2f); // 0.2�� ���� ��ȣ�ۿ� ����
        UIManager.Instance.isSaveDelay = false; // ��ȣ�ۿ� �����ϵ��� ����
    }
    private void HandleEventDialogue()
    {
        if (isEvent && isFirstInteraction)
        {
            isTalking = true;
            dialogueManager.SetCurrentNPC(this);
            isFirstInteraction = false;
        }

        if (isEvent && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && isTalking && !UIManager.Instance.isSaveDelay && !UIManager.Instance.isInventroy)
        {
            // ���� ��� ��� ���� ���� �Ϸ�� ���¸� ����
            if (dialogueManager.IsEffecting())
            {
                UIManager.Instance.isSaveDelay = true; // ��ȣ�ۿ� �����ϵ��� ����
                dialogueManager.SkipTypeEffect(); // Ÿ���� ȿ���� ��� �Ϸ�
            }
            else
            {
                dialogueManager.DisplayNextSentence(npcID); // ���� ���� ǥ��
            }
        }
    }

    private void HandleOverDialogue()
    {
        // Z �Ǵ� Space Ű�� ���Ȱ�, ��ȭ ������ ������ ���¿����� ����
        if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Space)) && canAdvanceDialogue)
        {
            // ���� ��簡 ���� ��쿡�� ���� �������� �Ѿ
            dialogueManager.DisplayNextGameOver();
            canAdvanceDialogue = false; // ���� ���� �Ѿ �Ŀ��� �ٽ� false�� ����
        }
    }
    public void OnDialogueEffectComplete()
    {
        canAdvanceDialogue = true; // ��� Ÿ���� �Ϸ� �� true�� �����Ͽ� Ű �Է� �����ϰ� ��
    }

    // ǥ���� �����ϴ� �޼ҵ�
    /// <summary>
    /// Default, Smile,Smiling, Pain, Angry, Sneer, Talking, Surprise, Sad
    /// </summary>
    /// <param name="expression"></param>
    public void SetExpression(string expression)
    {
        if (animator != null && HasTrigger(animator, expression))
        {
            animator.ResetTrigger("Talking");
            animator.ResetTrigger("Default");

            animator.SetTrigger(expression);
        }
        if (TextBar_animator != null && HasTrigger(TextBar_animator, expression))
        {
            TextBar_animator.ResetTrigger("Talking");
            TextBar_animator.ResetTrigger("Default");

            TextBar_animator.SetTrigger(expression);
        }
    }

    // Animator�� �ش� Ʈ���Ű� �ִ��� Ȯ���ϴ� �޼���
    private bool HasTrigger(Animator animator, string triggerName)
    {
        return animator.parameters.Any(param => param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName);
    }

    // �⺻ ǥ������ ���ư��� �޼ҵ�
    public void ResetToDefaultExpression()
    {
        if (animator != null && HasTrigger(animator, "Default"))
        {
            animator.SetTrigger("Default");
        }
        if (TextBar_animator != null && HasTrigger(TextBar_animator, "Default"))
        {
            TextBar_animator.SetTrigger("Default");
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialogueManager.SetCurrentNPC(this);
        dialogueManager.StartDialogue(npcID);
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

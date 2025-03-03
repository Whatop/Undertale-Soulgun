using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds = 10f; // �ʴ� ���� ��
    private string targetMsg;
    public TextMeshProUGUI msgText;
    private int index;
    public int txtId = 0;
    private System.Action onEffectEndCallback;
    private string txtsound = "SND_TXT1";
    public Coroutine typingCoroutine;
    // ���� �ڵ� ����
    private string currentExpression; // ���� ǥ�� ���� ����

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
    }

    public void SetMsg(string msg, System.Action onEffectEnd)
    {
        targetMsg = msg;
        onEffectEndCallback = onEffectEnd;
        StartEffect();
    }

    public void SetMsg(string msg, System.Action onEffectEnd, int eventNumber)
    {
        targetMsg = msg;
        onEffectEndCallback = onEffectEnd;

        switch (eventNumber)
        {
            case 0:
                txtsound = "SND_sASR";
                txtId = 3;
                break;
            case 100:
                txtsound = "voice_flowey_1";
                txtId = 17;
                break;
            default:
                txtsound = "SND_TXT1";
                txtId = 0;
                break;
        }
        StartEffect();
    }
    public void SetMsg(string msg, System.Action onEffectEnd, int eventNumber, string expression = null)
    {
        targetMsg = msg;
        onEffectEndCallback = onEffectEnd;
        currentExpression = expression; // ���� ǥ�� ������ ����

       
        switch (eventNumber)
        {
            case 0:
                txtsound = "SND_sASR";
                txtId = 3;
                break;
            case 100:
                txtsound = "voice_flowey_1";
                txtId = 17;
                break;
            default:
                txtsound = "SND_TXT1";
                txtId = 0;
                break;
        }

        StartEffect();
    }
    public void Clear()
    {
            msgText.text = "";
    }
    public bool IsEffecting()
    {
        return typingCoroutine != null; // `Effecting` �ڷ�ƾ�� ���� ���̸� true ��ȯ
    }

    public void Skip()
    {
        if (IsEffecting()) // �ڷ�ƾ�� ���� ���� ���� ��ŵ ���
        {
            StopCoroutine(typingCoroutine);
            msgText.text = targetMsg;
            EffectEnd();
            // Skip �ÿ��� ��ȭ ���� �� ��ȣ�ۿ��� �������մϴ�.
            DialogueManager.Instance.currentNPC?.StartCoroutine(DialogueManager.Instance.currentNPC.InteractionDelay());

        }
    }

    private void StartEffect()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        msgText.text = "";
        index = 0;
        typingCoroutine = StartCoroutine(Effecting());
    }

    private IEnumerator Effecting()
    {
        bool isExpressionSet = false; // ǥ�� ���� ���θ� ����
        string currentText = "";      // ������� ��µ� �ؽ�Ʈ

        while (index < targetMsg.Length)
        {
            // ���ο� �ܾ ������ ���۵� �� ǥ�� ����
            if (!isExpressionSet && (index == 0 || targetMsg[index - 1] == ' '))
            {
                if (currentExpression != null && DialogueManager.Instance.currentNPC != null)
                {
                    DialogueManager.Instance.currentNPC.SetExpression(currentExpression);
                    isExpressionSet = true;
                }
            }

            // �±� ���� ���� �� ó��
            if (targetMsg[index] == '<')
            {
                int closeIndex = targetMsg.IndexOf('>', index);
                if (closeIndex != -1)
                {
                    // �±׸� ��ü������ ó��
                    string tag = targetMsg.Substring(index, closeIndex - index + 1);
                    currentText += tag; // �±׸� �����ϸ� �߰�
                    msgText.text = currentText; // �ؽ�Ʈ ������Ʈ
                    index = closeIndex + 1;
                    continue; // �±״� �� ���� ó���ǹǷ� ���� ������ �Ѿ
                }
            }

            // �Ϲ� ���� ���
            if (targetMsg[index].ToString() != " " && targetMsg[index].ToString() != "?" &&
                targetMsg[index].ToString() != "." && targetMsg[index].ToString() != "*")
            {
                SoundManager.Instance.SFXTextPlay(txtsound, txtId);
            }

            // ���� �߰�
            currentText += targetMsg[index];
            msgText.text = currentText; // �ؽ�Ʈ ������Ʈ
            index++;

            // ���� �ð� ����
            float delay = (index < targetMsg.Length && targetMsg[index - 1].ToString() == " ") ? 0.02f : 1f / CharPerSeconds;
            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null; // Ÿ���� �Ϸ� �� null�� ����
        EffectEnd();
    }



    private void EffectEnd()
    {
        typingCoroutine = null; // �ڷ�ƾ�� ����Ǿ����� ��Ȯ�� ����
        onEffectEndCallback?.Invoke();
        if (DialogueManager.Instance.currentNPC != null)
        {
            DialogueManager.Instance.currentNPC.SetExpression("Default");
        }
    }
    private void OnDisable()
    {
        if (msgText != null)
        {
            msgText.text = "";
        }
    }
}

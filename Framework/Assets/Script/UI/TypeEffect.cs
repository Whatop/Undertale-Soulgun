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
    private Coroutine typingCoroutine;
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
                txtsound = "SND_TXT1";
                txtId = 1;
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
                txtsound = "SND_TXT1";
                txtId = 1;
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

        while (index < targetMsg.Length)
        {
            // ���ο� �ܾ ������ ���۵� �� ǥ�� ���� (index�� 0�̰ų� ���� ���ڰ� ������ ��)
            if (!isExpressionSet && (index == 0 || targetMsg[index - 1] == ' '))
            {
                if (currentExpression != null && DialogueManager.Instance.currentNPC != null)
                {
                    DialogueManager.Instance.currentNPC.SetExpression(currentExpression);
                    isExpressionSet = true; // �ߺ� ȣ�� ����
                }
            }

            // �±� ���� ���� �� ó��
            if (targetMsg[index] == '<')
            {
                int closeIndex = targetMsg.IndexOf('>', index);
                if (closeIndex != -1)
                {
                    string tag = targetMsg.Substring(index, closeIndex - index + 1);
                    msgText.text += tag;
                    index = closeIndex + 1;
                }
            }
            else
            {
                // ���� ���� ���
                if (targetMsg[index].ToString() != " " && targetMsg[index].ToString() != "?" &&
                    targetMsg[index].ToString() != "." && targetMsg[index].ToString() != "*")
                {
                    SoundManager.Instance.SFXTextPlay(txtsound, txtId);
                }

                msgText.text += targetMsg[index];
                index++;
            }

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

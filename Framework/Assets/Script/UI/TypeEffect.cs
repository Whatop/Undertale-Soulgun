using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds = 10; // �ʴ� ���� ��
    private string targetMsg;
    public TextMeshProUGUI msgText;
    private int index;
    public int txtId = 0;
    private System.Action onEffectEndCallback;
    private string txtsound = "SND_TXT1";
    public Coroutine typingCoroutine;
    // ���� �ڵ� ����
    private string currentExpression; // ���� ǥ�� ���� ����

    public bool isTextSfxEnable = true; // �ؽ�Ʈ SFX on/off ����
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

    public void SetMsg(string msg, System.Action onEffectEnd, float textspeed, int eventNumber, string expression = null)
    {
        targetMsg = msg;
        onEffectEndCallback = onEffectEnd;
        currentExpression = expression; // ���� ǥ�� ������ ����
        CharPerSeconds = textspeed;


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
            case 2:
                txtsound = "voice_flowey_1";
                txtId = 17;
                break;
            case 3:
                txtsound = "voice_flowey_2";
                txtId = 18;
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
        bool isExpressionSet = false;
        string currentText = "";

        while (index < targetMsg.Length)
        {
            if (!isExpressionSet && (index == 0 || targetMsg[index - 1] == ' '))
            {
                if (currentExpression != null && DialogueManager.Instance.currentNPC != null)
                {
                    DialogueManager.Instance.currentNPC.SetExpression(currentExpression);
                    isExpressionSet = true;
                }
            }

            if (targetMsg[index] == '<')
            {
                int closeIndex = targetMsg.IndexOf('>', index);
                if (closeIndex != -1)
                {
                    string tag = targetMsg.Substring(index, closeIndex - index + 1);
                    currentText += tag;
                    msgText.text = currentText;
                    index = closeIndex + 1;
                    continue;
                }
            }

            char currentChar = targetMsg[index];

            // ���� ���
            if (isTextSfxEnable && currentChar != ' ' && currentChar != '?' && currentChar != '.' && currentChar != '*')
            {
                if(isTextSfxEnable)
                    SoundManager.Instance.SFXTextPlay(txtsound, txtId);
                else
                    SoundManager.Instance.SFXErrorTextPlay(txtsound, txtId);
                

            }
            

            currentText += currentChar;
            msgText.text = currentText;
            index++;

            // �ٳѱ� ������ ��� �ణ�� �߰� ������
            if (currentChar == '\n')
            {
                yield return new WaitForSeconds(0.2f); // �ٳѱ� ������ �߰�
            }
            else
            {
                yield return new WaitForSeconds(1f / CharPerSeconds);
            }
        }

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
        isTextSfxEnable = true;
    }
    private void OnDisable()
    {
        if (msgText != null)
        {
            msgText.text = "";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeEffect : MonoBehaviour
{
    public float CharPerSeconds = 10f; // �ʴ� ���� ��
    private string targetMsg;
    public TextMeshProUGUI msgText;
    private float interval;
    private int index;
    public int txtId = 0;
    private System.Action onEffectEndCallback;
    private string txtsound = "SND_TXT1";

    private void Awake()
    {
        msgText = GetComponent<TextMeshProUGUI>();
    }

    public void SetMsg(string msg, System.Action onEffectEnd)
    {
        targetMsg = msg;
        onEffectEndCallback = onEffectEnd;
        EffectStart();
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
        EffectStart();
    }

    public void Skip()
    {
        CancelInvoke("Effecting"); // ��� ȣ�� ���
        msgText.text = targetMsg;
        EffectEnd();
    }

    private void EffectStart()
    {
        CancelInvoke("Effecting"); // ������ ȣ���� ����Ͽ� �ߺ� ����
        msgText.text = "";
        index = 0;
        interval = 1.0f / CharPerSeconds;
        Invoke("Effecting", interval);
    }

    private void Effecting()
    {
        if (index >= targetMsg.Length)
        {
            EffectEnd();
            return;
        }

        // �±׸� ó���ϱ� ���� ���� �߰�
        if (targetMsg[index] == '<')
        {
            // �±װ� ���� ������ ���ڿ��� ��� �߰�
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
            // ���� ����: ���� ���� �߰�
            if (index < targetMsg.Length && targetMsg[index].ToString() != " " && targetMsg[index].ToString() != "?" && targetMsg[index].ToString() != "." && targetMsg[index].ToString() != "*")
            {
                SoundManager.Instance.SFXTextPlay(txtsound, txtId);
            }

            msgText.text += targetMsg[index];
            index++;
        }

        // ���� ȣ�� ��� �� ���ο� ȣ�� ����
        CancelInvoke("Effecting");
        float delay = targetMsg[index - 1].ToString() == " " ? 0.02f : 1f / CharPerSeconds;
        Invoke("Effecting", delay);
    }

    private void EffectEnd()
    {
        onEffectEndCallback?.Invoke();
    }
}

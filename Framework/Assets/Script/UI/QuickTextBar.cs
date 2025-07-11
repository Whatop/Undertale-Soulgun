using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuickTextBar : MonoBehaviour
{
    public int txtId = 0;                  // ����� �ؽ�Ʈ ID
    public float charPerSec = 10f;         // Ÿ���� �ӵ�
    public TypeEffect typeEffect;
    public Transform target; // ���� ��� (�÷��̾�)
    public Vector3 offset = new Vector3(0, 1.65f, 0); // ��ġ ������
    public bool IsBusy { get; private set; } = false;

    private void Awake()
    {
        if (typeEffect == null)
            typeEffect = GetComponentInChildren<TypeEffect>();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    public void ShowMessage(string msg, float delay = 0f)
    {
        gameObject.SetActive(true);
        IsBusy = true;

        typeEffect.CharPerSeconds = charPerSec;
        typeEffect.txtId = txtId;

        typeEffect.SetMsg(msg, () =>
        {
            if (delay > 0f)
                StartCoroutine(HideAfterDelay(delay));
            else
            {
                gameObject.SetActive(false);
                IsBusy = false;
            }
        });
    }
    public void ShowErrorMessage(string msg, float delay = 0f)
    {
        gameObject.SetActive(true);
        IsBusy = true;

        typeEffect.CharPerSeconds = charPerSec;
        typeEffect.txtId = txtId;

        // [1] �Ϲ� �ؽ�Ʈ SFX OFF
        typeEffect.isTextSfxEnable = false;

        typeEffect.SetMsg(msg, () =>
        {
            if (delay > 0f)
                StartCoroutine(HideAfterDelay(delay));
            else
            {
                gameObject.SetActive(false);
                IsBusy = false;
            }
        });
    }


    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
         IsBusy = false;
    }
}

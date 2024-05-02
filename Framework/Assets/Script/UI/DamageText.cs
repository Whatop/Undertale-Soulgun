using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float speed = 1f; // �̵� �ӵ�
    public float destroyTime = 1.5f; // �ؽ�Ʈ�� ������� �ð�

    private TextMeshProUGUI damageText;
    private Vector3 moveDirection;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int damageAmount)
    {
        damageText.text = "-" + damageAmount;
        moveDirection = Vector3.up; // �ؽ�Ʈ �̵� ���� (��������)
        Destroy(gameObject, destroyTime); // ������ �ð� �Ŀ� �ؽ�Ʈ ����
    }

    private void Update()
    {
        // �ؽ�Ʈ�� �̵���Ŵ
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageText : MonoBehaviour
{
    public bool isObject;
    
    private float speed = 0.75f; // �̵� �ӵ�
    private float destroyTime = 1f; // �ؽ�Ʈ�� ������� �ð�
    private TextMeshProUGUI damageText;

    private Vector3 moveDirection;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();

    }

    public void Initialize(int damageAmount)
    {
        if (!isObject)
        {
            damageText.text = "<fade d=0>" + damageAmount;
            moveDirection = Vector3.up; // �ؽ�Ʈ �̵� ���� (��������)
        }
        else
        {
            speed = 1.5f;
            damageText.text = "<fade d=0>" + damageAmount;
            moveDirection = Vector3.right; 
            moveDirection += Vector3.up;
        }
        Destroy(gameObject, destroyTime); // ������ �ð� �Ŀ� �ؽ�Ʈ ����
    }

    private void Update()
    {
        // �ؽ�Ʈ�� �̵���Ŵ
        if(!isObject)
        transform.Translate(moveDirection * speed * Time.deltaTime);
        else
        {
            if (moveDirection.y > 0)
                moveDirection -= Vector3.up * Time.deltaTime;
            else
                moveDirection.y = 0;
        transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }
}

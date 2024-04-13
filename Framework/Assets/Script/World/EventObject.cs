using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public int EvnetNumber; // ��Ż�� ��ȣ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.OnEventEnter(this); // ��Ż ���� �̺�Ʈ ȣ��
        }
    }
}
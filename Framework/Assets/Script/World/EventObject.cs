using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObject : MonoBehaviour
{
    public int EventNumber; // ��Ż�� ��ȣ


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.Instance.TriggerEvent(EventNumber);
        }
    }
}
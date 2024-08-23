using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance; // �̱��� �ν��Ͻ�
    public bool isEvent = false;

    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("EventManager");
                    instance = obj.AddComponent<EventManager>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TriggerEvent(int eventNumber)
    {
        isEvent = true;
        DialogueManager.Instance.StartDialogue(eventNumber); // NPC 0������ ��ȭ ����

    }
    public void OnEventEnter(EventObject eventObject)
    {
      
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageScript : MonoBehaviour
{
    private Image image;
    public Sprite[] sprites; // ������Ʈ�� �ִ� ��������Ʈ���� ������ �迭

    void Start()
    {
        // �̹��� ������Ʈ�� �����ɴϴ�.
        image = GetComponent<Image>();
    }

    public void SetImage(int index)
    {
        // ��ȿ�� �ε��� ���� ���� �ִ��� Ȯ���մϴ�.
        if (index >= 0 && index < sprites.Length)
        {
            // �ش� �ε����� �ش��ϴ� ��������Ʈ�� �̹����� �����մϴ�.
            image.sprite = sprites[index];
        }
        else
        {
            Debug.LogError("Index out of range.");
        }
    }
}

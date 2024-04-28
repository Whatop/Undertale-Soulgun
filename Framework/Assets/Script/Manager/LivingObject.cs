using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingObject : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected bool isnpc; // ������!

    protected GameManager gameManager;

    protected Rigidbody2D rigid;
    protected Animator animator;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        // ���� �Ŵ������� �÷��̾� �����͸� �����ͼ� ����
        PlayerData playerData = gameManager.GetPlayerData();
        health = playerData.health;
        // ������ �ʿ��� �����͵� �����ͼ� ����
    }

    public virtual void Die()
    {
        Debug.Log("Object died!");
        // ���⿡ ���� ���� ������ �����մϴ�.
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Move(Vector3 direction)
    {
        if (!isnpc)
        {
            //������� ������ AI �ۼ�
        }
        

    }

    public void Init()
    {
    }
}

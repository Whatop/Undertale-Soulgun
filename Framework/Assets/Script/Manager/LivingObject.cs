using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObjectState
{
    Down,
    Up,
    Side,
    Angle,
    Roll,
    None
}
public class LivingObject : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected bool isnpc; // ������!

    protected bool isInvincible = false; // ���� �������� ����
    protected float invincibleDuration = 1.5f; // ���� ���� �ð�
    protected float invincibleTimer = 0f; // ���� Ÿ�̸�


    protected GameManager gameManager;

    protected PlayerData playerData;
    protected Rigidbody2D rigid;
    protected Animator animator;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        // ���� �Ŵ������� �÷��̾� �����͸� �����ͼ� ����
        playerData = gameManager.GetPlayerData();
        health = playerData.health;
        // ������ �ʿ��� �����͵� �����ͼ� ����
    }

    private void Update()
    {
        gameManager.SavePlayerData(playerData);
    }
    public virtual void Die()
    {
        Debug.Log("Object died!");
        // ���⿡ ���� ���� ������ �����մϴ�.
    }

    public void TakeDamage(int damageAmount) //������ ?
    {
        if (!isInvincible) // ���� ���°� �ƴ� ���� �������� ����
        {
            UIManager.Instance.ShowDamageText(transform.position, damageAmount);
            health -= damageAmount;
            if (health <= 0)
            {
                Die();
            }
        }
    }
    public void TakeDamage(int damageAmount, Vector3 position)// �����̴�?
    {
        if (!isInvincible) // ���� ���°� �ƴ� ���� �������� ����
        {
            UIManager.Instance.ShowDamageText(position, damageAmount);
            health -= damageAmount;
            if (health <= 0)
            {
                Die();
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    protected int maxHealth;
    protected float speed;
    protected bool isnpc; // ������!

    protected bool isInvincible = false; // ���� �������� ����
    protected float invincibleDuration = 1.5f; // ���� ���� �ð�
    protected float invincibleTimer = 0f; // ���� Ÿ�̸�

    protected Transform healthBarTransform; // ü�¹��� Transform
    public GameObject healthBarPrefab; // ü�¹� ������
    protected GameObject healthBar; // �ν��Ͻ�ȭ�� ü�¹�
    protected Slider healthSlider; // ü�¹� �����̴�

    protected GameManager gameManager;
    protected PlayerData playerData;
    protected Rigidbody2D rigid;
    protected Animator animator;

    public Canvas worldCanvas; // ���� ĵ����
    private Camera mainCamera;
    public GameObject hpBarTransfrom;
    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        playerData = gameManager.GetPlayerData();
        maxHealth = playerData.health; // �ִ� ü�� ����
        health = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = cameraObject.GetComponent<Camera>();

        // ü�¹� �ʱ�ȭ
        InitializeHealthBar();
    }

    protected void InitializeHealthBar()
    {
        if (healthBarPrefab != null && worldCanvas != null)
        {
            healthBar = Instantiate(healthBarPrefab, worldCanvas.transform);
            healthBarTransform = healthBar.transform;
            healthSlider = healthBar.GetComponentInChildren<Slider>();

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = health;
            }

        healthBarTransform.position = hpBarTransfrom.transform.position;
        }
    }

    private void Update()
    {
        // ü�¹� ��ġ ������Ʈ
        if (healthBar != null)
        {
            healthBarTransform.position = hpBarTransfrom.transform.position;
        }

        gameManager.SavePlayerData(playerData);
    }

    public virtual void Die()
    {
        Debug.Log("Object died!");
        Destroy(healthBar); // ü�¹� ����
    }

    public void TakeDamage(int damageAmount)
    {
        if (!isInvincible) // ���� ���°� �ƴ� ���� �������� ����
        {
            UIManager.Instance.ShowDamageText(transform.position, damageAmount);
            health -= damageAmount;

            // ü�¹� ������Ʈ
            if (healthSlider != null)
            {
                healthSlider.value = health;
            }

            if (health <= 0)
            {
                Die();
            }
        }
    }

    public void TakeDamage(int damageAmount, Vector3 position)
    {
        if (!isInvincible) // ���� ���°� �ƴ� ���� �������� ����
        {
            UIManager.Instance.ShowDamageText(position, damageAmount);
            health -= damageAmount;

            // ü�¹� ������Ʈ
            if (healthSlider != null)
            {
                healthSlider.value = health;
            }

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
            // ���� ���� ������ AI �ۼ�
        }
    }

    public void Init()
    {
    }
}

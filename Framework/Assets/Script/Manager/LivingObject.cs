using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    protected float health;
    protected float maxHealth;
    protected float speed;
    protected bool isnpc; // ������!
    protected bool isDie = false;

    protected bool isInvincible = false; // ���� ���� ����
    protected float invincibleDuration = 0.6f; // ���� ���� �ð� (0.6��)
    protected float invincibleTimer = 0f; // ���� ������ Ÿ�̸�

    protected Transform healthBarTransform; // ü�¹��� Transform
    public GameObject healthBarPrefab; // ü�¹� ������
    protected GameObject healthBar; // �ν��Ͻ�ȭ�� ü�¹�
    protected Slider healthSlider; // ü�¹� �����̴�
    protected TextMeshProUGUI healthText; // ü�¹� �ؽ�Ʈ

    protected GameManager gameManager;
    protected PlayerData playerData;
    protected Rigidbody2D rigid;
    protected Animator animator;

    public Canvas worldCanvas; // ���� ĵ����
    private Camera mainCamera;
    public GameObject hpBarPoint;
    protected SpriteRenderer spriteRenderer; // SpriteRenderer ����
    private Color originalColor; // ���� ���� ����

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        mainCamera = cameraObject.GetComponent<Camera>(); 
        // �ڽ� �߿� "Soul" �̸��� ���� ������Ʈ���� SpriteRenderer�� ������
        GameObject soulObj = GameObject.FindGameObjectWithTag("Soul");
        if (soulObj != null)
            spriteRenderer = soulObj.GetComponent<SpriteRenderer>();
        else
            Debug.LogWarning("�ڽĿ� 'Soul' ������Ʈ�� �����ϴ�!");
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
            healthText = healthBar.GetComponentInChildren<TextMeshProUGUI>();

            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = health;
            }
            if (healthText != null)
            {
                healthText.text = maxHealth + "/" + health;
            }

            healthBarTransform.position = hpBarPoint.transform.position;
        }
    }

    protected virtual void Update()
    {
        // ���� ���� Ÿ�̸� ó��
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer >= invincibleDuration)
            {
                isInvincible = false; // ���� ���� ����
                invincibleTimer = 0f;
            }
        }

        // ü�¹� ��ġ ������Ʈ
        if (healthBar != null)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = health;
            }
            if (healthText != null)
            {
                healthText.text = maxHealth + "/" + health;
            }
            healthBarTransform.position = hpBarPoint.transform.position;
        }

        if (transform.tag == "Player")
        {
            gameManager.SavePlayerData(playerData);
        }
    }

    public void IncreaseHealth(int v)
    {
        if (health + v < maxHealth)
            health += v;
        else
            health = maxHealth;

    }
    public virtual void Die()
    {
        isDie = true;
        Debug.Log("Object died!");
        //Destroy(healthBar); // ü�¹� ����

        if (transform.tag == "Player")
        {
            gameManager.Die(); 
        }
        else
        {
            animator.SetBool("isDie",true);
        }
    }

    public virtual void OffHpbar()
    {
        healthBar.SetActive(false);
    }

    public virtual void TakeDamage(DamageInfo info)
    {
        if (!isInvincible) // ���� ���°� �ƴ� ���� �������� ����
        {
            if (info.isHeal)
            {
                Heal(info.amount);
                return;
            }
            UIManager.Instance.ShowDamageText(transform.position, info.amount);
            SoundManager.Instance.SFXPlay("hit", 128);
            health -= info.amount;
            Debug.Log($"{gameObject.name}��(��) {info.amount} ���ظ� ����. ������: {info.attacker?.name}");

            if (healthSlider != null)
            {
                healthSlider.value = health;
            }

            if (health <= 0 && !isDie )
            {
                Die();
            }
            else
            {
                StartCoroutine(StartInvincibility()); // ���� ���·� ��ȯ
            }
            
        }
    }

    public virtual void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        // �ʿ��ϸ� �� ����Ʈ, ���� �� �߰�
        SoundManager.Instance.SFXPlay("heal", 123);

    }
    private IEnumerator StartInvincibility()
    {
        if (transform.tag == "Player")
        {
            if (spriteRenderer == null)
                yield break;

            isInvincible = true; // ���� ���� Ȱ��ȭ
            float elapsed = 0f;

            while (elapsed < invincibleDuration)
            {
                elapsed += Time.deltaTime;

                // ������ ȿ��
                float alpha = Mathf.Abs(Mathf.Sin(elapsed * 10)); // 10�� �����̴� �ӵ�
                SetSpriteAlpha(alpha);

                yield return null;
            }

            SetSpriteAlpha(1f); // ���� ���� ������� ����
            isInvincible = false; // ���� ���� ����
        }
    }

    private void SetSpriteAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
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
    public IEnumerator FadeOutSprite(GameObject targetObject, float duration)
    {
        // targetObject�� SpriteRenderer�� �پ��ִ��� Ȯ��
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            yield break; // ��������Ʈ�� ������ ����
        }

        // ���� ���İ�(���۰�)
        float startAlpha = spriteRenderer.color.a;
        // ��ǥ ���İ�
        float endAlpha = 0f;
        // ��� �ð�
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // 0 ~ 1 ������ ������ t
            float t = currentTime / duration;
            // ���� ������ ���� ���İ� ���
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, t);

            // ���� ���� ���İ� ����
            Color color = spriteRenderer.color;
            color.a = newAlpha;
            spriteRenderer.color = color;

            yield return null;  // ���� �����ӱ��� ���
        }

        // ���������� ���İ��� ��Ȯ�� 0���� ����
        {
            Color finalColor = spriteRenderer.color;
            finalColor.a = 0f;
            spriteRenderer.color = finalColor;
        }
    }

}

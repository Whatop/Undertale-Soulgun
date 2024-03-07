using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // ���� �������� �޾��� �� ȣ��Ǵ� �Լ�
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ���� �׾��� �� ȣ��Ǵ� �Լ�
        // �� ĳ������ ��� ȿ��, ��� ������ ���� ó���մϴ�.
        Destroy(gameObject);
    }
}

using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
    public Weapon EnemyWeapon;          // ���� ��� ���� ���� ����
    public Transform WeaponTransform;  // �� ���� Transform
    GameManager gameManager;
    float shootCoolTime = 4;
    float curTime = 0;


    private void Awake()
    {
        gameManager = GameManager.Instance;
    }
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        curTime += Time.deltaTime;

        if(curTime > shootCoolTime && bulletPrefab != null)
        {
            Shoot();
            curTime = 0;
        }
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
    void Shoot()
    {

        EnemyWeapon = gameManager.GetWeaponData();
        EnemyWeapon.current_magazine = EnemyWeapon.magazine;
        gameManager.SaveWeaponData(EnemyWeapon);
        // �Ѿ��� �����ϰ� �ʱ� ��ġ�� ���� ��ġ�� �����մϴ�.
        GameObject bullet = Instantiate(bulletPrefab, WeaponTransform.position, WeaponTransform.rotation);


        // �Ѿ˿� �ӵ��� �����Ͽ� �߻��մϴ�.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = WeaponTransform.up * bulletSpeed;
    }
    void Die()
    {
        // ���� �׾��� �� ȣ��Ǵ� �Լ�
        // �� ĳ������ ��� ȿ��, ��� ������ ���� ó���մϴ�.
        Destroy(gameObject);
    }
}

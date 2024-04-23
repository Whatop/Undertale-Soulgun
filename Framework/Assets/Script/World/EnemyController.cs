using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
    public Weapon weaponData;          // ���� ��� ���� ���� ����
    public Transform WeaponTransform;  // �� ���� Transform
    GameManager gameManager;
    float shootCoolTime = 4;
    float curTime = 0;

    private bool undying = false;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        weaponData = new Weapon();
    }
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        int curmagazine = weaponData.current_magazine;

        Vector3 playerPosition = gameManager.GetPlayerData().position;
        Vector2 direction = (playerPosition - WeaponTransform.position).normalized;
        WeaponTransform.up = direction;
        curTime += Time.deltaTime;

        if (curTime > shootCoolTime && bulletPrefab != null && curmagazine > 0)
        {
            Shoot();
            weaponData.current_magazine -= 1;
            weaponData.current_Ammo -= 1;
            gameManager.SaveWeaponData(weaponData);
            curTime = 0;
        }


        // �Ѿ��� ������ ������
        if (weaponData.current_Ammo < weaponData.maxAmmo &&
                 weaponData.current_magazine < weaponData.magazine)
        {

            weaponData.current_magazine = weaponData.magazine;
            gameManager.SaveWeaponData(weaponData);
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

        weaponData = gameManager.GetWeaponData();
        weaponData.current_magazine = weaponData.magazine;
        gameManager.SaveWeaponData(weaponData);
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

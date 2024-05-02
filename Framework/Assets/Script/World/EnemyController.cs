using UnityEngine;

public class EnemyController : LivingObject
{
    public int maxHealth = 100;
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
    public Weapon weaponData;          // ���� ��� ���� ���� ����
    public Transform WeaponTransform;  // �� ���� Transform
    public Transform hand;  // �� ���� Transform 
    public ObjectState objectState;

    float shootCoolTime = 4;
    float curTime = 0;

    private bool undying = false;

    protected override void Awake()
    {
        base.Awake(); // LivingObject�� Awake �޼��� ȣ��
        weaponData = new Weapon();
    }
    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        int curmagazine = weaponData.current_magazine;

        Vector3 playerPosition = gameManager.GetPlayerData().position;
        Vector2 direction = (playerPosition - WeaponTransform.position).normalized;
        hand.up = direction;
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

    void Shoot()
    {

        weaponData = gameManager.GetWeaponData();
        weaponData.current_magazine = weaponData.magazine;
        gameManager.SaveWeaponData(weaponData);
        // �Ѿ��� �����ϰ� �ʱ� ��ġ�� ���� ��ġ�� �����մϴ�.
        GameObject bullet = Instantiate(bulletPrefab, WeaponTransform.position, WeaponTransform.rotation);


        // �Ѿ˿� �ӵ��� �����Ͽ� �߻��մϴ�.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = hand.up * bulletSpeed;
    }
}

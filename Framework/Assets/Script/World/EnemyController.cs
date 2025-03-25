using UnityEngine;

public class EnemyController : LivingObject
{
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
    public Weapon weaponData;          // ���� ��� ���� ���� ����
    public Transform WeaponTransform;  // �� ���� Transform
    public Transform hand;  // �� ���� Transform 
    public ObjectState objectState;
    public float minDistance = 3f;  // �÷��̾���� �ּ� ���� �Ÿ�
    public float maxDistance = 6f;  // �÷��̾���� �ִ� ���� �Ÿ�

    public float shootCoolTime = 4;
    float curTime = 0;
    public bool isMove;

    private bool undying = false;

    protected override void Awake()
    {
        base.Awake(); // LivingObject�� Awake �޼��� ȣ��
        weaponData = new Weapon();
        //animator.GetComponent<Animator>();
    }

     void Start()
    {
        maxHealth = 10;
        health = maxHealth;
        speed = 2;
    }

    protected override void Update()
    {
        base.Update();
        if (!isDie)
        {
            float distanceToPlayer = Vector2.Distance(gameManager.GetPlayerData().position, transform.position);

            if (distanceToPlayer > maxDistance && isMove)
            {
                ChasePlayer();
            }
            else if (distanceToPlayer < minDistance)
            {
                MoveAwayFromPlayer();
            }
            else
            {
                StopMoving();
            }

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
                curTime = 0;
                SoundManager.Instance.SFXPlay("shotgun_shot_01", 218); // �� ����

            }

            // �Ѿ��� ������ ������
            if (weaponData.current_Ammo < weaponData.maxAmmo &&
                weaponData.current_magazine < weaponData.magazine)
            {
                weaponData.current_magazine = weaponData.magazine;
            }
        }
        else
            StopMoving();

    }

    void ChasePlayer()
    {
        Vector2 direction = (gameManager.GetPlayerData().position - transform.position).normalized;
        rigid.velocity = direction * speed;
    }

    void MoveAwayFromPlayer()
    {
        Vector2 direction = (transform.position - gameManager.GetPlayerData().position).normalized;
        rigid.velocity = direction * speed;
    }

    void StopMoving()
    {
        rigid.velocity = Vector2.zero;
    }

    void Shoot()
    {
       // weaponData.current_magazine = weaponData.magazine;
       //
       // GameObject bullet = BattleManager.Instance.GetBulletFromPool();
       // if (bullet == null) return;
       //
       // bullet.transform.position = WeaponTransform.position;
       // bullet.transform.rotation = WeaponTransform.rotation;
       //
       // Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
       // bulletRb.velocity = hand.up * bulletSpeed;
       //
       // BulletController bulletController = bullet.GetComponent<BulletController>();
       // if (bulletController != null)
       // {
       //     bulletController.isFreind = false;
       //     bulletController.InitializeBullet(hand.up, bulletSpeed, 0f, weaponData.damage, 15f); // ����
       // }
        // weaponData.current_magazine = weaponData.magazine;
        //
        // // �Ѿ��� �����ϰ� �ʱ� ��ġ�� ���� ��ġ�� �����մϴ�.
        // GameObject bullet = Instantiate(bulletPrefab, WeaponTransform.position, WeaponTransform.rotation);
        //
        // // �Ѿ˿� �ӵ��� �����Ͽ� �߻��մϴ�.
        // Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        // bulletRb.velocity = hand.up * bulletSpeed;
    }
}

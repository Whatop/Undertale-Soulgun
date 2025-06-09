using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum EnemyAttackType
{
    Melee,       // ������
    Laser,       // ������ �߻� (Gaster Blaster��)
    Bullet,      // �Ϲ� źȯ
    Sniper,      // ���� �� ���� źȯ
    Shotgun,     // ��ź
    Buff,        // ����/����/��ȭ
    Predictive,  // ���� ���
    Trap_Laser,  // ��ġ�� ����
    Trap_Bullet, 
    Trap_Melee,
    Undying,     // ���� ���� (�һ���, �����)
    Special,      // ��Ÿ Ư��
    None
}
public enum TrapDir { 
    Left,
    LeftUp,
    LeftDown,
    Right,
    RightUp,
    RightDown,
    Up,
    Down,
    None
}

public class EnemyController : LivingObject
{
    // ���Ӱ� ����� ������ �̸� (���� ����, ���ο��� �ڵ� ó��)
    [SerializeField] private string bulletPrefabName = "Enemy_None";
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
    public EnemyAttackType attackType;
    public TrapDir dir;

    [Header("�ӽ� ü��")]
    public float testhp = 10000;


    [Header("Ʈ�� ����")]
    public bool isTrapActive = true;     // Ʈ�� Ȱ��ȭ ����
    public float trapShootInterval = 2f; // Ʈ�� �߻� �ֱ�
    private float trapTimer = 0f;        // Ʈ���� Ÿ�̸�
    [Header("������ ����")]
    public GameObject laserPrefab; // LaserFadeOut ������
    private GameObject currentLaser; // ���� ������ ������
    public bool iskeepLaser;

    private bool undying = false;

    protected override void Awake()
    {
        base.Awake(); // LivingObject�� Awake �޼��� ȣ��
        //animator.GetComponent<Animator>();
    }

     void Start()
    {
        maxHealth = testhp;
        health = maxHealth;
        speed = 2;
        weaponData = new Weapon();
        if (IsTrapType())
        {
            RotateToTrapDirection(); // Ʈ���� ��� ���� ȸ��
        }
    }

    protected override void Update()
    {
        base.Update();
        if (!isDie)
        {
            if (attackType == EnemyAttackType.None)
                return;
            // Ʈ���� �÷��̾� �߰����� ����
            if (!IsTrapType())
            {
                float distanceToPlayer = Vector2.Distance(gameManager.GetPlayerData().position, transform.position);

                if (distanceToPlayer > maxDistance && isMove)
                    ChasePlayer();
                else if (distanceToPlayer < minDistance)
                    MoveAwayFromPlayer();
                else
                    StopMoving();
            }
            else
            {
                StopMoving(); // Ʈ���� ����
            }

            if (IsTrapType())
            {
                if (!isTrapActive) return;

                trapTimer += Time.deltaTime;
                if (trapTimer >= trapShootInterval)
                {
                    trapTimer = 0f;
                    Shoot(); // Ʈ���� Shoot()�� �̿���
                }
            }
            else
            {
                float curmagazine = weaponData.current_magazine;
                curTime += Time.deltaTime;

                Vector3 playerPosition = gameManager.GetPlayerData().position;
                Vector2 direction = (playerPosition - WeaponTransform.position).normalized;
                hand.up = direction;

                if (curTime > shootCoolTime && curmagazine > 0)
                {
                    Shoot();
                    weaponData.current_magazine -= 1;
                    weaponData.current_Ammo -= 1;
                    curTime = 0;
                }

                // ź�� ������
                if (weaponData.current_Ammo < weaponData.maxAmmo &&
                    weaponData.current_magazine < weaponData.magazine)
                {
                    weaponData.current_magazine = weaponData.magazine;
                }
            }
        }
        else
            StopMoving();

    }

    bool IsTrapType()
    {
        return attackType == EnemyAttackType.Trap_Bullet ||
               attackType == EnemyAttackType.Trap_Laser ||
               attackType == EnemyAttackType.Trap_Melee;
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
        string prefabName = GetBulletPrefabName(); // Ÿ�Կ� ���� �����ո� ��������

        Vector2 spawnPos = WeaponTransform.position;
        Quaternion spawnRot = WeaponTransform.rotation;
        Vector2 direction = hand.up;

        if (attackType != EnemyAttackType.Trap_Laser && attackType != EnemyAttackType.Laser)
        {
            BattleManager.Instance.SpawnBulletAtPosition(
                GetBulletType(),          // �Ѿ� ���� enum
                spawnPos,
                spawnRot,
                direction,
                prefabName,
                0,      // size
                0f,     // delay
                false,  // isFriend
                5f,     // maxRange
                bulletSpeed,
                1f,     // accuracy
                1f      // damage
            );
                    SoundManager.Instance.SFXPlay("shotgun_shot_01", 218);
            weaponData.current_magazine = weaponData.magazine;
        }
        else
        {
            FireLaser();
        }

    }
    void FireLaser()
    {
        if (currentLaser != null) return;

        currentLaser = Instantiate(laserPrefab, WeaponTransform.position, Quaternion.identity);
        LaserFadeOut laser = currentLaser.GetComponent<LaserFadeOut>();

        if (laser != null)
        {
            laser.laserOrigin = WeaponTransform;
            laser.obstacleMask = LayerMask.GetMask("Wall", "Barrier", "Player");
            laser.thickness = 0.6f;
            laser.growSpeed = 50f;
            laser.fadeDuration = 0.5f;
            laser.dotInterval = 0.2f;
            laser.autoFade = iskeepLaser; //  �ڵ� ���̵� ����
            laser.enabled = true;
        }

        currentLaser.transform.up = GetDirectionFromTrapDir(dir);

        SoundManager.Instance.SFXPlay("charge", 63);
        SoundManager.Instance.SFXPlayLoop(226, 0.05f); //  �ݺ� ���� ���
    }


    IEnumerator DisableLaserAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (currentLaser != null)
        {
         //    LaserFadeOut laser = currentLaser.GetComponent<LaserFadeOut>();
            currentLaser = null;
        }
    }
    public void DeactivateLaser()
    {
        if (currentLaser != null)
        {
            SoundManager.Instance.SFXStopLoop(226); //  �ݺ� ���� ����
            currentLaser = null;
        }
    }

    BulletType GetBulletType()
    {
        switch (attackType)
        {
            case EnemyAttackType.Bullet:
                return BulletType.Normal;
            case EnemyAttackType.Shotgun:
                return BulletType.Normal;
            case EnemyAttackType.Laser:
            case EnemyAttackType.Trap_Laser:
                return BulletType.Laser;
            case EnemyAttackType.Predictive:
                return BulletType.Speed;
            case EnemyAttackType.Trap_Bullet:
            case EnemyAttackType.Trap_Melee:
                return BulletType.Directional;
            default:
                return BulletType.Normal;
        }
    }
    string GetBulletPrefabName()
    {
        // �켱 ���⿡ �̸��� �����Ǿ� ������ �װ� ����, ������ Ÿ������ �б�

        switch (attackType)
        {
            case EnemyAttackType.Laser:
            case EnemyAttackType.Trap_Laser:
                return "Laser_Enemy";

            case EnemyAttackType.Trap_Bullet:
            case EnemyAttackType.Bullet:
                return "Enemy_None";

            case EnemyAttackType.Shotgun:
                return "Enemy_None";

            case EnemyAttackType.Sniper:
                return "Enemy_None";

            default:
                return "Enemy_None";
        }
    }
    Vector2 GetDirectionFromTrapDir(TrapDir dir)
    {
        Vector2 direction;

        switch (dir)
        {
            case TrapDir.Left: direction = Vector2.left; break;
            case TrapDir.LeftUp: direction = new Vector2(-1, 1).normalized; break;
            case TrapDir.LeftDown: direction = new Vector2(-1, -1).normalized; break;
            case TrapDir.Right: direction = Vector2.right; break;
            case TrapDir.RightUp: direction = new Vector2(1, 1).normalized; break;
            case TrapDir.RightDown: direction = new Vector2(1, -1).normalized; break;
            case TrapDir.Up: direction = Vector2.up; break;
            case TrapDir.Down: direction = Vector2.down; break;
            default: direction = Vector2.up; break;
        }

        return direction;
    }

    void RotateToTrapDirection()
    {
        Vector2 dirVector = GetDirectionFromTrapDir(dir);
        if (dirVector == Vector2.zero) return;

        // ������ Vector2.right �� ������ Vector2.up �������� ȸ�� ���
        float angle = Vector2.SignedAngle(Vector2.up, dirVector);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}

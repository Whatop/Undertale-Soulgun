using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;  // AnimatorControllerParameterType ����� ����

public enum VirtueType
{
    Bravery,       // ���
    Justice,       // ����
    Integrity,     // ���
    Kindness,      // ģ��
    Perseverance,  // ����
    Patience,      // �γ�
    Determination  // ����
}

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
    [Header("������")]
    [SerializeField] private EnemyData enemyData;  // �ν����Ϳ��� �Ҵ�
    public VirtueType virtue; // �� ���Ϳ� �ϳ��� �Ҵ�
    [SerializeField]
    private List<string> reactableEmotions = new List<string>();



    [Header("Target Indicator")]
    [Tooltip("���� ����� �� ǥ�ÿ� ������ (�ƿ����� + ��Ʈ)")]
    private GameObject outlineObject;
    [SerializeField] private GameObject outlineHeart;
    private SpriteRenderer outlineSpriteRenderer;
    public Material outlineMaterial; // �ܰ��� Material

    [Header("�ӽ� ü��")]
    public float testhp = 100;


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
                      //
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        weaponData = new Weapon();
        if (enemyData != null)
        {
            // �⺻ ���� ����
            virtue = enemyData.virtue;
            reactableEmotions = new List<string>(enemyData.reactableEmotions);

            maxHealth = enemyData.maxHealth;
            health = maxHealth;
            speed = enemyData.moveSpeed;
            shootCoolTime = enemyData.shootCooldown;
            bulletPrefabName = enemyData.bulletPrefabName;

            // Ʈ�� ����
            isTrapActive = enemyData.isTrapActive;
            trapShootInterval = enemyData.trapShootInterval;

            // �Ÿ� ����
            minDistance = enemyData.minDistance;
            maxDistance = enemyData.maxDistance;

            Debug.Log($"[{gameObject.name}] EnemyData ���� �Ϸ�");
        }
        else
        {
            Debug.LogWarning("EnemyData�� �Ҵ���� �ʾҽ��ϴ�.");
        }
        CreateOutline(); // ���̶���Ʈ�� �ܰ��� ������Ʈ ����
        if (IsTrapType())
        {
            RotateToTrapDirection(); // Ʈ���� ��� ���� ȸ��
        }
    }
    // �ܰ��� ������Ʈ ����
    void CreateOutline()
    {
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one * 1.1f; // ���� ũ�⺸�� �ణ ũ�� ����

        outlineSpriteRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.color = Color.yellow;
        outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        outlineSpriteRenderer.material = outlineMaterial; // Material�� �ܰ��� Material�� ����
        outlineSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1; // NPC���� �ڿ� ǥ�õǵ��� ���� ���� ����

        outlineObject.SetActive(false); // ó������ ��Ȱ��ȭ
    }
    /// <summary>
    /// �÷��̾ ������ ���� ǥ���� �޾��� �� ȣ��
    /// </summary>
    /// <param name="emotion">���� Ű���� (��: "Mercy", "Anger" ��)</param>
    public void ReceiveEmotion(string emotion)
    {
        // 1) ���� ����Ʈ ���
        Vector3 effectPos = transform.position + Vector3.up * 1.5f;
        // ����Ʈ Ǯ�� "Emotion_Mercy", "Emotion_Anger" �� �̸����� �̸� ����� �μ���
       // EffectManager.Instance.SpawnEffect($"Emotion_{emotion}", effectPos, Quaternion.identity); :contentReference[oaicite: 1]{ index = 1}

        // 2) �ִϸ����� Ʈ���� ����
        if (animator != null && HasTrigger(animator, emotion))
            animator.SetTrigger(emotion);

        // 3) �߰� ����: ���� ��� ������ ���� ü�� ����/����, ���� �̻� ���� �� ����
    }

    // Animator�� �ش� Ʈ���Ű� �ִ��� Ȯ�� (NPC.cs�� HasTrigger ����) :contentReference[oaicite:2]{index=2}
    private bool HasTrigger(Animator anim, string triggerName)
    {
        foreach (var param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
                return true;
        }
        return false;
    }
    /// <summary>
    /// BattleManager ���� ȣ���ϴ� ���� ���� ��ȣ�ۿ� �޼���
    /// </summary>
    /// <param name="emotion">��: "SpeedUp", "DamageDown", "Mercy", "Flee"</param>
    public void ProcessEmotion(string emotion)
    {
        // ������ ������ ���� ����Ʈ�� ���ٸ� ����
        if (!reactableEmotions.Contains(emotion)) return;

        Debug.Log($"[{gameObject.name}] {emotion} ���� ���ŵ� / �̴�: {virtue}");

        switch (virtue)
        {
            case VirtueType.Bravery: // ���
                if (emotion == "Flirt" || emotion == "Anger")
                {
                    ReceiveEmotion(emotion);
                    // �Ͻ������� �̵��ӵ� ����
                    StartCoroutine(TempSpeedUp(1.5f, 2f));
                }
                break;

            case VirtueType.Justice: // ����
                if (emotion == "Respect" || emotion == "Disgust")
                {
                    ReceiveEmotion(emotion);
                    // ź�� ����
                    bulletSpeed += 2f;
                    Invoke(nameof(ResetBulletSpeed), 2f);
                }
                break;

            case VirtueType.Integrity: // ���
                if (emotion == "Affirm" || emotion == "Deny")
                {
                    ReceiveEmotion(emotion);
                    // �����ð� ���� (��: ���� ����)
                }
                break;

            case VirtueType.Kindness: // ģ��
                if (emotion == "Mercy" || emotion == "Pray")
                {
                    ReceiveEmotion(emotion);
                    // ü�� ���� ȸ��
                    Heal(10f);
                }
                break;

            case VirtueType.Perseverance: // ����
                if (emotion == "Sorrow" || emotion == "Fear")
                {
                    ReceiveEmotion(emotion);
                    // �׾��� ��� 1ȸ ��Ȱ
                    if (health <= 0 && !undying)
                    {
                        undying = true;
                        health = maxHealth * 0.3f;
                        Debug.Log($"{gameObject.name}�� ����� �ٽ� �Ͼ���ϴ�.");
                    }
                }
                break;

            case VirtueType.Patience: // �γ�
                if (emotion == "Ignore" || emotion == "Truth")
                {
                    ReceiveEmotion(emotion);
                    // 2�ʰ� ���� ����
                    StopAllCoroutines();
                    StartCoroutine(DelayAttack(2f));
                }
                break;

            case VirtueType.Determination: // ����
                if (emotion == "Love" || emotion == "Respect")
                {
                    ReceiveEmotion(emotion);
                    // ü�� 50% ������ ��� ��� �ɷ� ��ȭ
                    if (health < maxHealth * 0.5f)
                    {
                        speed *= 1.5f;
                        bulletSpeed += 3f;
                        shootCoolTime *= 0.8f;
                        Debug.Log($"{gameObject.name} ������ ��Ÿ������!");
                    }
                }
                break;
        }
    }
    IEnumerator TempSpeedUp(float multiplier, float duration)
    {
        speed *= multiplier;
        yield return new WaitForSeconds(duration);
        speed /= multiplier;
    }
     
    IEnumerator DelayAttack(float delay)
    {
        float originalCool = shootCoolTime;
        shootCoolTime = 999f; // �ſ� ��� �����ؼ� ��� ����
        yield return new WaitForSeconds(delay);
        shootCoolTime = originalCool;
    }

    void ResetBulletSpeed()
    {
        bulletSpeed = 10f; // �⺻������ �ǵ���
    }

    /// <summary>
    /// BattleManager���� ���� ����� ���� �� ȣ��
    /// </summary>
    public void SetTargetingSprite(bool on)
    {
        if (outlineObject != null)
            outlineObject.SetActive(on);
        if (outlineHeart != null)
            outlineHeart.SetActive(on);
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

            if(bulletPrefabName == "BARK")
            SoundManager.Instance.SFXPlay("shotgun_shot_01", 102);
            else
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
        if(bulletPrefabName == "BARK")
            return "BARK";

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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum BulletType
{
    Normal,     // �⺻ �Ѿ�
    Homing,     // ���� �Ѿ�
    Spiral,     // ȸ���� �Ѿ�
    Split,      // �п� �Ѿ�
    Directional,// ���� ���� �Ѿ�
    FixedPoint,  // Ư�� ��ġ�� �̵��ϴ� �Ѿ�
    Speed // ���� ��������
}

public class BulletController : MonoBehaviour
{
    public BulletType bulletType = BulletType.Normal; // �Ѿ��� Ÿ��
    public int damage;
    public float speed;
    public float accuracy;
    public float maxrange = 10f;
    public bool isFreind = false;

    private float gravityEffect = 0.3f;  // ������ �߷� ȿ��
    private float maxTurnAngle = 150f;  // �ִ� ȸ�� ���� ����
    private float homingDuration = 2f;  // ���� ���� �ð�

    private float maxSpeed = 16f; // �ִ� �ӵ� ����
    private float speedIncreaseRate = 4f; // �ʴ� �ӵ� ������
    private bool isAccelerating = false;
    private bool isSplitted = false; // �п� ���� Ȯ��

    private Vector2 initialPosition; // �Ѿ��� �ʱ� ��ġ
    private Vector2 targetPosition;  // Ư�� ��ġ�� �̵��� ��� ���
    private Transform target; // ���� źȯ�� Ÿ��
    private bool hasTarget = false; // ��ǥ ��ġ ����
    private Rigidbody2D rb;

    private static readonly Dictionary<BulletType, Color> bulletColors = new Dictionary<BulletType, Color>
    {
        { BulletType.Normal, Color.white },
        { BulletType.Homing, Color.red },
        { BulletType.Spiral, Color.yellow },
        { BulletType.Split, Color.green },
        { BulletType.Directional, Color.white },
        { BulletType.Speed, Color.white },
        { BulletType.FixedPoint, Color.cyan }
    };

    private void Start()
    {
        initialPosition = transform.position;

        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = bulletColors[bulletType];
        }
    }

    public void InitializeBullet(Vector2 direction, float bulletSpeed, float bulletAccuracy, int bulletDamage, float maxRange,
                                 BulletType type = default, bool accelerate = false, Transform target = null)
    {
        Vector2 adjustedDirection = ApplyAccuracy(direction);
        speed = bulletSpeed;
        damage = bulletDamage;
        accuracy = bulletAccuracy;
        maxrange = maxRange;
        bulletType = type;
        isAccelerating = accelerate;

        if (target != null)
        {
            targetPosition = target.position;
            hasTarget = true;
            StartCoroutine(MoveAndNext(type));
            //���� ��ġ�� �̵���
        }

    }

    void Update()
    {
        switch (bulletType)
        {
            case BulletType.Homing:
                StartCoroutine(HomingMove());
              
                break;
            case BulletType.Spiral:
                SpiralMove();
                break;
            case BulletType.Split:
                if (!isSplitted) StartCoroutine(SplitBullets(3));
                break;
            case BulletType.Directional:
                DirectionalMove();
                break;
            case BulletType.Normal:
                StartCoroutine(MoveTargetPlayer());
                break;
            case BulletType.Speed:
                StartCoroutine(MoveTargetPlayer());
                StartCoroutine(IncreaseSpeedOverTime());
                break;
        }

        if (Vector2.Distance(initialPosition, transform.position) >= maxrange)
        {
            DestroyBullet();
        }
    }

    // ��Ȯ���� �����Ͽ� ������ �����ϴ� �޼���
    private Vector2 ApplyAccuracy(Vector2 direction)
    {
        float randomAngle = Random.Range(-accuracy, accuracy);
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        return rotation * direction;
    }

    // Ư�� ��ġ�� �̵��ϴ� �Ѿ�
    private IEnumerator MoveAndNext(BulletType type = default)
    {
        float speed = this.speed;
        while (hasTarget && Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            yield return null;
        }
        if (type != default)
        {
            switch (bulletType)
            {
                case BulletType.Homing:
                    HomingMove();
                    break;
                case BulletType.Spiral:
                    SpiralMove();
                    break;
                case BulletType.Split:
                    if (!isSplitted) StartCoroutine(SplitBullets(3));
                    break;
                case BulletType.Directional:
                    DirectionalMove();
                    break;
                case BulletType.Normal:
                    StartCoroutine(MoveTargetPlayer());
                    break;
                case BulletType.Speed:
                    StartCoroutine(MoveTargetPlayer());
                    StartCoroutine(IncreaseSpeedOverTime());
                    break;
            }
        }
        DestroyBullet();
    }
    // ó�� �÷��̾� �������� ���� �ð� ���� �̵��� ��, �ش� ���� ����
    private IEnumerator MoveTargetPlayer()
    {
        if (target == null)
        {
            target = GameManager.Instance.GetPlayerData().player.transform; // �÷��̾ Ÿ������ ����
        }

        if (target != null)
        {
            Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized; // �÷��̾� ���� ���
            GetComponent<Rigidbody2D>().velocity = direction * speed; // ó�� �ӵ� ����

            yield return new WaitForSeconds(0.5f); // 0.5�� ���� �÷��̾� ���� ����

            // ���Ŀ��� �ش� ������ �����ϸ鼭 ����
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
    }


    // ���� �������� �Ѿ�
    private IEnumerator IncreaseSpeedOverTime()
    {
        while (speed < maxSpeed)
        {
            speed += speedIncreaseRate * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            yield return null;
        }
    }

    // �����¿� �� �밢�� ���� �̵�
    void DirectionalMove()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
    }

    // ���� źȯ ����
    private IEnumerator HomingMove()
    {
        float timer = 0f;

        while (timer < homingDuration && target != null)
        {
            if (rb != null)
            {
                Vector2 currentVelocity = rb.velocity;
                Vector2 targetDirection = ((Vector2)target.position - (Vector2)transform.position).normalized;

                // 1. ������ ȿ��: Y�� �ӵ��� �߷� ����
                Vector2 gravity = new Vector2(0, -gravityEffect * Time.deltaTime);
                currentVelocity += gravity;

                // 2. ���� ������ �θ鼭 �ε巴�� ȸ��
                float currentAngle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
                float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

                // ���� ������ ��ǥ ���� �������� �ε巴�� ȸ�� (���� ����)
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxTurnAngle * Time.deltaTime);

                // ���ο� �������� �ӵ� �缳��
                Vector2 newVelocity = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * speed;
                rb.velocity = newVelocity;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // ���� ���� �� ������ �������� ����
        if (rb != null)
        {
            rb.velocity = rb.velocity.normalized * speed;
        }
    }

    // ȸ���� ����
    void SpiralMove()
    {
        float angle = Time.time * 200f;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        GetComponent<Rigidbody2D>().velocity = direction * speed;


    }

    // ���� �Ÿ� �̵� �� �п�
    private IEnumerator SplitBullets(int splitCount)
    {
        isSplitted = true;
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < splitCount; i++)
        {
            float angle = (360f / splitCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject newBullet = Instantiate(gameObject, transform.position, Quaternion.identity);
            newBullet.GetComponent<BulletController>().InitializeBullet(direction, speed, accuracy, damage, maxrange, BulletType.Directional);
        }
        Destroy(gameObject);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}

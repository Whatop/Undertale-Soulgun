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
    public float speed = 5;
    public float accuracy;
    public float maxrange = 10f;
    public bool isFreind = false;

    private float gravityEffect = 0.3f;  // ������ �߷� ȿ��
    private float maxTurnAngle = 150f;  // �ִ� ȸ�� ���� ����
    private float homingDuration = 2f;  // ���� ���� �ð�

    private float maxSpeed = 16f; // �ִ� �ӵ� ����
    private float speedIncreaseRate = 4f; // �ʴ� �ӵ� ������
    private bool isActivated = false;
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
        rb = GetComponent<Rigidbody2D>();

        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = bulletColors[bulletType];
        }

        StartPattern(); // ó�� ������ �� ����
    }
    // ó�� �� ���� ������ ����
    private void StartPattern()
    {
        if (isActivated) return; // �� ���� ����ǵ��� ����
        isActivated = true;

        switch (bulletType)
        {
            case BulletType.Homing:
                StartCoroutine(HomingMove());
                break;
            case BulletType.Spiral:
                StartCoroutine(MoveTargetPlayer());
                break;
            case BulletType.Split:
                if (!isSplitted) StartCoroutine(SplitBullets(3));
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
    public void InitializeBullet(Vector2 direction, float bulletSpeed, float bulletAccuracy, int bulletDamage, float maxRange,
                                 float delay = 0,BulletType type = default, Transform target = null)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        accuracy = bulletAccuracy;
        maxrange = maxRange;
        bulletType = type;

        if (target != null)
        {
            targetPosition = target.position;
            hasTarget = true;
            StartCoroutine(MoveAndNext(type, delay, direction));
            //���� ��ġ�� �̵���
        }
    }

    void Update()
    {
        //if (Vector2.Distance(initialPosition, transform.position) >= maxrange)
        //{
        //    DestroyBullet();
        //}
    }
    public void ChangeBulletType(BulletType newType)
    {
        bulletType = newType;
        isActivated = false; // ���ο� Ÿ���� �����Ǿ����Ƿ� �ٽ� ���� �����ϵ��� ����

        // ���� ����
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = bulletColors[newType];
        }

        StartPattern(); // ���ο� ���� ����
    }

    // ��Ȯ���� �����Ͽ� ������ �����ϴ� �޼���
    private Vector2 ApplyAccuracy(Vector2 direction)
    {
        float randomAngle = Random.Range(-accuracy, accuracy);
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        return rotation * direction;
    }

    // Ư�� ��ġ�� �̵� �� ���� ����
    private IEnumerator MoveAndNext(BulletType type = default, float delay = 0,Vector2 dir = default)
    {
        float speed = this.speed;
        bulletType = type;
        //  ��ǥ ��ġ���� �̵�
        while (hasTarget && Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            yield return null;
        }

        //  �̵� ��, ���� ���� ���� ��� (�ʿ��� ���)
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        //  �̵� �Ϸ� ��, ���� ����
        ExecuteBulletPattern(type, dir);
    }

    // ���� ������ ���� �и��� �޼���
    private void ExecuteBulletPattern(BulletType type,Vector2 dir = default)
    {
        Debug.Log("���Ͻ���");
        switch (type)
        {
            case BulletType.Homing:
                StartCoroutine(HomingMove());
                break;
            case BulletType.Spiral:
                StartCoroutine(SpiralBullets(dir));
                break;
            case BulletType.Split:
                if (!isSplitted) StartCoroutine(SplitBullets(3));
                break;
            case BulletType.Directional:
                StartCoroutine(DirectionalMove(dir));
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

    //  �ش� ���� ����
    private IEnumerator DirectionalMove(Vector2 moveDirection = default)
    {
        if (rb == null) yield break;

        // �⺻ ������ �������� �ʾҴٸ� ���� �ӵ��� ����
        if (moveDirection == Vector2.zero)
            moveDirection = rb.velocity.normalized; // ���� �ӵ� ����

        moveDirection = moveDirection.normalized;
        rb.velocity = moveDirection * speed;

        yield return new WaitForSeconds(0.5f); // ���� �ð� ���� �̵� ���� (�ʿ� �� ���� ����)

    }


    // ���� ���� źȯ ����
    private IEnumerator HomingMove()
    {
        float timer = 0f;
        float gravityEffect = 0.3f;  // �������� ���� �߷� ȿ��
        float maxTurnAngle = 50f;     // �� �����Ӵ� �ִ� ȸ�� ���� ����
        Vector2 lastVelocity = rb.velocity;

        while (timer < homingDuration)
        {
            if (rb != null)
            {
                Vector2 targetDirection = ((Vector2)GameManager.Instance.GetPlayerData().position - (Vector2)transform.position).normalized;

                // 1. ������ ȿ�� ����
                rb.velocity += new Vector2(0, -gravityEffect * Time.deltaTime);

                // 2. ��ǥ �������� �ε巴�� ȸ��
                float currentAngle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxTurnAngle * Time.deltaTime);

                // 3. ���ο� �������� �ӵ� �缳��
                Vector2 newVelocity = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad)) * speed;
                rb.velocity = newVelocity;

                lastVelocity = newVelocity; // ������ �ӵ� ����
            }
            timer += Time.deltaTime;
            yield return null;
        }

        //  ���� ���� �� ������ ���� ����
        if (rb != null)
        {
            rb.velocity = lastVelocity;
        }
    }

    private IEnumerator SpiralBullets(Vector2 moveDirection)
    {
        float angle = 0;

        // �ʱ� �̵� ������ ����
        if (moveDirection == Vector2.zero)
            moveDirection = Vector2.right; // �⺻������ ���������� �̵�

        while (true)
        {
            angle += 300 * Time.deltaTime; //  �� ������ ȸ�� (�� ���� ����)

            // ȸ�� ���� ��� (�� �)
            Vector2 spiralDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // ���� �̵� ���⿡ ȸ�� ���͸� ���ؼ� ������ �̵�
            Vector2 finalDirection = (moveDirection + spiralDirection).normalized;

            rb.velocity = finalDirection * speed;

            yield return null;
        }
    }

    private IEnumerator SplitBullets(int splitCount)
    {
        if (isSplitted) yield break;
        isSplitted = true;
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < splitCount; i++)
        {
            float angle = (360f / splitCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(gameObject, transform.position, Quaternion.identity);
            BulletController bulletController = newBullet.GetComponent<BulletController>();

            if (bulletController != null)
            {
                // �п��� �Ѿ˵� ��� �̵��ϵ��� �ӵ� ����
                bulletController.InitializeBullet(direction, speed, accuracy, damage, maxrange, 0, bulletType);
                bulletController.rb.velocity = direction * speed; //��� �ӵ� ����
            }
        }
    }
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"�Ѿ��� {other.gameObject.name}�� �浹");
        if (other.CompareTag("Enemy") && isFreind && other.GetComponent<EnemyController>().objectState != ObjectState.Roll)
        {
            other.GetComponent<EnemyController>().TakeDamage(damage, other.transform.position);
            DestroyBullet();
        }
        else if (other.CompareTag("Soul") && !isFreind && GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().objectState != ObjectState.Roll)
        {
            GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().TakeDamage(damage, GameManager.Instance.GetPlayerData().player.transform.position);
            DestroyBullet();
        }
    }

}

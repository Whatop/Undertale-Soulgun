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
    Speed, // ���� ��������
    None
}

public class BulletController : MonoBehaviour
{
    public BulletType bulletType = BulletType.Normal; // �Ѿ��� Ÿ��
    public int damage;
    public float speed = 5;
    public float accuracy;
    public float maxrange = 10f;
    public bool isFreind = false;

    private float maxLifetime = 10f; 
    private float gravityEffect = 0.3f;  // ������ �߷� ȿ��
    private float maxTurnAngle = 150f;  // �ִ� ȸ�� ���� ����
    private float homingDuration = 5f;  // ���� ���� �ð�

    private float maxSpeed = 16f; // �ִ� �ӵ� ����
    private float speedIncreaseRate = 4f; // �ʴ� �ӵ� ������
    private bool isSplitted = false; // �п� ���� Ȯ��
    private bool isHoming = false; // �߰� ���� Ȯ��
    private bool isSpiral = false; // ȸ�� ���� Ȯ��

    private Vector2 initialPosition; // �Ѿ��� �ʱ� ��ġ
    private Vector2 targetPosition;  // Ư�� ��ġ�� �̵��� ��� ���
    private Vector2 storedFireDirection;
    private Transform target; // ���� źȯ�� Ÿ��
    private bool hasTarget = false; // ��ǥ ��ġ ����
    private Rigidbody2D rb;
    // Spiral źȯ �� ������ ������ ����
    private float spiralAngle = 0f;
    private float spiralRadius = 0.5f;
    private float bulletSize = 0;


    private static readonly Dictionary<BulletType, Color> bulletColors = new Dictionary<BulletType, Color>
    {
        { BulletType.Normal, Color.white },
        { BulletType.Homing, Color.red },
        { BulletType.Spiral, Color.yellow },
        { BulletType.Split, Color.green },
        { BulletType.Directional, Color.white },
        { BulletType.Speed, Color.white },
        { BulletType.FixedPoint, Color.cyan },
        { BulletType.None, Color.white }
    }
    ; private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        initialPosition = transform.position;

        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = bulletColors[bulletType];
        }
    }
    private void OnEnable()
    {
        isHoming = false;
        isSpiral = false;
        isSplitted = false;
        rb.velocity = Vector2.zero;
        // �ʿ��ϴٸ� scale, rotation, ���� �ʱ�ȭ � ���⼭
    }

    private void StartPattern()
    {

        if (!isFreind)//��
        {
            switch (bulletType)
            {
                case BulletType.Homing:
                    StartCoroutine(HomingMove());
                    StartCoroutine(MoveTargetPlayer());
                    break;
                case BulletType.Spiral:
                    StartCoroutine(SpiralBullets());
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
        else //�÷��̾�
        {

        }
    }
    public void InitializeBullet(Vector2 fireDirection, float bulletSpeed, float bulletAccuracy, int bulletDamage, float maxRange,
                                 float delay = 0, BulletType type = default, Transform target = null,int size = 0)
    {  // ���� �� ���� �ܿ�...
        isHoming = false;
        isSpiral = false;
        isSplitted = false;
        rb.velocity = Vector2.zero;

        // ���� �ٽ� ����
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = bulletColors[bulletType];

        speed = bulletSpeed;
        damage = bulletDamage;
        accuracy = bulletAccuracy;
        maxrange = maxRange;
        bulletType = type;
        storedFireDirection = fireDirection;
        bulletSize= size;

        if (target != null)
        {
            targetPosition = target.position;
            hasTarget = true;
            Debug.Log($"{bulletType} bullet ��� ��ġ: {targetPosition} / target �̸�: {target.name}");

            StartCoroutine(MoveAndNext(type, delay));
            //���� ��ġ�� �̵���
        }
        else
        {
            ExecuteBulletPattern(type, storedFireDirection);
        }
    }

    void Update()
    {

        switch (bulletType)
        {
            case BulletType.Homing:
                if (isHoming)
                    UpdateHoming(); // �� �� ������ ����
                break;

            case BulletType.Spiral:
                if (isSpiral)
                    UpdateSpiral(); // �� ȸ�� �ݰ� Ŀ������ ����
                break;
        }
    }
    // Homing źȯ �� ������ ����
    private void UpdateHoming()
    {
        float gravityEffect = 0.3f;  // �������� ���� �߷� ȿ��
        float maxTurnAngle = 50f;     // �� �����Ӵ� �ִ� ȸ�� ���� ���� (���� ũ�� �ް��� ȸ��)
        float timer = 0f;

        if (timer < homingDuration)
        {
            if (rb != null && GameManager.Instance.GetPlayerData().player != null)
            {
                Vector2 currentVelocity = rb.velocity;
                Vector2 targetDirection = ((Vector2)GameManager.Instance.GetPlayerData().position - (Vector2)transform.position).normalized;

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
        }
        else
        {
            rb.velocity = rb.velocity.normalized * speed; // ������ �������� ����
            isHoming = false;
        }
    }
    private void UpdateSpiral()
    {
        if (rb == null) return;

        // 1) ������ '��'���� '����'���� ��ȯ�Ͽ� ������Ű�� ����
        spiralAngle += 300f * Mathf.Deg2Rad * Time.deltaTime; // �ʴ� 300�� ȸ��
     
                                                                    // 3) �����̷� ���� ��� (�̹� �������� cos/sin ���)
        Vector2 spiral = new Vector2(
            Mathf.Cos(spiralAngle),
            Mathf.Sin(spiralAngle)
        ) * spiralRadius;

        // 4) velocity ���� (Time.deltaTime�� ���� speed�� ��)
        rb.velocity = spiral * speed;
    }

    public void ChangeBulletType(BulletType newType)
    {
        bulletType = newType;

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
    private IEnumerator MoveAndNext(BulletType type = default, float delay = 0)
    {
        float elapsed = 0f;
        float timeout = 3f; // �ִ� 3�� ���ȸ� �̵� �õ�

        while (hasTarget && Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            Vector2 newPos = rb.position + direction * speed * Time.deltaTime;
            rb.MovePosition(newPos); // �� �̰ɷ� �̵�
            elapsed += Time.deltaTime; // ������ Ÿ�̸� ���� �ڵ�
            yield return null;
        }


        // ���� ���� ��Ȳ ���� ó��
        if (elapsed >= timeout)
        {
            Debug.LogWarning("MoveAndNext Ÿ�Ӿƿ�! ��ġ�� �������� ���߾��.");
        }

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        ExecuteBulletPattern(type, storedFireDirection);
    }


    // ���� ������ ���� �и��� �޼���
    private void ExecuteBulletPattern(BulletType type, Vector2 dir = default)
    {
        switch (type)
        {
            case BulletType.Homing:
                StartCoroutine(HomingMove());
                StartCoroutine(MoveTargetPlayer());
                break;
            case BulletType.Spiral:
                StartCoroutine(SpiralBullets());
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
            case BulletType.Directional:
                StartCoroutine(DirectionalMove(dir));
                break;

            case BulletType.None:
                Debug.Log("�Ѿ˴��");
                break;
        }
    }

    // ó�� �÷��̾� �������� ���� �ð� ���� �̵��� ��, �ش� ���� ����
    private IEnumerator MoveTargetPlayer()
    {
        target = GameManager.Instance.GetPlayerData().player.transform; // �÷��̾ Ÿ������ ����

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized; // �÷��̾� ���� ���
        GetComponent<Rigidbody2D>().velocity = direction * speed; // ó�� �ӵ� ����

        yield return new WaitForSeconds(0.5f); // 0.5�� ���� �÷��̾� ���� ����

        // ���Ŀ��� �ش� ������ �����ϸ鼭 ����
        GetComponent<Rigidbody2D>().velocity = direction * speed;

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
    private IEnumerator DirectionalMove(Vector2 moveDirection)
    {
        if (rb == null) yield break;

        // ������ġ: ������ ���ٸ� ���� �ӵ� ���� or �⺻��
        if (moveDirection == Vector2.zero)
            moveDirection = rb.velocity != Vector2.zero ? rb.velocity.normalized : Vector2.right;

        rb.velocity = moveDirection.normalized * speed;

        yield return new WaitForSeconds(0.5f);
        rb.velocity = moveDirection.normalized * speed;
    }


    // ���� ���� źȯ ����
    private IEnumerator HomingMove()
    {
        isHoming = true;
        yield return null;
    }

    private IEnumerator SpiralBullets()
    {
           isSpiral = true;
        switch (bulletSize)
        {
            case 0:
                spiralRadius = 1.75f; // ������
                break;
            case 1:
                spiralRadius = 2.75f; // �߰���
                break;
            case 2:
                spiralRadius = 3.75f; // ū��
                break;
        }

        yield return null;
    }


    private IEnumerator SplitBullets(int splitCount)
    {
        if (isSplitted) yield break;
        isSplitted = true;
        StartCoroutine(MoveTargetPlayer());
        yield return new WaitForSeconds(5f);

        for (int i = 0; i < splitCount; i++)
        {
            float angle = (360f / splitCount) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(gameObject, transform.position, Quaternion.identity);
            BulletController bulletController = newBullet.GetComponent<BulletController>();
            bulletController.InitializeBullet(direction, speed, accuracy, damage, maxrange, 0, BulletType.Directional);

        }
        DestroyBullet();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"�Ѿ��� {other.gameObject.name}�� �浹");
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
    void DestroyBullet()
    {
        gameObject.SetActive(false);
    }

}

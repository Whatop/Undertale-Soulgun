using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damage;
    public float speed;
    public float accuracy;
    public float maxrange = 10f;      // �Ѿ��� �ִ� �����Ÿ�
    public bool isFreind = false;

    private Vector2 initialPosition;  // �Ѿ��� �ʱ� ��ġ
    private Vector2 targetPosition;   // �Ѿ��� ��ǥ ��ġ
    private bool hasTarget = false;   // ��ǥ ��ġ�� �����Ǿ����� ����

    // Start���� �ʱ� ��ġ ����
    private void Start()
    {
        initialPosition = transform.position;  // �Ѿ��� �ʱ� ��ġ ����
    }

    // �Ѿ� �ʱ�ȭ �޼���
    public void InitializeBullet(Vector2 direction, float bulletSpeed, float bulletAccuracy, int bulletDamage, float maxRange, Transform target = null)
    {
        // ���⿡ ��Ȯ���� �����Ͽ� ������ ���� ���
        Vector2 adjustedDirection = ApplyAccuracy(direction);

        // �Ѿ��� �Ӽ� ����
        speed = bulletSpeed;
        damage = bulletDamage;
        accuracy = bulletAccuracy;
        maxrange = maxRange;

        if (target != null)
        {
            targetPosition = target.position; // ��ǥ ��ġ ����
            hasTarget = true; // ��ǥ�� ������
        }

        // �Ѿ� �߻�
        Shoot(adjustedDirection);
    }

    // ��Ȯ���� �����Ͽ� ������ �����ϴ� �޼���
    private Vector2 ApplyAccuracy(Vector2 direction)
    {
        float randomAngle = Random.Range(-accuracy, accuracy);
        Quaternion rotation = Quaternion.AngleAxis(randomAngle, Vector3.forward);
        return rotation * direction;
    }

    // �Ѿ��� �߻��ϴ� �޼���
    private void Shoot(Vector2 direction)
    {
        if (!hasTarget)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed;
        }
    }

    void Update()
    {
        // ��ǥ ��ġ�� ������ ��� �ش� ��ġ�� �̵�
        if (hasTarget)
        {
            Vector2 currentPosition = transform.position;
            Vector2 direction = (targetPosition - currentPosition).normalized;
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            // ��ǥ ��ġ�� �����ϸ� �Ҹ�
            if (Vector2.Distance(currentPosition, targetPosition) <= 0.1f)
            {
                DestroyBullet();
            }
        }
        else
        {
            // �Ѿ��� �ִ� �����Ÿ��� �ʰ��ϸ� �Ҹ�
            if (Vector2.Distance(initialPosition, transform.position) >= maxrange)
            {
                DestroyBullet();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ���� �浹 �� ó��
        if (other.CompareTag("Enemy") && isFreind && other.GetComponent<EnemyController>().objectState != ObjectState.Roll)
        {
            other.GetComponent<EnemyController>().TakeDamage(damage, other.transform.position);
            DestroyBullet();
        }
        // �÷��̾�� �浹 �� ó��
       // else if (other.CompareTag("Player") && !isFreind && other.GetComponent<PlayerMovement>().objectState != ObjectState.Roll)
       // {
       //     other.GetComponent<PlayerMovement>().TakeDamage(damage, other.transform.position);
       //     DestroyBullet();
       // }
        else if (other.CompareTag("Soul") && !isFreind && GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().objectState != ObjectState.Roll)
        {
            GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().TakeDamage(damage, GameManager.Instance.GetPlayerData().player.transform.position);
            DestroyBullet();
        }
       //else if (other.CompareTag("Wall"))
       //{
       //    DestroyBullet();
       //}
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}

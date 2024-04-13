using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damage;
    public float speed;
    public float accuracy;
    public float maxrange = 10f;      // �Ѿ��� �ִ� �����Ÿ�

    private Vector2 initialPosition;   // �Ѿ��� �ʱ� ��ġ
    // �Ѿ� �ʱ�ȭ �޼���
    public void InitializeBullet(Vector2 direction, float bulletSpeed, float bulletAccuracy, int bulletDamage, float maxRange)
    {
        // ���⿡ ��Ȯ���� �����Ͽ� ������ ���� ���
        Vector2 adjustedDirection = ApplyAccuracy(direction);

        // �Ѿ��� �Ӽ� ����
        speed = bulletSpeed;
        damage = bulletDamage;
        accuracy = bulletAccuracy;
        maxrange = maxRange;
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
        // �Ѿ��� ����� �ӵ��� �°� �߻��ϴ� ������ ����
        // �� �κп��� Rigidbody ���� Ȱ���Ͽ� �Ѿ��� �̵���ų �� ����
        // ���� ���, GetComponent<Rigidbody2D>().velocity = direction.normalized * speed;
        GetComponent<Rigidbody2D>().velocity = direction * speed;
    }

        void Update()
        {
            // �ִ� �����Ÿ��� �ʰ��ϸ� �Ѿ��� �Ҹ��Ŵ
            if (Vector2.Distance(initialPosition, transform.position) >= maxrange)
            {
                DestroyBullet();
            }
        }

    void OnTriggerEnter2D(Collider2D other)
    {
        // �ٸ� ������Ʈ�� �浹 �� ó���� ������ �߰��մϴ�.
        if (other.CompareTag("Enemy"))
        {
            // ��: ������ �������� �����ϴ�.
            other.GetComponent<EnemyController>().TakeDamage(damage);
            Debug.Log("������ : " + damage);
           // �Ѿ� �Ҹ� �Ǵ� ȿ�� �߰� ���� �����մϴ�.
            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        // �Ѿ� �Ҹ� �� ó���� ������ ���⿡ �߰��մϴ�.
        Destroy(gameObject);
    }
}

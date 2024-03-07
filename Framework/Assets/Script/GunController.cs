using UnityEngine;

// �� ���� Ư���� ��Ÿ���� Ŭ����
[System.Serializable]
public class Gun
{
    public int id;             // ���� ������ ID
    public string gunName;     // ���� �̸�
    public int damage;         // ���� ���ݷ�
    public int currentAmmo;    // ���� ź������ �����ִ� �Ѿ� ��
    public int maxAmmo;        // ź������ �ִ� �Ѿ� ��
    public int maxRange;
    public float bulletSpeed;  // �Ѿ� �ӵ�
    public float accuracy;     // ���� ��Ȯ��
    public Transform firePoint; // �Ѿ��� �߻�� ��ġ
}

public class GunController : MonoBehaviour
{
    public Transform gunTransform;  // �� ���� Transform
    public GameObject bulletPrefab; // �Ѿ� ������
    public Gun currentGun;          // ���� ��� ���� ���� ����

    public float rotationSpeed = 10f; // �÷��̾� ȸ�� �ӵ�

    void Update()
    {
        // ���콺 ���� ���
        Vector3 direction = GetDirectionToMouse();

        // �÷��̾� ȸ��
        RotatePlayer(direction);

        // ���콺 ���� ��ư Ŭ�� �� �Ѿ� �߻�
        if (Input.GetMouseButtonDown(0))
        {
            ShootBullet(direction);
        }
    }

    // ���콺 ��ġ�κ��� ���� ���� ���
    Vector3 GetDirectionToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return (targetPosition - transform.position).normalized;
    }

    // �÷��̾� ȸ��
    void RotatePlayer(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        gunTransform.rotation = Quaternion.Slerp(gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // �Ѿ� �߻�
    void ShootBullet(Vector3 direction)
    {
        // �Ѿ��� ���� ���� ��� �߻�
        if (currentGun.currentAmmo > 0)
        {
            // �Ѿ� ���� �� �ʱ�ȭ
            GameObject bullet = Instantiate(bulletPrefab, currentGun.firePoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.InitializeBullet(direction, currentGun.bulletSpeed, currentGun.accuracy, currentGun.damage, currentGun.maxRange);

            // ���� ź�������� �Ѿ� ����
            currentGun.currentAmmo--;

            // �α� ���
            Debug.Log($"Shot {currentGun.gunName}! Ammo: {currentGun.currentAmmo}/{currentGun.maxAmmo}");
        }
        else
        {
            // �Ѿ��� ������ ��� �α� ���
            Debug.Log("Out of ammo!");
        }
    }
}

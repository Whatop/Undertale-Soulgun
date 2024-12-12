using UnityEngine;

// �� ���� Ư���� ��Ÿ���� Ŭ����
[System.Serializable]
public class Weapon
{
    public int id;             // ���� ������ ID
    public string WeaponName;     // ���� �̸�
    public int damage;         // ���� ���ݷ�
    public int current_Ammo;    // ���� ź������ �����ִ� �Ѿ� ��
    public int magazine;       //źâ �ִ� �Ѿ˼�  
    public int current_magazine;    // ���� �����ִ� �Ѿ� ��
    public int maxAmmo;        // �ִ� �Ѿ� ��
    public int maxRange;        //��Ÿ�
    public float bulletSpeed;  // �Ѿ� �ӵ�
    public float accuracy;     // ���� ��Ȯ��
    public Transform firePoint; // �Ѿ��� �߻�� ��ġ
    public Weapon()
    {
        // �ʱ�ȭ ���� �߰� (��: �⺻�� ����)
        id = 0;
        WeaponName = "None";
        damage = 1;
        maxAmmo = -1;
        current_Ammo = maxAmmo;
        magazine = 6;
        current_magazine = magazine;
        bulletSpeed = 1;
        accuracy = 1;
        // �߰� ������ �ʱ�ȭ
    }
    // ���� �Ѿ� ���� Ȯ�� �޼ҵ�
    public bool IsInfiniteAmmo()
    {
        return current_Ammo == -1;  // Ammo�� -1�̸� �������� ����
    }
}

public class WeaponController : MonoBehaviour
{
    public Transform WeaponTransform;  // �� ���� Transform
    public GameObject bulletPrefab; // �Ѿ� ������
    public Weapon currentWeapon;          // ���� ��� ���� ���� ����

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
        WeaponTransform.rotation = Quaternion.Slerp(WeaponTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // �Ѿ� �߻�
    void ShootBullet(Vector3 direction)
    {
        // �Ѿ��� ���� ���� ��� �߻�
        if (currentWeapon.current_magazine > 0)
        {
            // �Ѿ� ���� �� �ʱ�ȭ
            GameObject bullet = Instantiate(bulletPrefab, currentWeapon.firePoint.position, Quaternion.identity);
            BulletController bulletController = bullet.GetComponent<BulletController>();
            bulletController.InitializeBullet(direction, currentWeapon.bulletSpeed, currentWeapon.accuracy, currentWeapon.damage, currentWeapon.maxRange);

            // ���� �Ѿ� ���°� �ƴϸ� ź�� ����
            if (!currentWeapon.IsInfiniteAmmo())
            {
                currentWeapon.current_magazine--;
            }

            // �α� ���
            Debug.Log($"Shot {currentWeapon.WeaponName}! Ammo: {currentWeapon.current_magazine}/{currentWeapon.maxAmmo}");
        }
        else
        {
            // �Ѿ��� ������ ��� �α� ���
            Debug.Log("Out of ammo!");
        }
    }
}

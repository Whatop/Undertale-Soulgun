using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform WeaponTransform; // ���� Transform ������Ʈ
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�

    void Update()
    {
        // ���콺 ��ġ�� �������� ���� ������ �����մϴ�.


        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - WeaponTransform.position).normalized;
        WeaponTransform.up = direction;

        // ���콺 ���� ��ư�� Ŭ���ϸ� �Ѿ��� �߻��մϴ�.
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // �Ѿ��� �����ϰ� �ʱ� ��ġ�� ���� ��ġ�� �����մϴ�.
        GameObject bullet = Instantiate(bulletPrefab, WeaponTransform.position, WeaponTransform.rotation);

        // �Ѿ˿� �ӵ��� �����Ͽ� �߻��մϴ�.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = WeaponTransform.up * bulletSpeed;
    }
    void Reload()
    {
    }
}


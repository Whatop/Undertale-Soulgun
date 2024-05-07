using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform WeaponTransform; // ���� Transform ������Ʈ
    public Transform shotpoint; // ���� Transform ������Ʈ
    public GameObject bulletPrefab; // �Ѿ� ������
    public float bulletSpeed = 10f; // �Ѿ� �߻� �ӵ�
    Weapon weaponData;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        weaponData = new Weapon();
    }

    void Update()
    {
        // ���콺 ��ġ�� �������� ���� ������ �����մϴ�.

        weaponData = gameManager.GetWeaponData();
        int current_magazine= weaponData.current_magazine;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - WeaponTransform.position).normalized;
        WeaponTransform.up = direction;

        // ���콺 ���� ��ư�� Ŭ���ϸ� �Ѿ��� �߻��մϴ�.
        if (Input.GetMouseButtonDown(0) && current_magazine > 0  && weaponData.current_Ammo > 0)
        {
            Shoot();
            weaponData.current_magazine -= 1;
            weaponData.current_Ammo -= 1;
            gameManager.SaveWeaponData(weaponData);
        }

        if(weaponData.current_Ammo < weaponData.maxAmmo &&
            Input.GetKeyDown(KeyCode.R) &&
            weaponData.current_magazine < weaponData.magazine)
        {
            weaponData.current_magazine = weaponData.magazine;
            gameManager.SaveWeaponData(weaponData);
        }
    }

    void Shoot()
    {
        // �Ѿ��� �����ϰ� �ʱ� ��ġ�� ���� ��ġ�� �����մϴ�.
        GameObject bullet = Instantiate(bulletPrefab, shotpoint.position, WeaponTransform.rotation);
        
        
        // �Ѿ˿� �ӵ��� �����Ͽ� �߻��մϴ�.
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = WeaponTransform.up * bulletSpeed;
    }
    void Reload()
    {
    }
}


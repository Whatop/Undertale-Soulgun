using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Transform WeaponTransform; // ���� Transform ������Ʈ
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

        Weapon weapon = gameManager.GetWeaponData();
        int current_magazine= weapon.current_magazine;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - WeaponTransform.position).normalized;
        WeaponTransform.up = direction;

        // ���콺 ���� ��ư�� Ŭ���ϸ� �Ѿ��� �߻��մϴ�.
        if (Input.GetMouseButtonDown(0) && current_magazine > 0)
        {
            Shoot();
            weapon.current_magazine -= 1;
            weapon.current_Ammo -= 1;
            gameManager.SaveWeaponData(weapon);
        }

        if(weapon.current_Ammo < weapon.maxAmmo &&Input.GetKeyDown(KeyCode.R) && weapon.current_magazine < weapon.magazine)
        {

            weapon.current_magazine = weapon.magazine;
            gameManager.SaveWeaponData(weapon);
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


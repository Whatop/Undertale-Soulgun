using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingObject : MonoBehaviour
{
    protected int health;
    protected float speed;
    protected bool isnpc; // ������!
    public virtual void Die()
    {
        Debug.Log("Object died!");
        // ���⿡ ���� ���� ������ �����մϴ�.
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Move(Vector3 direction)
    {
        if (!isnpc)
        {
            //������� ������ AI �ۼ�
        }
        

    }

    public virtual void Init(int startingHealth, float startingSpeed)
    {
        health = startingHealth;
        speed = startingSpeed;
    }
}

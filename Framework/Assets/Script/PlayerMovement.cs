using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Walk,
    Idle,
    Roll,
    None
}
public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed;
    Animator animator;
    Rigidbody2D rigid;
    public float h;
    public float v;
    Vector3 dirVec;

    GameObject scanObject;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z; // ���콺�� z ��ǥ�� ����
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = targetPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Vector3 currentScale = transform.localScale;
        //currentScale.x = Mathf.Abs(currentScale.x) * -1;
        //transform.localScale = currentScale;

        bool isUp = false;
        bool isSide = false;
        bool isDown = false;
        bool isAngle = false;

        if (angle > -35f && angle <= 35f)
        {
            // ������ ����
            isSide = true;
        }
        else if (angle > 35f && angle <= 125f)
        {
            // ���� ����
            isUp = true;
        }
        else if (angle > 125f || angle <= -215f)
        {
            // ���ʰ� ������ ������ Side�� ����
            isSide = true;
        }
        else if (angle > -215f && angle <= -35f)
        {
            // �Ʒ��� ����
            isDown = true;
        }
        else
        {
            // ���� ����
            isAngle = true;
        }

        // �ִϸ����Ϳ� bool ���� ����
        animator.SetBool("IsUp", isUp);
        animator.SetBool("IsSide", isSide);
        animator.SetBool("IsDown", isDown);
        animator.SetBool("IsAngle", isAngle);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = 6f;
        }
        else
        {
            MoveSpeed = 4f;
        }
        if (GameManager.Instance.GetPlayerData().currentState == GameState.Event)
        {
            animator.SetInteger("v", 0);
            animator.SetInteger("h", 0);
            MoveSpeed = 0f;
        }


    }
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (h != animator.GetInteger("h"))
        {
            animator.SetInteger("h", (int)h);

            animator.SetBool("isMove", true);
        }
        else if (v != animator.GetInteger("v"))
        {
            animator.SetInteger("v", (int)v);

            animator.SetBool("isMove", true);
        }

        if (v == 0 && h == 0)
        {
            animator.SetBool("isMove", false);
        }

        rigid.velocity = new Vector2(h, v) * MoveSpeed;
    }
    void UpdateAnimation(Vector2 moveInput)
    {
        // moveInput�� ���� �ִϸ��̼��� �����մϴ�.
        if (moveInput.magnitude > 0)
        {
            animator.SetFloat("Horizontal", moveInput.x);
            animator.SetFloat("Vertical", moveInput.y);
        }
        else
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
        }
    }
}

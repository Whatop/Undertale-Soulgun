using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Down,
    Up,
    Side,
    Angle,
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

    bool isMove = false;

    public float cooldownTime = 0.5f;
    private bool isCooldown = false;

    GameObject scanObject;
    public PlayerState playerState;
    GameManager gameManager;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }

    
    void Update()
    {
        float angle = CalculateMouseAngle();


        Debug.Log("���� ����: " + angle);
        if (Input.GetMouseButtonDown(1) && !isCooldown && isMove)
        {
            // ��Ŭ�� �Է��� �����Ǹ� ó���ϰ� ��Ÿ�� ����
            playerState = PlayerState.Roll;
            StartRoll();
            StartCooldown();
        }

        // ��Ÿ�� ����
        if (isCooldown)
        {
            cooldownTime -= Time.deltaTime;

            // ��Ÿ���� ������ ���� �ʱ�ȭ
            if (cooldownTime <= 0)
            {
                isCooldown = false;
                cooldownTime = 0.5f; // ���ϴ� ��Ÿ�� ������ ����
                playerState = PlayerState.None;
            }
        }

        if (playerState != PlayerState.Roll)//������� ����ó���� �����ؼ�
        {
            animator.SetBool("IsUp", false);
            animator.SetBool("IsSide", false);
            animator.SetBool("IsDown", false);
            animator.SetBool("IsAngle", false);

            if (angle > -45f && angle <= 15f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * 1;
                transform.localScale = currentScale;

                SetPlayerState(PlayerState.Side);
            }
            else if (angle > 45f && angle <= 135f)
            {
                SetPlayerState(PlayerState.Up);
            }
            else if ((angle > 165f && angle <= 180f) || (angle >= -180f && angle < -135f))
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * -1;
                transform.localScale = currentScale;
                SetPlayerState(PlayerState.Side);
            }
            else if (angle >= -135f && angle < -45f)
            {
                SetPlayerState(PlayerState.Down);
            }
            else if (angle < 45f && angle >= 15f)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * 1;
                transform.localScale = currentScale;

                SetPlayerState(PlayerState.Angle);
            }
            else
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * -1;
                transform.localScale = currentScale;

                SetPlayerState(PlayerState.Angle);
            }
        }
    }

    private void StartRoll() //Roll�� ����ó�� ���� �� �̵������� �Ǻ��ؼ� 
    {
        if (v != 0 && h == 0) // ���� �Ǵ� ���������� �̵��� ��
        {
            Vector3 currentScale = transform.localScale;
            if (h < 0) // ���� �̵�
            {
                currentScale.x = Mathf.Abs(currentScale.x) * -1;// ������ �ݴ�� ����
            }
            else // ������ �̵�
            {
                currentScale.x = Mathf.Abs(currentScale.x) * 1;//  ������ �״�� ����
            }
            transform.localScale = currentScale;
            animator.SetBool("IsSide", true);
        }
        else if (v == 0 && h != 0) // �� �Ǵ� �Ʒ��� �̵��� ��
        {
            animator.SetBool("IsUp", v > 0);
            animator.SetBool("IsDown", v < 0);
        }
        else // �밢�� �̵��� ��
        {
            Vector3 currentScale = transform.localScale;
            if (h < 0 && v > 0) // ���� �� �밢��
            {
                currentScale.x = Mathf.Abs(currentScale.x) * -1;// ������ �ݴ�� ����
                animator.SetBool("IsAngle", true);
            }
            else if (h > 0 && v > 0) // ������ �� �밢��
            {
                currentScale.x = Mathf.Abs(currentScale.x) * 1;//  ������ �״�� ����
                animator.SetBool("IsAngle", true);
            }
            else if (h < 0 && v < 0) // ���� �Ʒ� �밢��
            {
                animator.SetBool("IsSide", true);
                currentScale.x = Mathf.Abs(currentScale.x) * -1;// ������ �ݴ�� ����
            }
            else if (h > 0 && v < 0) // ������ �Ʒ� �밢��
            {
                animator.SetBool("IsSide", true);
                currentScale.x = Mathf.Abs(currentScale.x) * 1;//  ������ �״�� ����
            }
            transform.localScale = currentScale;

            //���� �밢�� �Ʒ��� �����Ϲݴ��, ������ �Ʒ� �밢���̸� �״�� 
        }
            animator.SetTrigger("IsRoll");
    }

    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if (h != animator.GetInteger("h"))
        {
            isMove = true;
            animator.SetInteger("h", (int)h);

        }
        else if (v != animator.GetInteger("v"))
        {
            isMove = true;
            animator.SetInteger("v", (int)v);
        }

        if (v == 0 && h == 0)
        {
            isMove = false;
        }
        animator.SetBool("isMove", isMove);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            MoveSpeed = 6f;
        }
        else
        {
            MoveSpeed = 4f;
        }
        if (gameManager.GetPlayerData().currentState == GameState.Event)
        {
            animator.SetInteger("v", 0);
            animator.SetInteger("h", 0);
            MoveSpeed = 0f;
        }
        rigid.velocity = new Vector2(h, v) * MoveSpeed;
    }
    void StartCooldown()
    {
        // ��Ÿ�� ����
        isCooldown = true;
    }
    void SetPlayerState(PlayerState newState)
    {

        // �� ���¿� ���� �ʱ�ȭ ���� ������ �߰��� �� �ֽ��ϴ�.
        switch (newState)
        {
            case PlayerState.Angle:
                // Walk ���¿� ���� �ʱ�ȭ ����
                animator.SetBool("IsAngle", true);
                break;
            case PlayerState.Down:
                // Idle ���¿� ���� �ʱ�ȭ ����
                animator.SetBool("IsDown", true);
                break;
            case PlayerState.Side:
                // Roll ���¿� ���� �ʱ�ȭ ����
                animator.SetBool("IsSide", true);
                break;
            case PlayerState.Up:
                // None ���¿� ���� �ʱ�ȭ ����
                animator.SetBool("IsUp", true);
                break;
            default:
                break;
        }
    }
    float CalculateMouseAngle()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = targetPosition - transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
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

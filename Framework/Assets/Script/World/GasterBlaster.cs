using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    private Animator animator;
    public GameObject laserPrefab;               // ������ ������
    public Transform laserSpawnPoint;            // ������ ���� ��ġ
    private bool laserFired = false;
    public Vector2 targetDirection = Vector2.down; // BattleManager���� ����
    public bool trackPlayer = true; // true�� �÷��̾� ����, false�� targetDirection ���

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void OnEnable()
    {
        laserFired = false;
        animator.Play("Idle");
    }
    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Shot()
    {
        animator.SetTrigger("OpenMouth");
        Vector2 targetPos = GameManager.Instance.GetPlayerData().player.transform.position;
        Vector2 myPos = transform.position;

        Vector2 dir = targetPos - myPos; // �� �� �÷��̾� ����
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle+90); // Sprite�� ������ forward�� -90 ����

        SoundManager.Instance.SFXPlay("gasterblaster", 225); // ����/�߻� ����
    }

    // �ִϸ��̼ǿ��� ȣ��
    public void FireLaser()
    {
        if (laserFired) return;
        laserFired = true;

        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        laser.transform.SetParent(transform); // �θ�� �ٿ� ����
        CameraController.Instance.ShakeCamera();
        EndAttack();
    }
    public void EndAttack()
    {
        StartCoroutine(MoveBackAndDisable());
    }

    IEnumerator MoveBackAndDisable()
    {
        Vector3 backDir = transform.up;
        Vector3 start = transform.position;
        Vector3 end = start + backDir * 40f; // �Ÿ� 2��� ����

        float duration = 5f; // ���� 2.5f�� 2��� ����
        float t = 0f;

        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false); // �ʿ��ϴٸ� ���� ó��
    }


    void DeactiveDelay()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Soul"))
        {
            GameObject player = other.gameObject;

            GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().TakeDamage(1, GameManager.Instance.GetPlayerData().player.transform.position);


        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soul"))
        {
            GameObject player = other.gameObject;

            GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().TakeDamage(1, GameManager.Instance.GetPlayerData().player.transform.position);


        }
    }
}

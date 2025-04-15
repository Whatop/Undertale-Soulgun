using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasterBlaster : MonoBehaviour
{
    private Animator animator;
    public GameObject laserPrefab;               // ������ ������
    public Transform laserSpawnPoint;            // ������ ���� ��ġ
    private bool laserFired = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        laserFired = false;
        animator.Play("Idle");       // �ʱ� ����
        Invoke(nameof(Shot), 0.3f);  // ���� �� �ణ�� ���� �ΰ� ���� ����
        Invoke(nameof(DeactiveDelay), 10f); // Ȥ�� �� ����� ��� ���
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    void Shot()
    {
        animator.SetTrigger("OpenMouth");
        SoundManager.Instance.SFXPlay("gasterblaster", 225); // ����/�߻� ����
        CameraController.Instance.ShakeCamera();
    }

    // �ִϸ��̼ǿ��� ȣ��
    public void FireLaser()
    {
        if (laserFired) return;
        laserFired = true;

        GameObject laser = Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        laser.transform.SetParent(transform); // �θ�� �ٿ� ����
    }

    // �ִϸ��̼ǿ��� ȣ��
    public void EndAttack()
    {
        StartCoroutine(MoveBackAndDisable());
    }

    IEnumerator MoveBackAndDisable()
    {
        Vector3 start = transform.position;
        Vector3 end = transform.position + Vector3.down * 3f;
        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            transform.position = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
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

            player.GetComponent<EnemyController>().TakeDamage(1);
          
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Soul"))
        {
            GameObject player = other.gameObject;

            player.GetComponent<EnemyController>().TakeDamage(1);
             
        }
    }
}

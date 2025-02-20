using UnityEngine;
using System.Collections;

public class ShellBehavior : MonoBehaviour
{
    // ź�� �̵� ����
    public Vector2 velocity;       // �ʱ� �ӵ� (Eject �� ����)
    public float drag = 3f;       // ����(����) ���
    public float rotationSpeed = 360f; // ź�� ȸ�� �ӵ� (��/��)

    // ���� ����
    public float minVelocity = 0.1f;  // �� ���� �ӵ��� ������ ����
    public float maxLifetime = 2f;    // �ִ� �̵� �ð�
    private float elapsedTime = 0f;
    private bool hasLanded = false;   // ���� ����

    // ���� �� ���̵� �ƿ�
    public float fadeDuration = 1f;
    private bool isFading = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    // ����
    public string landSoundName = "shell_land_01";
    public int landSoundIndex = 300;
    public float landVolume = 0.7f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (!hasLanded)
        {
            // 1) ź�� �̵� (�ܼ� x,y ��ǥ �̵�)
            transform.Translate(velocity * Time.deltaTime, Space.World);

            // 2) ź�� ȸ�� (���Ѵٸ�)
            float rotateAngle = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotateAngle);

            // 3) ������ �ӵ� ������ ����
            //    �ӵ� ũ�Ⱑ drag*dt ��ŭ �پ�� (�ܼ� �������� ����)
            float speed = velocity.magnitude;
            speed -= drag * Time.deltaTime;
            if (speed < 0f) speed = 0f;
            velocity = velocity.normalized * speed;

            // 4) ���� ���� : ���� �ð� ��� or �ӵ� �ʹ� �۾���
            if (elapsedTime >= maxLifetime || velocity.magnitude < minVelocity)
            {
                Land();
            }
        }
        else
        {
            // �̹� ����������, ���̵� ������ Ȯ�θ�
            // ������ �ڿ��� ��ġ �̵� �� �� ��
            rb.gravityScale = 0;
        }
    }

    void Land()
    {
        hasLanded = true;
        // ������ ����
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;  // ȸ���� ����
        SoundManager.Instance.SFXPlay("shotgun_reload_01", 224); // ������ ����

        // ���� �� ���̵� �ƿ� ����
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        if (isFading) yield break;
        isFading = true;

        Color originColor = sr.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            sr.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}

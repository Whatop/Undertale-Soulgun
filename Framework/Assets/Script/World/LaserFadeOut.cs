using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserFadeOut : MonoBehaviour
{
    [Header("������ �⺻ ����")]
    public Transform laserOrigin;       // ������ �߻� ���� ���� (��: �ҿ��� Transform)
    public float maxDistance = 30f;     // �������� ���� �� �ִ� �ִ� �Ÿ�
    public LayerMask obstacleMask;      // Raycast�� ����� ���̾� ����ũ (Wall, Barrier, Soul �� ��� ����)

    [Header("���� & ���̵� ����")]
    public float growSpeed = 10f;       // �������� �ʴ� ��ŭ ������� (����/��)
    public float fadeDuration = 0.5f;   // Barrier�� ���� �� ���İ� 1��0���� �پ��� �ð�(��)

    [Header("���־� �β� ����")]
    public float thickness = 0.8f;      // ������ ���� �β� (LineRenderer.widthMultiplier ��� ������ ���)

    [Header("DoT ����")]
    public float dotInterval = 0.2f;    // Soul(Player) ���� ������ �ֱ� (��)
    public float dotDPS = 50f;          // �ʴ� ������ (Damage Per Second)
    private Dictionary<GameObject, float> hitTimer = new Dictionary<GameObject, float>();

    // ��Barrier�����ǡ� ��ǥ �Ÿ� (�� ������ ����)
    private float targetDistance = 0f;
    // ���� �������� �ڶ� ���� (World ���� ����)
    private float currentLength = 0f;
    // ���̵�ƿ� ���� ���� ��� �ð�
    private float fadeTimer = 0f;

    // ���� �� ���� �׶��̼�(���� ����)�� ����
    private Gradient originalGradient;
    private bool hasHitBarrier = false;  // Barrier �浹 ���� �߰�

    // LineRenderer ������Ʈ
    private LineRenderer lineRenderer;
    [Header("�ڵ� ���̵� ����")]
    public bool autoFade = false;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // �ݵ�� �� ������ �׸����� ����
        lineRenderer.positionCount = 2;

        // LineRenderer�� ������ ��ǥ���� �׸��� ����
        lineRenderer.useWorldSpace = true;

        // ������ �β� ����
        lineRenderer.widthMultiplier = thickness;

        // ���� Inspector�� ������ Color Gradient�� ������ ��
        originalGradient = lineRenderer.colorGradient;
    }
    private void Start()
    {
        currentLength = 0f;
        fadeTimer = 0f;
        hitTimer.Clear();

        // Start/End Color ���ĸ� 1�� �ʱ�ȭ
        Color sc = lineRenderer.startColor;
        Color ec = lineRenderer.endColor;
        sc.a = 1f;
        ec.a = 1f;
        lineRenderer.startColor = sc;
        lineRenderer.endColor = ec;
    }

    private void Update()
    {
        Vector2 originPos = laserOrigin.position;
        Vector2 direction = laserOrigin.up.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(originPos, direction, maxDistance, obstacleMask);
        float nearestBarrierDist = maxDistance;
        bool barrierFound = false;
        List<RaycastHit2D> soulHits = new List<RaycastHit2D>();
        foreach (var h in hits)
        {
            if (h.collider == null) continue;
            if (h.distance < 0.1f)
                continue;

            if (h.collider.CompareTag("Barrier") && h.distance < nearestBarrierDist)
            {
                nearestBarrierDist = h.distance;
               barrierFound = true;
            }
            else if (h.collider.CompareTag("Soul"))
            {
                soulHits.Add(h);
            }
        }
        // 7) ������ DoT(���� ������) ó��
        if (barrierFound)
        {
            // Barrier �տ� �ִ� Enemy�鸸 ó��
            foreach (var eh in soulHits)
            {
                if (eh.distance < nearestBarrierDist)
                {
                    ApplyDotToEnemy(eh.collider.gameObject);
                }
            }
        }
        else
        {
            // Barrier ������ ���� �� ��� Enemy���� ó��
            foreach (var eh in soulHits)
            {
                ApplyDotToEnemy(eh.collider.gameObject);
            }
        }

        // 1. targetDistance ����
        targetDistance = barrierFound ? nearestBarrierDist : maxDistance;

        // 2. ���� vs ���̵�
        if (!hasHitBarrier && currentLength < targetDistance)
        {
            currentLength += growSpeed * Time.deltaTime;
            currentLength = Mathf.Min(currentLength, targetDistance);
        }

        // 3. Barrier ���� ����
        if (barrierFound && !hasHitBarrier && currentLength >= nearestBarrierDist - (growSpeed * Time.deltaTime))
        {
            hasHitBarrier = true;
            EffectManager.Instance.SpawnEffect("barrier_flash", originPos + direction * nearestBarrierDist, Quaternion.identity);
        }

        // 4. LineRenderer �׸���
        float actualDist = barrierFound ? nearestBarrierDist : currentLength;
        lineRenderer.SetPosition(0, originPos);
        lineRenderer.SetPosition(1, originPos + direction * actualDist);

        // 5. ���̵� Ʈ����: Barrier�� ��Ұų�, maxDistance�� �������� ��
        if (!autoFade && (hasHitBarrier || currentLength >= maxDistance))
        {
            fadeTimer += Time.deltaTime;
            float fadePercent = Mathf.Clamp01(fadeTimer / fadeDuration);
            float currentAlpha = Mathf.Lerp(1f, 0f, fadePercent);

            SetLineRendererAlpha(currentAlpha);

            if (Mathf.Approximately(currentAlpha, 0f))
            {
                Destroy(gameObject);
            }
        }
    }

    // Material ���� ���� �Լ�
    private void SetLineRendererAlpha(float alpha)
    {
        if (lineRenderer.material.HasProperty("_BaseColor"))
        {
            Color currentColor = lineRenderer.material.GetColor("_BaseColor");
            currentColor.a = alpha;
            lineRenderer.material.SetColor("_BaseColor", currentColor);
        }
        else if (lineRenderer.material.HasProperty("_Color"))
        {
            Color currentColor = lineRenderer.material.color;
            currentColor.a = alpha;
            lineRenderer.material.color = currentColor;
        }
    }


    /// <summary>
    /// ���� ����(dotInterval)���� Soul(GameObject)�� DoT(���� ������) ����
    /// </summary>
    private void ApplyDotToEnemy(GameObject soulObj)
    {
        if (!hitTimer.ContainsKey(soulObj) || Time.time - hitTimer[soulObj] >= dotInterval)
        {
            var playerMove = GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>();
            if (playerMove == null)
                return;

            // ������ ���̸� ������ �� ��
            if (playerMove.objectState == ObjectState.Roll)
                return;

            hitTimer[soulObj] = Time.time;
            if (soulObj.CompareTag("Soul"))
            {
                LivingObject player = GameManager.Instance.GetPlayerData().player.GetComponent<LivingObject>();
                ObjectState state = GameManager.Instance.GetPlayerData().player.GetComponent<PlayerMovement>().objectState;
                if (player != null )
                {
                    DamageInfo info = new DamageInfo(1, this.gameObject, DamageType.Normal, false, false);
                    player.TakeDamage(info);
                }
            }
        }
    }


}

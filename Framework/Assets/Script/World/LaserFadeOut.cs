using System.Collections;
using UnityEngine;

public class LaserFadeOut : MonoBehaviour
{
    private SpriteRenderer sr;
    private Vector3 originalScale;
    public float shrinkStopAt = 0.5f;
    public float duration = 1.5f;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
        StartCoroutine(FadeAndShrink());
    }

    IEnumerator FadeAndShrink()
    {
        float t = 0f;

        while (t < duration)
        {
            float percent = t / duration;

            // ���̱�
            float scale = Mathf.Lerp(1f, shrinkStopAt, percent);
            transform.localScale = new Vector3(originalScale.x * scale, originalScale.y, originalScale.z);

            // ���� ���� (�Ĺݺ�)
            if (percent > 0.7f)
            {
                float fadeT = (percent - 0.7f) / 0.3f;
                Color color = sr.color;
                color.a = Mathf.Lerp(1f, 0f, fadeT);
                sr.color = color;
            }

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}

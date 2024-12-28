using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    private Animator animator;
    private float animationLength;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            // ù ��° �ִϸ��̼� Ŭ���� ���̸� ������
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                animationLength = clips[0].length; // �ʿ��� �ִϸ��̼� ���̸� ����
            }
        }
    }

    private void OnEnable()
    {
        // �ִϸ��̼��� ������ �ڵ����� ��Ȱ��ȭ
        Invoke(nameof(DisableEffect), animationLength);
    }

    private void DisableEffect()
    {
        gameObject.SetActive(false); // ����Ʈ�� ��Ȱ��ȭ
    }
}

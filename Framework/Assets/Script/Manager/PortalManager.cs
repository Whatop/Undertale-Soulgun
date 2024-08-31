using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Cinemachine ���ӽ����̽� �߰�
using UnityEngine.UI; // Cinemachine ���ӽ����̽� �߰�

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;
    public GameObject[] portalPoints;
    private int currentPortalPointIndex = 0;
    private GameObject Player;
    private PlayerMovement playerMovement;
    public GameObject defaultPoint;
    public CinemachineVirtualCamera defaultvirtualCamera;  // �� ��Ż ������ �����ϴ� ���� ī�޶� �迭

    public Image fadeImage;
    public float fadeDuration = 1.0f;
    private bool isFading = false;

    private GameManager gameManager;

    public CinemachineVirtualCamera[] virtualCameras;  // �� ��Ż ������ �����ϴ� ���� ī�޶� �迭

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = Player.GetComponent<PlayerMovement>();

        if (portalPoints.Length == 0)
        {
            Debug.LogError("��Ż ������ �������� �ʾҽ��ϴ�. �ν����Ϳ��� ��Ż ������ �������ּ���.");
        }
    }

    public void OnPortalEnter(PortalGate portal)
    {
        if (isFading)
            return;

        StartCoroutine(FadeAndMove(portal.portalNumber));
    }

    public void OnPortalTeleport(int point)
    {
        if (isFading)
            return;

        gameManager.ChangeGameState(GameState.Event);
        StartCoroutine(FadeAndMove(point));
    }

    IEnumerator FadeAndMove(int point)
    {
        isFading = true;
        gameManager.isPortalTransition = true;
        // �ð� ���� �� �÷��̾� �ִϸ����� ��Ȱ��ȭ
        Time.timeScale = 0f;
        playerMovement.SetAnimatorEnabled(false);

        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f));

        currentPortalPointIndex = point;
        // �÷��̾� �̵�
        if (currentPortalPointIndex >= 0 && currentPortalPointIndex < portalPoints.Length)
        {
            Player.transform.position = portalPoints[currentPortalPointIndex].transform.position;
        }
        else
        {
            Player.transform.position = defaultPoint.transform.position;
            currentPortalPointIndex = 0;
            SwitchCamera(-1);
            Debug.Log("�߸��� �ڷ���Ʈ �����Դϴ�. �⺻ �������� �̵��մϴ�.");
        }

        // ī�޶� �̵�
        SwitchCamera(point);
        // ���̵� ��
        yield return StartCoroutine(Fade(0f));

        gameManager.ChangeGameState(GameState.None);

        // �ð� �簳 �� �÷��̾� �ִϸ����� Ȱ��ȭ
        Time.timeScale = 1f;
        playerMovement.SetAnimatorEnabled(true);
        gameManager.isPortalTransition = false;
        isFading = false;
    }

    void SwitchCamera(int point)
    {
        // ��� ���� ī�޶� ��Ȱ��ȭ
        foreach (var cam in virtualCameras)
        {
            cam.gameObject.SetActive(false);
            defaultvirtualCamera.gameObject.SetActive(false);
        }

        // ����Ʈ�� �ش��ϴ� ���� ī�޶� Ȱ��ȭ
        if (point >= 0 && point < virtualCameras.Length)
        {
            virtualCameras[point].transform.position = portalPoints[point].transform.position;
            virtualCameras[point].gameObject.SetActive(true);
        }
        else
        {
            defaultvirtualCamera.transform.position = defaultPoint.transform.position;
            defaultvirtualCamera.gameObject.SetActive(true);
        }
    }

    IEnumerator Fade(float targetAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        float startAlpha = fadeImage.color.a;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;

            elapsedTime += Time.unscaledDeltaTime; // �ð� ���� ���¿��� ���̵尡 �ùٸ��� ����ǵ��� unscaledDeltaTime ���
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}

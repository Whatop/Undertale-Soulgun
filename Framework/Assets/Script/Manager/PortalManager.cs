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
    public Camera mainsCamera;

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
        SwitchCamera(-1);
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

        // �÷��̾� �̵��� ���õ� ��� �Է� ����
        playerMovement.enabled = false; // �Է��� ��Ȱ��ȭ
        playerMovement.SetAnimatorEnabled(false); // �ִϸ����� ��Ȱ��ȭ

        // �ð� ����
        Time.timeScale = 0f;

        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f));

        // �÷��̾� �̵�
        currentPortalPointIndex = point;
        if (currentPortalPointIndex >= 0 && currentPortalPointIndex < portalPoints.Length)
        {
            Player.transform.position = portalPoints[currentPortalPointIndex].transform.position;
            SwitchCamera(point);
        }
        else
        {
            Player.transform.position = defaultPoint.transform.position;
            currentPortalPointIndex = 0;
            SwitchCamera(-1);
            Debug.Log("�߸��� �ڷ���Ʈ �����Դϴ�. �⺻ �������� �̵��մϴ�.");
        }

        // ���̵� ��
        yield return StartCoroutine(Fade(0f));

        // ���� �ʱ�ȭ
        gameManager.ChangeGameState(GameState.None);

        // �ð� �簳
        Time.timeScale = 1f;

        // �÷��̾� �Է� �ٽ� Ȱ��ȭ
        playerMovement.enabled = true; // �Է� �ٽ� Ȱ��ȭ
        playerMovement.SetAnimatorEnabled(true); // �ִϸ����� �ٽ� Ȱ��ȭ
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
        switch (point)
        {
            case 0:
                virtualCameras[1].transform.position = portalPoints[1].transform.position;
                mainsCamera.transform.position = virtualCameras[1].transform.position;
                virtualCameras[1].gameObject.SetActive(true);
                break;

            case 1:
                virtualCameras[0].transform.position = portalPoints[0].transform.position;
                mainsCamera.transform.position = virtualCameras[0].transform.position;
                virtualCameras[0].gameObject.SetActive(true);
                break;

            case 2:
                virtualCameras[2].transform.position = portalPoints[2].transform.position;
                mainsCamera.transform.position = virtualCameras[2].transform.position;
                virtualCameras[2].gameObject.SetActive(true);
                break;

            case 3:
                virtualCameras[1].transform.position = portalPoints[2].transform.position;
                mainsCamera.transform.position = virtualCameras[2].transform.position;
                virtualCameras[1].gameObject.SetActive(true);
                break;

            case 4:
                virtualCameras[3].transform.position = portalPoints[2].transform.position;
                mainsCamera.transform.position = virtualCameras[2].transform.position;
                virtualCameras[3].gameObject.SetActive(true);
                break;
            default:
                defaultvirtualCamera.transform.position = defaultPoint.transform.position;
                mainsCamera.transform.position = defaultvirtualCamera.transform.position;
                defaultvirtualCamera.gameObject.SetActive(true);
                break;
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

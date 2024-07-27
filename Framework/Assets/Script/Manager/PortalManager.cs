using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;
    public GameObject[] portalPoints;
    [SerializeField]
    private int currentPortalPointIndex = 0;
    private GameObject Player;
    private PlayerMovement playerMovement;
    public GameObject defaultPoint;

    public Image fadeImage;
    public float fadeDuration = 1.0f;
    private bool isFading = false;

    private GameManager gameManager;
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

        gameManager.ChangeGameState(GameState.Event);
        switch (portal.portalNumber)
        {
            case 0:
                gameManager.ChangeCameraState(CameraType.All);
                break;
            case 1:
                gameManager.ChangeCameraState(CameraType.Hor, portal.portalNumber);
                break;
           // case 2:
           //     gameManager.ChangeCameraState(CameraType.All);
           //     break;
           // case 3:
           //     gameManager.ChangeCameraState(CameraType.All);
           //     break;
        }
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

        // �ð� ���� �� �÷��̾� �ִϸ����� ��Ȱ��ȭ
        Time.timeScale = 0f;
        playerMovement.SetAnimatorEnabled(false);

        // ���̵� �ƿ�
        yield return StartCoroutine(Fade(1f));

        // �÷��̾� �̵�
        if (point >= 0 && point < portalPoints.Length)
        {
            Player.transform.position = portalPoints[point].transform.position;
        }
        else
        {
            Player.transform.position = defaultPoint.transform.position;
            currentPortalPointIndex = 0;
            Debug.Log("�߸��� �ڷ���Ʈ �����Դϴ�. �⺻ �������� �̵��մϴ�.");
        }

        // ���̵� ��
        yield return StartCoroutine(Fade(0f));

        // �ð� �簳 �� �÷��̾� �ִϸ����� Ȱ��ȭ
        Time.timeScale = 1f;
        playerMovement.SetAnimatorEnabled(true);

        gameManager.ChangeGameState(GameState.None);
        isFading = false;
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

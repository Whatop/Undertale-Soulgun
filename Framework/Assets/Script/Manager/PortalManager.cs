using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Cinemachine ���ӽ����̽� �߰�
using UnityEngine.UI; // Cinemachine ���ӽ����̽� �߰�

[System.Serializable]
public class PortalData
{
    public int portalNumber;
    public GameObject portalPoint;
    public CinemachineVirtualCamera virtualCamera;
}



public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;
    public GameObject[] portalPoints;
    private int currentPortalPointIndex = 0;
    public int lastPortalNumber = -1; // �ʱⰪ�� �⺻ ���¸� �ǹ� (-1)

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
    // �ν����Ϳ��� ����
    public List<PortalData> portalDataList;

    public GameObject[] Rooms;

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
        //SwitchCamera();
    }
    public void HandlePortal(int point)
    {
        if (isFading)
            return;

        StartCoroutine(FadeAndMove(point));
    }

    public void OnPortalEnter(PortalGate portal)
    {
        HandlePortal(portal.portalNumber);
    }

    public void OnPortalTeleport(int point)
    {
        gameManager.ChangeGameState(GameState.Event);
        HandlePortal(point);
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
        else if (point == 999)
        {
            Player.transform.position = portalPoints[5].transform.position;
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
        lastPortalNumber = point; // ���� Ȱ��ȭ�� ��Ż ��ȣ ����

        foreach (var data in portalDataList)
        {
            data.virtualCamera.gameObject.SetActive(data.portalNumber == point);
        }
        defaultvirtualCamera.gameObject.SetActive(!portalDataList.Exists(data => data.portalNumber == point));

        Debug.Log(lastPortalNumber + " : ��ȣ�� �̵���");
    
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

            elapsedTime += isFading ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        color.a = targetAlpha;
        fadeImage.color = color;
    }
}

using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance; // �̱��� �ν��Ͻ�

    public GameObject[] portalPoints; // ��Ż �̵�����Ʈ GameObject �迭
    private int currentPortalPointIndex = 0; // ���� ��Ż ����Ʈ�� �ε���
    private GameObject Player; // ��Ż ����Ʈ GameObject �迭
    public GameObject defaultPoint;


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
    }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (portalPoints.Length == 0)
        {
            Debug.LogError("Portal points are not assigned. Please assign portal points in the inspector.");
        }
    }

    public void OnPortalEnter(PortalGate portal)
    {
        // ��Ż ��ȣ�� ���� �̵� ���� ����
        int direction = (portal.portalNumber % 2 == 0) ? 1 : -1;

        // ���� ����Ʈ �ε��� ���
        currentPortalPointIndex += direction;
        Debug.Log(currentPortalPointIndex + "�� ��Ż�� �̵�");
        // ����Ʈ �迭 ���� üũ
        if (currentPortalPointIndex >= 0 && currentPortalPointIndex < portalPoints.Length)
        {
            // �÷��̾ �ش� ����Ʈ�� �̵�
            Player.transform.position = portalPoints[currentPortalPointIndex].transform.position;
        }
        else
        {
            // ������ ��� ��� �⺻ ����Ʈ�� �̵�
            Player.transform.position = defaultPoint.transform.position;
            currentPortalPointIndex = 0;
        }
    }
}

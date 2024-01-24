using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance; // �̱��� �ν��Ͻ�

    private GameObject[] portals; // ��Ż ����Ʈ GameObject �迭
    public GameObject[] portalPoints; // ��Ż ����Ʈ GameObject �迭
    private int currentPortalPointIndex = 0; // ���� ��Ż ����Ʈ�� �ε���
    public GameObject Player; // ��Ż ����Ʈ GameObject �迭

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
        portals = GameObject.FindGameObjectsWithTag("Portal");
        Player = GameObject.FindGameObjectWithTag("Player");

        if (portalPoints.Length == 0)
        {
            Debug.LogError("Portal points are not assigned. Please assign portal points in the inspector.");
        }
    }

    public void OnPortalEnter(PortalPoint portal)
    {
        Debug.Log("�۵��� �Ǵ°�?");

        // ���� ��Ż�� ��ȣ�� ���Ͽ� ���� ����Ʈ�� �̵�
        int currentPortalNumber = portals[currentPortalPointIndex].GetComponent<PortalPoint>().portalNumber;

        // ���� ����Ʈ�� �̵� (��ȯ)
        currentPortalPointIndex = (currentPortalPointIndex + 1) % portals.Length;

        // �÷��̾ �ش� ����Ʈ�� �̵���Ű�ų� �ٸ� ���� ����
        int nextPortalNumber = portals[currentPortalPointIndex].GetComponent<PortalPoint>().portalNumber;

        if (nextPortalNumber % 2 == 0)
        {
            // ¦���� ��� ������ �̵� ���� �߰�
            // ����: MovePlayerForward();
            Player.transform.position = portalPoints[nextPortalNumber + 1].transform.position;
        }
        else
        {
            Player.transform.position = portalPoints[nextPortalNumber - 1].transform.position;
            // Ȧ���� ��� �ڷ� �̵� ���� �߰�
            // ����: MovePlayerBackward();
        }
    }
}

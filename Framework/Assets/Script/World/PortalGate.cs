using UnityEngine;

public class PortalGate : MonoBehaviour
{
    public int portalNumber; // ��Ż�� ��ȣ

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PortalManager.Instance.OnPortalTeleport(portalNumber);
        }
    }
}

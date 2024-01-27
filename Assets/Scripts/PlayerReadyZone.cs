using UnityEngine;

public class PlayerReadyZone : MonoBehaviour
{
    private GameObject detected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (detected != null && (detected.GetComponent<PlayerController>()?.isReady ?? false))
            return;

        if (collision.TryGetComponent(out PlayerController playerController))
        {
            detected = collision.gameObject;
            playerController.SetReady();
        }
    }
}

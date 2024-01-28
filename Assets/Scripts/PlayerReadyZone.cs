using UnityEngine;

public class PlayerReadyZone : MonoBehaviour
{
    public Color playerColor;
    public Transform mainSpawn;
    public Transform copySpawn;

    private GameObject detected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (detected != null && detected.TryGetComponent(out PlayerController detectedController) && detectedController.isReady)
            return;

        if (collision.TryGetComponent(out PlayerController playerController) && playerController.playerName == "main")
        {
            detected = collision.gameObject;
            playerController.SetReady(this);
        }
    }
}

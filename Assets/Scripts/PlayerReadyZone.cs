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

        if (collision.transform.parent.TryGetComponent(out PlayerController playerController) && playerController.playerName == "main")
        {
            detected = collision.transform.parent.gameObject;
            playerController.SetReady(this);
        }
    }
}

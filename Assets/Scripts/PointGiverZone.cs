using UnityEngine;

public class PointGiverZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.TryGetComponent(out PlayerController controller))
            controller.StartPointIncrease();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.TryGetComponent(out PlayerController controller))
            controller.StopPointIncrease();
    }
}

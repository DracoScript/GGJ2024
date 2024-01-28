using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            GameController.Instance.points[collision.gameObject.GetComponentInParent<PlayerController>().id] += 10;
            Destroy(this.gameObject);
        }
    }
}

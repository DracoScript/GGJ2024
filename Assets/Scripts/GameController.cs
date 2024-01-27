using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public static GameController instance;
    public Transform[] pos1, pos2;

    private int qtdPlayers = 0;

    void Awake() {
        instance = this;
    }

    public void NewPlayer(string playerName, GameObject player) {
        if(playerName == "main") {
            player.transform.position = pos1[qtdPlayers].position;
        }
        else {
            player.transform.position = pos2[qtdPlayers].position;
            qtdPlayers++;
        }
    }
}

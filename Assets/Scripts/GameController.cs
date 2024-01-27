using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform[] pos1, pos2;

    public List<int> points = new();
    public List<GameObject> mainPlayers = new();
    public List<GameObject> copyPlayers = new();

    // Singleton
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void NewPlayer(string playerName, GameObject player)
    {
        if (playerName == "main")
        {
            mainPlayers.Add(player);
            if (mainPlayers.Count < 4)
                player.transform.position = pos1[mainPlayers.Count - 1].position;
            else
                Debug.LogWarning("Limit de 4 player atingido!");

            if (player.TryGetComponent(out PlayerController controller))
                controller.id = mainPlayers.Count - 1;

            points.Add(0);
        }
        else
        {
            copyPlayers.Add(player);
            if (copyPlayers.Count < 4)
                player.transform.position = pos2[copyPlayers.Count - 1].position;

            if (player.TryGetComponent(out PlayerController controller))
                controller.id = copyPlayers.Count - 1;
        }
    }

    public void CheckReady()
    {
        foreach (GameObject player in mainPlayers)
        {
            if (!player.GetComponent<PlayerController>()?.isReady ?? false)
                return; // Someone is not ready
        }

        StartGame();
    }

    public void StartGame()
    {
        for (int i = 0; i < points.Count; i++)
            points[i] = 0;

        // TODO: Ajustar a posição do player com stat.Object.transform 
    }
}

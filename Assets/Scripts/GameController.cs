using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Lobby Spawns")]
    public Transform mainSpawn;
    public Transform copySpawn;

    [Header("Cameras")]
    public GameObject lobbyCamera;
    public GameObject game1Camera;
    public GameObject game2Camera;

    [Header("Players")]
    public List<int> points = new();
    public List<GameObject> mainPlayers = new();
    public List<GameObject> copyPlayers = new();

    [Header("Other Configs")]
    public List<GameObject> games;
    public List<PlayerReadyZone> zones;

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
        if (playerName != "main")
            return;

        // Main
        mainPlayers.Add(player);
        if (mainPlayers.Count < 5)
            player.transform.position = mainSpawn.position;
        else
            Debug.LogWarning("Limit de 4 player atingido!");

        if (player.TryGetComponent(out PlayerController mainController))
            mainController.id = mainPlayers.Count - 1;

        points.Add(0);

        // Copy
        GameObject copy = player.transform.parent.GetChild(1).gameObject;
        copyPlayers.Add(copy);
        if (copyPlayers.Count < 5)
            copy.transform.position = copySpawn.position;

        if (copy.TryGetComponent(out PlayerController copyController))
            copyController.id = copyPlayers.Count - 1;
    }

    public void CheckReady()
    {
        if (mainPlayers.Count < 2)
            return;

        foreach (GameObject player in mainPlayers)
        {
            if (player.TryGetComponent(out PlayerController controller) && !controller.isReady)
                return; // This player is not ready
        }

        StartGame();
    }

    public void StartGame()
    {
        if (games.Count < 2)
            Debug.LogError("Precisa configurar pelo menos 2 games no GameController.");

        // Random Games - TODO: fazer ser random mesmo
        GameObject game1 = games[0];
        GameObject game2 = games[1];

        // TODO: Ajustar as cameras corretas para cada game

        // Arruma os spawns
        foreach (Transform t in game1.GetComponentsInChildren<Transform>())
        {
            if (t.name == "Spws")
            {
                if (t.childCount < zones.Count)
                    Debug.LogError(t.name + " tem menos spawners configurados do que os " + zones.Count + " necessários");

                for (int i = 0; i < zones.Count; i++)
                    zones[i].mainSpawn = t.GetChild(i);

                break;
            }
        }
        foreach (Transform t in game2.GetComponentsInChildren<Transform>())
        {
            if (t.name == "Spws")
            {
                if (t.childCount < zones.Count)
                    Debug.LogError(t.name + " tem menos spawners configurados do que os " + zones.Count + " necessários");

                for (int i = 0; i < zones.Count; i++)
                    zones[i].copySpawn = t.GetChild(i);

                break;
            }
        }

        // Zera pontuações
        for (int i = 0; i < points.Count; i++)
            points[i] = 0;

        // Movimenta os jogadores
        for (int i = 0; i < mainPlayers.Count; i++)
        {
            if (mainPlayers[i].TryGetComponent(out PlayerController controller) && controller.zone != null)
            {
                mainPlayers[i].transform.position = controller.zone.mainSpawn.position;
                copyPlayers[i].transform.position = controller.zone.copySpawn.position;

                controller.ClearReady();
            }
        }

        game1Camera.SetActive(true);
        game2Camera.SetActive(true);
        lobbyCamera.SetActive(false);
    }

    public void EndGame()
    {
        for (int i = 0; i < mainPlayers.Count; i++)
        {
            mainPlayers[i].transform.position = mainSpawn.position;
            copyPlayers[i].transform.position = copySpawn.position;

            if (mainPlayers[i].TryGetComponent(out PlayerController controller))
            {
                controller.ClearReady(true);
            }
        }

        lobbyCamera.SetActive(true);
        game1Camera.SetActive(false);
        game2Camera.SetActive(false);
    }
}

using System.Collections.Generic;
using System.Linq;
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
        player.transform.position = mainSpawn.position;

        if (mainPlayers.Count > 4)
            Debug.LogWarning("Limit de 4 player excedido!");

        if (player.TryGetComponent(out PlayerController mainController))
            mainController.id = mainPlayers.Count - 1;

        points.Add(0);

        // Copy
        GameObject copy = player.transform.parent.GetChild(1).gameObject;
        copyPlayers.Add(copy);
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

        // Random Games
        games = games.OrderBy(item => Random.value).ToList();
        GameObject game1 = games[0];
        GameObject game2 = games[1];

        // Ajusta as cameras para a posição dos games escolhidos
        game1Camera.transform.position = game1.transform.position;
        game2Camera.transform.position = game2.transform.position;

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

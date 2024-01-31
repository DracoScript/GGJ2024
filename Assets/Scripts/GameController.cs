using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("EndGame")]
    public GameObject endCanvas;
    public GameObject gridWinner;
    public GameObject gridLosers;
    public GameObject resultPrefab;

    [Header("Other Configs")]
    public List<GameObject> games;
    public List<PlayerReadyZone> zones;
    public GameObject tutorial;
    public GameObject tutorialText;

    [Header("Time")]
    public GameObject timerObject;
    public TMP_Text timeTxt;
    public float gameTime = 120;

    private float timeRemaining; //Reinicia tempo
    private bool timerIsRunning = false;
    private GameObject game1;
    private GameObject game2;
    private List<GameObject> resultList = new();

    private bool tutorialool = false;

    // Singleton
    public static GameController Instance { get; private set; }
    private void Awake()
    {
        endCanvas.SetActive(false);
        timerObject.SetActive(false);

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        timeRemaining = gameTime;

        //DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                if (timeRemaining < 0)
                    timeRemaining = 0;

                DisplayTime(timeRemaining);
            }
            else
            {
                timeRemaining = gameTime;
                DisplayTime(timeRemaining);
                timerIsRunning = false;
                EndGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
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
        tutorial.SetActive(false);
        tutorialText.SetActive(false);
        if (games.Count < 2)
            Debug.LogError("Precisa configurar pelo menos 2 games no GameController.");

        timerObject.SetActive(true);
        timerIsRunning = true;

        // Random Games
        games = games.OrderBy(item => Random.value).ToList();
        game1 = games[0];
        game2 = games[1];

        // Ajusta as cameras para a posi��o dos games escolhidos
        game1Camera.transform.position = game1.transform.position;
        game2Camera.transform.position = game2.transform.position;

        // Arruma os spawns
        foreach (Transform t in game1.GetComponentsInChildren<Transform>())
        {
            if (t.name == "Spws")
            {
                if (t.childCount < zones.Count)
                    Debug.LogError(t.name + " tem menos spawners configurados do que os " + zones.Count + " necess�rios");

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
                    Debug.LogError(t.name + " tem menos spawners configurados do que os " + zones.Count + " necess�rios");

                for (int i = 0; i < zones.Count; i++)
                    zones[i].copySpawn = t.GetChild(i);

                break;
            }
        }

        // Zera pontua��es
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

        // Iniciar spawn de moedas
        if (game1.TryGetComponent(out CoinsSpawner spawner1))
            spawner1.enabled = true;
        if (game2.TryGetComponent(out CoinsSpawner spawner2))
            spawner2.enabled = true;

        game1Camera.SetActive(true);
        game2Camera.SetActive(true);
        lobbyCamera.SetActive(false);
    }

    public void EndGame()
    {
        // Encerra spawn de moedas
        if (game1.TryGetComponent(out CoinsSpawner spawner1))
        {
            spawner1.ClearCoins();
            spawner1.enabled = false;
        }
        if (game2.TryGetComponent(out CoinsSpawner spawner2))
        {
            spawner2.ClearCoins();
            spawner2.enabled = false;
        }

        // Movendo os players para o Lobby
        for (int i = 0; i < mainPlayers.Count; i++)
        {
            mainPlayers[i].transform.position = mainSpawn.position;
            copyPlayers[i].transform.position = copySpawn.position;

            if (mainPlayers[i].TryGetComponent(out PlayerController controller))
            {
                controller.ClearReady(true);
            }
        }

        timerObject.SetActive(false);

        // Mostrar resultados
        ShowResults();

        lobbyCamera.SetActive(true);
        game1Camera.SetActive(false);
        game2Camera.SetActive(false);
    }

    public void ShowResults()
    {
        endCanvas.SetActive(true);
        StartCoroutine("ShowResultsStepByStep");
    }

    public IEnumerator ShowResultsStepByStep()
    {
        // Check winner
        Dictionary<int, GameObject> results = new Dictionary<int, GameObject>();
        for (int i = 0; i < points.Count; i++)
        {
            results.Add(points[i], mainPlayers[i]);
        }

        results = results.OrderBy(item => item.Key).ToDictionary(item => item.Key, item => item.Value);

        for (int i = 0; i < points.Count; i++)
        {
            yield return new WaitForSeconds(1);
            GameObject result = CreateResult(results.ElementAt(i).Key, results.ElementAt(i).Value);
            if (i >= points.Count - 1)
                result.transform.SetParent(gridWinner.transform);
            else
                result.transform.SetParent(gridLosers.transform);
            resultList.Add(result);
        }

        foreach ((int point, GameObject player) in results)
        {
            yield return new WaitForSeconds(1);
            CreateResult(point, player);
        }
    }

    private GameObject CreateResult(int point, GameObject player)
    {
        GameObject result = Instantiate(resultPrefab);

        // Cor
        if (player.TryGetComponent(out PlayerController controller) && controller.zone != null)
            result.GetComponentInChildren<CanvasRenderer>().SetColor(controller.zone.playerColor);

        // Player
        result.transform.GetChild(0).GetComponent<TMP_Text>().text = "Player";

        // Points
        result.transform.GetChild(1).GetComponent<TMP_Text>().text = point + "p";

        return result;
    }

    public void CloseEndCanvas()
    {

        if (endCanvas.activeSelf)
        {
            // Destroy resultados
            foreach (GameObject r in resultList)
            {
                Destroy(r, 1);
            }
            resultList = new();

            endCanvas.SetActive(false);
            tutorialText.SetActive(true);

            SceneManager.LoadScene(0);
        }
        else if (lobbyCamera.activeSelf)
        {
            if (tutorialool)
                tutorial.SetActive(false);
            else
                tutorial.SetActive(true);

            tutorialool = !tutorialool;
        }
    }
}

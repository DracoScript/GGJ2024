using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public List<PlayerStat> playerStats;

    // Singleton
    public static GameState Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerStats = new List<PlayerStat>();

        DontDestroyOnLoad(gameObject);
    }

    public void CheckReady()
    {
        foreach (PlayerStat stat in playerStats)
        {
            if (!stat.Object.GetComponent<PlayerController>()?.isReady ?? false)
            {
                Debug.Log(stat.Object.name + " is not ready");
                return;
            }
        }

        StartGame();
    }

    public void StartGame()
    {
        foreach (PlayerStat stat in playerStats)
        {
            stat.Points = 0;
            // TODO: Ajustar a posição do player com stat.Object.transform 
        }
    }
}

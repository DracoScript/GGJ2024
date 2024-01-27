using UnityEngine;

public class GameState : MonoBehaviour
{
    // Singleton
    public static GameState Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Content

    public int[] teamPoints;

    public void InitiateState(int teamCount)
    {
        teamPoints = new int[teamCount];
    }
}

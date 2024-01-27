using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        GameState.Instance.InitiateState(2); // 2 teams

        SceneManager.LoadScene(1);
    }
}

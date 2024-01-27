using TMPro;
using UnityEngine;

public class GameCanvasController : MonoBehaviour
{
    public TMP_Text team1Points;
    public TMP_Text team2Points;

    void Update()
    {
        team1Points.text = GameState.Instance.teamPoints[0].ToString("000000");
        team2Points.text = GameState.Instance.teamPoints[1].ToString("000000");
    }
}

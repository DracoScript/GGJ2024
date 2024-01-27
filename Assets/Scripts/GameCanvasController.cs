using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvasController : MonoBehaviour
{
    public List<TMP_Text> points;

    void Update()
    {
        for (var i = 0; i < GameState.Instance.playerStats.Count; i++)
        {
            if (points.Count > i)
                points[i].text = GameState.Instance.playerStats[i].Points.ToString("000000");
        }
    }
}

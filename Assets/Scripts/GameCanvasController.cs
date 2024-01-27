using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCanvasController : MonoBehaviour
{
    public List<TMP_Text> points;

    void Update()
    {
        for (var i = 0; i < GameController.Instance.points.Count; i++)
        {
            if (points.Count > i)
                points[i].text = GameController.Instance.points[i].ToString("000000");
        }
    }
}

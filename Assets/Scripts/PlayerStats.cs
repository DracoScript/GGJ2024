using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerStat
    {
        public bool IsConnected { get; set; }
        public int Points { get; set; }
        public GameObject Object { get; set; }

        public PlayerStat(GameObject obj)
        {
            IsConnected = true;
            Points = 0;
            Object = obj;
        }
    }
}
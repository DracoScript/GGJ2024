using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public Transform spawnSpot;
    public int maxCoins = 3;

    private List<Transform> spots = new();
    private List<GameObject> coins = new();

    void Start()
    {
        if (!coinPrefab)
            Debug.LogError("Precisa adicionar o prefab da moeda!");
        else
        {
            // Pega spots
            for (int i = 0; i < spawnSpot.childCount; i++)
                spots.Add(spawnSpot.GetChild(i));

            StartCoroutine("TimedSpawn");
        }
    }

    public IEnumerator TimedSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 4.0f));

            while (coins.Count < spots.Count)
            {
                // Procura uma posição não usada
                bool found = true;
                Vector3 pos = spots[Random.Range(0, spots.Count)].position;
                foreach (GameObject coin in coins)
                {
                    if (coin != null && pos == coin.transform.position)
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    // Create
                    GameObject newCoin = Instantiate(coinPrefab);
                    newCoin.transform.position = pos;
                    coins.Add(newCoin);

                    break;
                }
            }

            // Removendo moedas deletadas
            RemovingDeletedCoins();

            // Wait collect
            while (coins.Count >= maxCoins)
            {
                yield return new WaitForSeconds(0.1f);
                RemovingDeletedCoins();
            }
        }
    }

    private void RemovingDeletedCoins()
    {
        foreach (GameObject coin in coins)
        {
            if (coin == null)
            {
                coins.Remove(coin);
                break;
            }
        }
    }

    public void ClearCoins()
    {
        StopCoroutine("TimedSpawn");

        foreach (GameObject coin in coins)
        {
            if (coin != null)
                DestroyImmediate(coin);
        }

        coins = new();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class SpawnFish : MonoBehaviour
{
    public Transform[] spawnPoints;

    void Start()
    {
        SpawnGroupedFish();
    }

    void SpawnGroupedFish()
    {
        var groupedFish = new Dictionary<FishData, int>();

        // Count fish
        foreach (FishData fish in FishManager.Instance.caughtFish)
        {
            if (groupedFish.ContainsKey(fish))
                groupedFish[fish]++;
            else
                groupedFish.Add(fish, 1);
        }

        // Spawn by group
        foreach (var pair in groupedFish)
        {
            FishData fishType = pair.Key;
            int count = pair.Value;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            for (int i = 0; i < count; i++)
            {
                Vector3 offset = Random.insideUnitSphere * 2f;

                GameObject obj = Instantiate(fishType.underseaPrefab,spawnPoint.position + offset,
                    Quaternion.identity);

                Rigidbody[] rbs = obj.GetComponentsInChildren<Rigidbody>();

                foreach (var rb in rbs)
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
        }
    }
}
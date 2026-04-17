using UnityEngine;
using System.Collections.Generic;

public class FishManager : MonoBehaviour
{
    public static FishManager Instance;

    public List<FishData> caughtFish = new List<FishData>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("FishManager");
            obj.AddComponent<FishManager>();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddFish(FishData fish)
    {
        caughtFish.Add(fish);
    }
}
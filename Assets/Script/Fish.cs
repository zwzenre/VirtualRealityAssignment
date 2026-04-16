using UnityEngine;

public class Fish : MonoBehaviour
{
    public FishData data;
    public bool isCollected = false;
}

[System.Serializable]
public class FishData
{
    public string fishName;
    public GameObject prefab;
    public GameObject underseaPrefab;
    public float score;
}
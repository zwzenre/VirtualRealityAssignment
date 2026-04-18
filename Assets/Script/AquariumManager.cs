using UnityEngine;

public class AquariumManager : MonoBehaviour
{
    public static AquariumManager Instance;
    public Transform center;

    void Awake()
    {
        Instance = this;
    }
}
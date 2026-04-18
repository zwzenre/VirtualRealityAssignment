using UnityEngine;

public class FishFood : MonoBehaviour
{
    void OnEnable()
    {
        FishFoodManager.Register(this);
    }

    void OnDisable()
    {
        FishFoodManager.Unregister(this);
    }

    public void Consume()
    {
        gameObject.SetActive(false);
    }
}
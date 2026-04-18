using UnityEngine;

public class FishFeeder : MonoBehaviour
{
    public GameObject fishFoodObject;

    public void SpawnFood()
    {
        if (fishFoodObject == null)
            return;

        fishFoodObject.SetActive(true);
    }

    public void HideFood()
    {
        if (fishFoodObject == null)
            return;

        fishFoodObject.SetActive(false);
    }
}

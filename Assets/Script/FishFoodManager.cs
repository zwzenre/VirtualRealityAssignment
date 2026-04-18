using System.Collections.Generic;
using UnityEngine;

public class FishFoodManager : MonoBehaviour
{
    public static readonly List<FishFood> ActiveFood = new List<FishFood>();

    public static void Register(FishFood food)
    {
        if (food != null && !ActiveFood.Contains(food))
            ActiveFood.Add(food);
    }

    public static void Unregister(FishFood food)
    {
        if (food != null)
            ActiveFood.Remove(food);
    }

    public static FishFood GetClosestFood(Vector3 position, float maxRange)
    {
        FishFood closest = null;
        float closestSqr = maxRange * maxRange;

        foreach (var food in ActiveFood)
        {
            if (food == null) continue;

            float sqr = (food.transform.position - position).sqrMagnitude;
            if (sqr < closestSqr)
            {
                closest = food;
                closestSqr = sqr;
            }
        }

        return closest;
    }
}

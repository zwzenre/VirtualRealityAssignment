using TMPro;
using UnityEngine;

public class StoreFish : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        GameObject gcObj = GameObject.FindWithTag("GameController");
        gameController = gcObj ? gcObj.GetComponent<GameController>() : null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            Fish fishComponent = other.GetComponent<Fish>();

            if (fishComponent != null && fishComponent.data != null && !fishComponent.isCollected && gameController != null)
            {
                fishComponent.isCollected = true;

                gameController.caughtFish.Add(fishComponent.data);

                Destroy(other.gameObject);
            }
        }
    }
}

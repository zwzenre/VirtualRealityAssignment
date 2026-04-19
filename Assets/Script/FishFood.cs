using UnityEngine;

public class FishFood : MonoBehaviour
{
    public AudioClip biteClip;
    public float biteVolume = 1f;

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
        if (biteClip != null)
        {
            AudioSource.PlayClipAtPoint(biteClip, transform.position, biteVolume);
        }

        Destroy(gameObject);
    }
}
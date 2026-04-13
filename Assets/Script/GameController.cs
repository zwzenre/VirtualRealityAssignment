using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public FishingRodCasting rod;
    public TextMeshProUGUI statusText;
    //public AudioSource audioSource;
    //public AudioClip biteSound;

    enum FishingState
    {
        Idle,
        Waiting,
        FishBite,
        Hooked
    }

    FishingState state = FishingState.Idle;

    void Update()
    {
        if (rod == null) return;


        if (rod.IsCast() && state == FishingState.Idle)
        {
            state = FishingState.Waiting;
            Debug.Log("Waiting for fish...");
            statusText.text = "Waiting for fish...";
        }

        if (state == FishingState.Waiting)
        {
            //if (Random.value < 0.002f)
            //{
            //    state = FishingState.FishBite;
            //    Debug.Log("Fish is biting!");
            //    statusText.text = "Fish is biting!";
            //}

            if (Input.GetKeyDown(KeyCode.F))
            {
                state = FishingState.FishBite;
                Debug.Log("Fish is biting!");
                statusText.text = "Fish is biting!";
                //audioSource.PlayOneShot(biteSound);
            }
        }

        if (state == FishingState.FishBite)
        {
            rod.SimulateFish();
            

            if (Input.GetKey(KeyCode.R))
            {
                state = FishingState.Hooked;
                Debug.Log("Hooked!");
                statusText.text = "Hooked! Reel now!";
                rod.SpawnFish();
            }
        }

        if (state == FishingState.Hooked)
        {
            float distance = Vector3.Distance(
                rod.rodTip.position,
                rod.GetHookPosition()
            );

            float tension = distance / rod.GetLineLength();
            float tension01 = Mathf.Clamp01(tension);
            rod.line.startColor = Color.Lerp(Color.white, Color.red, tension01);
            rod.line.endColor = Color.Lerp(Color.white, Color.red, tension01);

            if (tension > 1.3f)
            {
                Debug.Log("Line broke!");
                statusText.text = "Ohno! Line broke";
                state = FishingState.Idle;
                return;
            }

            if (rod.GetLineLength() < 3f)
            {
                Debug.Log("Fish Caught!");
                statusText.text = "Yeah! Caught!";
                state = FishingState.Idle;

            }
        }
    }
}

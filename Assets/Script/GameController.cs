using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TensionUI tensionUI;
    public FishingRodCasting rod;
    public TextMeshProUGUI statusText;
    private float breakTimer = 0f;

    private float visualTension = 0f;
    public AudioSource audioSource;
    public AudioClip castSound;
    public AudioClip biteSound;
    public AudioClip reelSound;

    private float waitTimer = 0f;
    private float minWaitTime = 2f;
    private float maxWaitTime = 5f; 
    private float targetWaitTime;
    private bool isReeling = false;

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

        if (Input.GetKeyDown(KeyCode.Space) && state == FishingState.Idle)
        {
            OnCastPressed();
        }

        if (state == FishingState.Waiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= targetWaitTime)
            {
                state = FishingState.FishBite;
                statusText.text = "Fish is biting!";
                audioSource.PlayOneShot(biteSound);
            }
        }

        if (state == FishingState.FishBite)
        {
            rod.SimulateFish();

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnReelPressed();
            }
        }

        if (state == FishingState.Hooked)
        {
            if (isReeling)
            {
                Debug.Log("Reeling...");
                rod.Reel();

                if (!audioSource.isPlaying)
                    audioSource.PlayOneShot(reelSound);
            }
            else
            {
                rod.RelaxLine();
            }

            float targetTension = rod.GetTension();
            visualTension = Mathf.Lerp(visualTension, targetTension, Time.deltaTime * 3f);
            tensionUI.UpdateTension(visualTension);

            rod.SimulateFish();
            rod.line.material.color = Color.Lerp(Color.white, Color.red, visualTension);

            if (visualTension > 0.95f)
            {
                breakTimer += Time.deltaTime;

                if (breakTimer > 1f)
                {
                    statusText.text = "Line broke!";
                    ResetFishing();
                    return;
                }
            }
            else
            {
                breakTimer = 0f;
            }

            if (Vector3.Distance(rod.rodTip.position, rod.GetHookPosition()) < 2.5f)
            {
                statusText.text = "Fish Caught!";
                ResetFishing();
            }
        }
    }
    public void OnCastPressed()
    {
        if (state != FishingState.Idle) return;

        rod.Cast();
        visualTension = 0f;
        state = FishingState.Waiting;
        statusText.text = "Waiting for fish...";
        audioSource.PlayOneShot(castSound);

        targetWaitTime = Random.Range(minWaitTime, maxWaitTime);
        waitTimer = 0f;
    }

    public void OnReelPressed()
    {
        if (state == FishingState.FishBite)
        {
            rod.Reel();
            state = FishingState.Hooked;
            statusText.text = "Hooked! Reel now!";
            rod.SpawnFish();
            breakTimer = 0f;
        }

        isReeling = true;
    }

    public void OnReelReleased()
    {
        isReeling = false;
    }

    void ResetFishing()
    {
        visualTension = 0f;
        rod.line.material.color = Color.white;
        rod.ResetCast();
        tensionUI.ResetBar();
        state = FishingState.Idle;
    }
}

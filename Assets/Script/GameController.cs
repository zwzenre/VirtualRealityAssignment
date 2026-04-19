using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class GameController : MonoBehaviour
{
    public TensionUI tensionUI;
    public FishingRodCasting rod;
    public TextMeshProUGUI statusText;

    public AudioSource audioSource;
    public AudioClip castSound;
    public AudioClip biteSound;
    public AudioClip reelSound;

    public XRBaseController leftController;
    public XRBaseController rightController;

    private float visualTension = 0f;
    private float waitTimer = 0f;
    private float minWaitTime = 2f;
    private float maxWaitTime = 5f;
    private float targetWaitTime;

    private bool isReeling = false;
    private float breakTimer = 0f;

    private float hapticTimer = 0f;
    private float hapticInterval = 0.05f;

    enum FishingState
    {
        Idle,
        Waiting,
        FishBite,
        Hooked
    }

    FishingState state = FishingState.Idle;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (rod == null) return;

        HandleHaptics();

        HandleInput();

        HandleState();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && state == FishingState.Idle)
            OnCastPressed();

        if (state == FishingState.FishBite && Input.GetKeyDown(KeyCode.R))
            OnReelPressed();
    }

    void HandleState()
    {
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

        if (state != FishingState.Hooked) return;

        HandleHookedState();
    }

    void HandleHookedState()
    {
        if (isReeling)
        {
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
            FishData fish = rod.GetCurrentFishData();
            ResetFishing();
        }
    }

    void HandleHaptics()
    {
        if (state != FishingState.Hooked) return;

        hapticTimer += Time.deltaTime;

        if (hapticTimer < hapticInterval) return;

        hapticTimer = 0f;

        float intensity = Mathf.Lerp(0.1f, 1f, visualTension);

        if (rightController != null)
            rightController.SendHapticImpulse(intensity, 0.04f);
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
            FishData fish = rod.SpawnFish();
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject statusObj = GameObject.FindWithTag("StatusText");
        if (statusObj != null)
            statusText = statusObj.GetComponent<TextMeshProUGUI>();

        GameObject tensionObj = GameObject.FindWithTag("TensionUI");
        if (tensionObj != null)
            tensionUI = tensionObj.GetComponent<TensionUI>();

        GameObject rodObj = GameObject.FindWithTag("FishingRod");
        if (rodObj != null)
            rod = rodObj.GetComponent<FishingRodCasting>();

        GameObject leftCtrl = GameObject.FindWithTag("LeftController");
        if (leftCtrl != null)
            leftController = leftCtrl.GetComponent<XRBaseController>();

        GameObject rightCtrl = GameObject.FindWithTag("RightController");
        if (rightCtrl != null)
            rightController = rightCtrl.GetComponent<XRBaseController>();
    }
}
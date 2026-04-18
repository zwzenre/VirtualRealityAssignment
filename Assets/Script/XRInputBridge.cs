using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class XRInputBridge : MonoBehaviour
{
    public GameController gameController;
    public XRGrabInteractable rodGrab;
    public FishFeeder fishFeeder;
    public PauseMenu pauseMenu;

    public InputActionReference reelAction;
    public InputActionReference castAction;
    public InputActionReference feedAction;

    bool isHoldingRod = false;
    bool isInitialized = false;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnbindAll();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitAfterSceneLoad());
    }

    IEnumerator InitAfterSceneLoad()
    {
        yield return null;

        GameObject gcObj = GameObject.FindWithTag("GameController");
        gameController = gcObj ? gcObj.GetComponent<GameController>() : null;

        GameObject rodObj = GameObject.FindWithTag("FishingRod");
        rodGrab = rodObj ? rodObj.GetComponent<XRGrabInteractable>() : null;

        if (reelAction != null)
        {
            reelAction.action.Enable();
            reelAction.action.performed += OnReel;
            reelAction.action.canceled += OnReel;
        }

        if (castAction != null)
        {
            castAction.action.Enable();
            castAction.action.performed += OnCast;
        }

        if (rodGrab != null)
        {
            rodGrab.selectEntered.AddListener(OnGrab);
            rodGrab.selectExited.AddListener(OnRelease);
        }

        if(feedAction != null)
        {
            feedAction.action.Enable();
            feedAction.action.performed += OnFeed;
        }

        isInitialized = true;
    }

    void UnbindAll()
    {
        if (reelAction != null)
        {
            reelAction.action.performed -= OnReel;
            reelAction.action.canceled -= OnReel;
        }

        if (castAction != null)
        {
            castAction.action.performed -= OnCast;
        }

        if (feedAction != null)
        {
            feedAction.action.performed -= OnFeed;
        }

        if (rodGrab != null)
        {
            rodGrab.selectEntered.RemoveListener(OnGrab);
            rodGrab.selectExited.RemoveListener(OnRelease);
        }
    }

    void OnReel(InputAction.CallbackContext ctx)
    {
        if (!isInitialized || gameController == null) return;

        float value = ctx.ReadValue<float>();

        if (value > 0.1f)
            gameController.OnReelPressed();
        else
            gameController.OnReelReleased();
    }

    void OnCast(InputAction.CallbackContext ctx)
    {
        if (!isInitialized || gameController == null) return;
        if (!isHoldingRod) return;

        gameController.OnCastPressed();
    }

    void OnFeed(InputAction.CallbackContext ctx)
    {
        if (!isInitialized || fishFeeder == null)
            return;

        if (pauseMenu != null && pauseMenu.activeWristUI)
            return;

        fishFeeder.SpawnFood();
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isHoldingRod = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isHoldingRod = false;
    }
}
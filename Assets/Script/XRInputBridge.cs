using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class XRInputBridge : MonoBehaviour
{
    public GameController gameController;
    public XRGrabInteractable rodGrab;

    public InputActionReference reelAction;
    public InputActionReference castAction;

    bool isHoldingRod = false;

    void OnEnable()
    {
        reelAction.action.Enable();
        castAction.action.Enable();

        reelAction.action.performed += OnReel;
        reelAction.action.canceled += OnReel;

        castAction.action.performed += OnCast;

        rodGrab.selectEntered.AddListener(OnGrab);
        rodGrab.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        reelAction.action.performed -= OnReel;
        reelAction.action.canceled -= OnReel;

        castAction.action.performed -= OnCast;
        rodGrab.selectEntered.RemoveListener(OnGrab);
        rodGrab.selectExited.RemoveListener(OnRelease);
    }

    void OnReel(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();

        if (value > 0.1f)
            gameController.OnReelPressed();
        else
            gameController.OnReelReleased();
    }

    void OnCast(InputAction.CallbackContext ctx)
    {
        if (!isHoldingRod) return;
        gameController.OnCastPressed();
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
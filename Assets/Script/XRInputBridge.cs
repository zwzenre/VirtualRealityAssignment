using UnityEngine;
using UnityEngine.InputSystem;

public class XRInputBridge : MonoBehaviour
{
    public GameController gameController;

    public InputActionReference reelAction;
    public InputActionReference castAction;

    void OnEnable()
    {
        reelAction.action.Enable();
        castAction.action.Enable();

        reelAction.action.performed += OnReel;
        reelAction.action.canceled += OnReel;

        castAction.action.performed += OnCast;
    }

    void OnDisable()
    {
        reelAction.action.performed -= OnReel;
        reelAction.action.canceled -= OnReel;

        castAction.action.performed -= OnCast;
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
        gameController.OnCastPressed();
    }
}
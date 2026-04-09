using UnityEngine;
using UnityEngine.XR;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;

    private bool isPaused = false;
    private bool wasPressedLastFrame = false;

    void Update()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool buttonPressed = false;

        // A button on right controller
        if (rightHand.TryGetFeatureValue(CommonUsages.primaryButton, out buttonPressed))
        {
            if (buttonPressed && !wasPressedLastFrame)
            {
                TogglePause();
            }

            wasPressedLastFrame = buttonPressed;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
}
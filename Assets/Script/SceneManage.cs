using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadBeach()
    {
        SceneManager.LoadScene("Beach");
    }

    public void LoadSea()
    {
        SceneManager.LoadScene("Sea");
    }

    public void LoadLakeHouse()
    {
        SceneManager.LoadScene("LakeHouse");
    }
}
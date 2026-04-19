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

    public void LoadFishpedia()
    {
        SceneManager.LoadScene("Underwater");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); 
        Application.Quit();     
    }
}
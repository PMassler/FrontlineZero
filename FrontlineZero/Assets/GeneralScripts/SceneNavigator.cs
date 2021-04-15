using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void GoToTrainingSimulator()
    {
        SceneManager.LoadScene("TrainingSimulation");
    }

    public void GoToInGameScene()
    {
        SceneManager.LoadScene("InGame");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}

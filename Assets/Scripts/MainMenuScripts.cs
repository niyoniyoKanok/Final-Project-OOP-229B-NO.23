using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenu : MonoBehaviour
{
  
    public void PlayGame()
    {
      
        SceneManager.LoadScene("InGameScene");
    }


    public void ExitGame()
    {
        Debug.Log("Game Closed"); 
        Application.Quit(); 
    }
}

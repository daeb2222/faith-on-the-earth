using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
   

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
                ChangeScene();
            
        }
    }
    private void ChangeScene()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

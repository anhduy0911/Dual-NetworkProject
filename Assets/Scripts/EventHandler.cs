using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        //Client.closeConnection();
        //Destroy(Client.clientInstance);

        Destroy(Client.clientInstance.gameObject, 1.0f);
        Debug.Log("Back to main menu");
        SceneManager.LoadScene(0);
    }

    public void Option()
    {
        Debug.Log("Option!");
    }
}

using GameMessage;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManagement;

public class ConnectionInit : MonoBehaviour
{

    //Start is called before the first frame update
    void Start()
    {
        if (Client.clientInstance != null)
        { 
            Debug.Log("Hello, this is the client instance ptr!");
            //start init the connection, wait in a loop until successfully enter the game
            Client.startConnection();
            checkConnection();
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkConnection();
    }

    void checkConnection()
    {
        int flag = Client.clientInstance.parseReceivedMessage();

        if (flag >= 0)
        {
            switch (flag)
            {
                case 0:
                    Debug.Log("Error! We received Game State at this moment before State message...");
                    break;
                case 1:
                    Status s = Client.clientInstance.parseStatus();
                    if (s.Status_ == 1)
                    {
                        Debug.Log("Status: 1 - Entering the game...");
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                    else
                        Debug.Log("Status: " + s.Status_);
                    break;
            }
        }

    }
}

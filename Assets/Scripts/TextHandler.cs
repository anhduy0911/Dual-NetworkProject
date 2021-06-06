using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    private static TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        switch (Client.winState)
        {
            case 0:
            text.text = "YOU LOST";
            break;
            case 1:
            text.text = "YOU WON";
            break;
            case 2:
            text.text = "OPPONENT QUIT MATCH";
            break;
        }

    }
}

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    private int course = 0;
    private void Update()
    {

    }




    private void HandleGameOver()
    {
        // Implement game over logic
        statusText.text = "Game Over, you lost!";
        // Potentially disable player controls, show a restart button, etc.
    }

}

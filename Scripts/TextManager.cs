using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    private int life = 3;
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;
    private float catcherTimer = 120.0f;
    public bool isCatcher = false;
    private bool isFirstTimeJoin = true;
    private void Update()
    {
        // If the cooldown is active, decrement the timer
        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;

            // Once the cooldown finishes, disable it
            if (cooldownTimer <= 0)
            {
                isCooldownActive = false;
            }
        }
        if (isCatcher)
        {
            catcherTimer -= Time.deltaTime;
            if (catcherTimer <= 0)
            {
                life -= 1;
                catcherTimer = 120.0f; // Reset the timer
                if (life <= 0)
                {
                    HandleGameOver();
                    return;
                }
                statusText.text = $"You are catcher, life = {life}";
            }
        }
    }


    public void SetAsRunner()
    {

        if (!isCooldownActive)
        {
            isFirstTimeJoin = false;
            isCatcher = false;
            statusText.text = $"You are runner, life = {life}";
            isCooldownActive = true;
            cooldownTimer = 5f; // 5 seconds cooldown


        }
    }
        public void SetAsCatcher()
    {
        if (!isCooldownActive && !isCatcher)
        {
            isCatcher = true;
            catcherTimer = 120.0f;
            if(!isFirstTimeJoin){
                life -= 1;
            }
            isFirstTimeJoin = false;
            
            statusText.text = "You are catcher, life = " + life;

            // Activate the cooldown
            isCooldownActive = true;
            cooldownTimer = 5f; // 5 seconds cooldown

            // Check if life has reached 0 or below
            if (life <= 0)
            {
                HandleGameOver();
            }
        }
    }

    private void HandleGameOver()
    {
        // Implement game over logic
        statusText.text = "Game Over, you lost!";
        // Potentially disable player controls, show a restart button, etc.
    }

}

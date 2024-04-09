using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class ScoreCount : MonoBehaviour
{
    // Start is called before the first frame update
    private int score = 0;
    public TextMeshProUGUI statusText;
    private float scoreCooldown = 1.0f;
    private float lastScoreTime = -1.0f;
    void Start()
    {
        statusText = GameObject.Find("Tele").GetComponent<TextMeshProUGUI>();
        PhotonView[] playerViews = FindObjectsOfType<PhotonView>();
        foreach (var view in playerViews) {
        if (view.CompareTag("PlayerOG") && view.IsMine) {
            statusText.text = $"Score: {score}";
        }
        }
    }



    // Update is called once per frame
    void Update()
    {
        PhotonView[] playerViews = FindObjectsOfType<PhotonView>();
        foreach (var view in playerViews) {
        if (view.CompareTag("PlayerOG") && view.IsMine) {
            statusText.text = $"Score: {score}";
        }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    // Assuming "PlayerOG" is the tag of one specific object and this script is attached to the other specific object
        if (collision.gameObject.CompareTag("Ball"))
        {
            // This code block will only execute if the GameObject this script is attached to
            // collides with the GameObject tagged as "PlayerOG"
            if (Time.time - lastScoreTime >= scoreCooldown)
            {
                lastScoreTime = Time.time;
                score++;
            }
        }
    }
}

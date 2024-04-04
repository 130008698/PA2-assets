using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class TeleCount : MonoBehaviour
{
    // Start is called before the first frame update
    private int tele = 0;
    public TextMeshProUGUI statusText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PhotonView[] playerViews = FindObjectsOfType<PhotonView>();
        foreach (var view in playerViews) {
        if (view.CompareTag("PlayerOG") && view.IsMine) {
            MovementManager mm = view.gameObject.GetComponent<MovementManager>();
            tele = mm.tele;
            statusText.text = $"Teleportation: {tele}";
        }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Teleport : MonoBehaviourPunCallbacks
{
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other){
        if(other.CompareTag("Player")){
            MovementManager movementManager = other.GetComponentInParent<MovementManager>();
            if (movementManager != null) {
                movementManager.tele +=1;
                
            }
            Debug.Log("Teleportation Picked Up By Player!");
            PhotonView photonView = GetComponentInParent<PhotonView>();
            if (photonView != null)
            {
                // Call the RPC method on all clients
                photonView.RPC("HidePowerUp", RpcTarget.All);
            }
            else
            {
                Debug.LogError("PhotonView not found on the parent!");
            }
            
        }
    }

}

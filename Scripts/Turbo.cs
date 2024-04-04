using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TurboForce : MonoBehaviourPunCallbacks
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
                movementManager.movementSpeed = 8.0f;
                movementManager.fastSE.Play();
                StartCoroutine(ResetSpeed(movementManager, 5.0f));
            }
            Debug.Log("Turbo Picked Up By Player!");
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
    private IEnumerator ResetSpeed(MovementManager manager, float delay)
    {
        // Wait for the delay
        yield return new WaitForSeconds(delay);

        // Reset the speed to its original constant value
        manager.movementSpeed = 4.0f;
        Debug.Log("Speed Back to origin");
    }

}

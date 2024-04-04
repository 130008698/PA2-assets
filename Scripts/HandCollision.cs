using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ControllerSphereCollision : MonoBehaviour
{
    // Assign this via the inspector, assuming you want to change the player's material to this upon collision
    //public Material newMaterial;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is a player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit!");
            PhotonView photonView = other.GetComponentInParent<PhotonView>();
            if (photonView != null && !photonView.IsMine)
            {
                // Fetch the Renderer component of the player's body
                Debug.Log("Hitother!");
                photonView.RPC("ChangePlayerMaterial", RpcTarget.AllBuffered);

                PhotonView myPhotonView = GameObject.Find("NetworkPlayer(Clone)").GetComponent<PhotonView>();

                // Ensure you have a PhotonView on your player object.
                if (myPhotonView != null && myPhotonView.IsMine)
                {
                    // Call an RPC method to change the material to white on all clients.
                    myPhotonView.RPC("ChangeToWhiteMaterial", RpcTarget.AllBuffered);

                }
                
                
            }
        }
    }
}
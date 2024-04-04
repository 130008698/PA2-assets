using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tractor : MonoBehaviourPunCallbacks
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

            Debug.Log("Tractor Picked Up By Player!");
            PhotonView photonView = GetComponentInParent<PhotonView>();
            if (photonView != null)
            {
                // Call the RPC method on all clients
                photonView.RPC("HidePowerUp", RpcTarget.All);
                PhotonView[] playerViews = FindObjectsOfType<PhotonView>();
                foreach (var view in playerViews) {
                if (view.CompareTag("PlayerOG") && !view.IsMine) {
                //if(view.CompareTag("Player") && !view.IsMine){
                    // The position sent is the repulsor's position (you might adjust based on your hierarchy)
                    UIManager UI = GameObject.Find("XR Origin (XR Rig)").GetComponentInChildren<UIManager>(true);
                    
                    if(UI != null){
                        
                        if(UI.isCatcher){
                            view.RPC("ApplyTractorEffect", RpcTarget.All, photonView.transform.position);
                            
                        }
                        else{
                            view.RPC("ApplyRepulsorEffect", RpcTarget.All, photonView.transform.position);
                            
                        }
                    }
                    else{
                        Debug.Log ("UI not found");
                    }
                
                }
            }
            }
            else
            {
                Debug.LogError("PhotonView not found on the parent!");
            }
            
        }
    }
}

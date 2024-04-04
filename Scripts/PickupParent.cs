using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TurboParent : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    [PunRPC]
    public void HidePowerUp() {
        // Logic to hide or disable the power-up.
        Renderer renderer = GetComponent<Renderer>();
        Collider collider = GetComponent<Collider>();

        if (renderer != null) {
            renderer.enabled = false;
        }
        if (collider != null){
            collider.enabled = false;
        }
    

    Renderer[] childRenderers = GetComponentsInChildren<Renderer>();
    Collider[] childColliders = GetComponentsInChildren<Collider>();

    foreach (Renderer rendererC in childRenderers) {
        rendererC.enabled = false;
    }

    foreach (Collider colliderC in childColliders) {
        colliderC.enabled = false;
    }
    }
}

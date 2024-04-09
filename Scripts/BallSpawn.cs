using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit; // Import XR Interaction Toolkit namespace

public class BallSpawner : MonoBehaviourPunCallbacks
{
    public GameObject ball; // Assign this in the inspector
    //public XRGrabInteractable grabInteractable; // Assign this in the inspector
    private Transform course; // Assign your planet transform here
    public float spawnHeightAboveCourse = 1f; // Adjust based on your needs
    private GameObject spawnedBall;
    public bool hasSpawnedBall = false;


    public void HandleGrab()
    {
        if (!hasSpawnedBall)
        {
            SpawnBall();
            hasSpawnedBall = true;
        }
    }

    public void SpawnBall()
    {

        Vector3 spawnPositionBase = transform.position + Vector3.up * 1f; // 1 meter in front of the interactor
        Quaternion spawnRotation = Quaternion.identity; // Adjust as needed

        Vector3 spawnPosition = spawnPositionBase;
        spawnedBall = PhotonNetwork.Instantiate(ball.name, spawnPosition, spawnRotation);

    }

    public void DestroyBall()
    {
        if(spawnedBall != null)
        {
            PhotonNetwork.Destroy(spawnedBall);
        }
    }
}
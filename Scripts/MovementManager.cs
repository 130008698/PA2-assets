using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using System.Linq;

public class MovementManager : MonoBehaviourPunCallbacks
{
    private PhotonView myView;
    private GameObject myChild;
    private Transform planetCenter; // Assign this in the editor
    //public float gravityStrength = 3.2f;

    private float xInput;
    private float yInput;
    public float movementSpeed = 112.0f;

    private InputData inputData;
    //[SerializeField] private GameObject myObjectToMove;
    private Rigidbody myRB;
    private Transform myXRRig;
    private Transform myCam;
    private Transform cameraOffset;

    public float jumpForce = 100f;  // Adjustable force for the jump
    private bool isGrounded = false;

    public int tele = 0;
    private Vector3 moveDir;


    private AudioSource repulsorSE;
    private AudioSource teleportSE;
    public AudioSource fastSE;
    public AudioSource jumpSE;

    private Transform[] banners;
    private Transform gazedAtBanner = null;
    public GameObject CurrentPutter;
    // Start is called before the first frame update
    void Start()
    {
        myView = GetComponent<PhotonView>();
        repulsorSE = GetComponents<AudioSource>()[0];
        teleportSE = GetComponents<AudioSource>()[1];
        fastSE = GetComponents<AudioSource>()[2];
        jumpSE = GetComponents<AudioSource>()[3];
        myChild = transform.GetChild(0).gameObject;
        myRB =  myChild.GetComponent<Rigidbody>();

        //planetCenter = GameObject.Find("forest").transform;
        if(cameraOffset == null)
        {
            cameraOffset = GameObject.Find("XR Origin (XR Rig)").transform;
            if (cameraOffset == null)
            {
                Debug.LogError("no camera offset");
                return;
            }
        }
        GameObject myXrOrigin = GameObject.Find("XR Origin (XR Rig)");
        myXRRig = myXrOrigin.transform;
        myCam = GameObject.FindWithTag("MainCamera").transform;
        inputData = myXrOrigin.GetComponent<InputData>();
        
        GameObject[] bannerObjects = GameObject.FindGameObjectsWithTag("Banner");
        banners = bannerObjects.Select(obj => obj.transform).ToArray();
        

        photonView.RPC("CreatePutterNetwork", RpcTarget.All);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(myView.IsMine)
        {

            myXRRig.position = myRB.transform.position + Vector3.up * 0.8f;

            if(inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement))
            {
                xInput = movement.x;
                yInput = movement.y;
                
                moveDir = new Vector3(xInput, 0, yInput).normalized;
            }

            // if(Input.GetKeyDown(KeyCode.Space))
            // {
            //     TeleportPlayer();
            //     Debug.Log("Space key was pressed.");
            // }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(isGrounded){
                Jump();
                }
                else{
                    Debug.Log("123");
                }
            }
        }
    }

    private void FixedUpdate()
    {

            //PlanetGravity();
        if(myView.IsMine)
        {
            Vector2 movement = default(Vector2);
            if (XRSettings.enabled)
            {
                // Assuming VR context, replace with your actual input method.
                movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            else
            {
                // Placeholder for non-VR input.
                movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }

            //AlignBodyToCamera();
            Vector3 forward = myCam.forward;
            Vector3 right = myCam.right;
            // Remove any vertical (y-axis) movement from the camera's forward and right vectors
            forward.y = 0;
            right.y = 0;

            // Normalize the vectors (important to prevent faster movement when moving diagonally)
            forward.Normalize();
            right.Normalize();

            moveDir = (forward * movement.y + right * movement.x).normalized;
            //moveDir = new Vector3(movement.x, 0, movement.y).normalized;
            myRB.MovePosition(myRB.position + myChild.transform.TransformDirection(moveDir) * movementSpeed * Time.deltaTime);

            
            CheckGaze();
            if (inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool jump)&& jump && isGrounded )
            {
                Jump();
            }
            if (inputData.rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool tele)&& tele && gazedAtBanner != null )
            {
                TeleportPlayer();
            }


        }

    }


    public void CreatePutter()
    {
        if (CurrentPutter != null)
        {
            CurrentPutter.GetComponent<BallSpawner>().DestroyBall();
            PhotonNetwork.Destroy(CurrentPutter);
        }

        // Instantiate at a specific location or based on player position
        CurrentPutter = PhotonNetwork.Instantiate("Putter", myRB.position, Quaternion.identity);
        CurrentPutter.GetComponent<BallSpawner>().HandleGrab();
    }
    public void CreatePutter2()
    {
        if (CurrentPutter != null)
        {
            PhotonNetwork.Destroy(CurrentPutter);
        }

        // Instantiate at a specific location or based on player position
        CurrentPutter = PhotonNetwork.Instantiate("Putter", myRB.position, Quaternion.identity);
        CurrentPutter.GetComponent<BallSpawner>().SpawnBall();
    }

    void CheckGaze()
    {
        Ray ray = new Ray(myCam.position, myCam.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            foreach (Transform banner in banners)
            {
                if (hit.transform == banner)
                {
                    gazedAtBanner = banner;
                    return;
                }
            }
        }

        gazedAtBanner = null; // Reset if no banner is gazed at
    }

    private void Jump()
    {
        // Apply an upward force to the Rigidbody to simulate jumping

        myRB.AddForce(myChild.transform.up.normalized * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Character is now in the air
    }

    void TeleportPlayer()
    {
        if(gazedAtBanner != null){
            var teleportPosition = gazedAtBanner.GetChild(0).gameObject.transform.position;
            gazedAtBanner = null;

            myRB.MovePosition(teleportPosition);
        }
        photonView.RPC("CreatePutterNetwork", RpcTarget.All);
    }

    [PunRPC]
    void CreatePutterNetwork()
    {
        // This RPC will be executed by all clients, but the putter creation
        // should only occur for the local player who initiated the teleport
        if (photonView.IsMine)
        {
            CreatePutter();
        }
    }

    // [PunRPC]
    // void TeleportPlayerNetwork(Vector3 position)
    // {
    //     // This will be executed by all clients in the room
    //     myRB.MovePosition(position);
    //     CreatePutter();
    // }

    public void SetGroundedState(bool grounded)
    {
        isGrounded = grounded;
    }

    [PunRPC]
    private void NetworkedJump(Vector3 position, Vector3 jumpForce)
    {
        // This method is called on all other clients to visualize the jump
        myRB.MovePosition(position);
        myRB.AddForce(jumpForce, ForceMode.Impulse);
    }



    // [PunRPC]
    // public void ChangePlayerMaterial()
    // {
    //     // Find the material by name (ensure the material is in a Resources folder)
    //     Vector3 randomPositionOnSphere = Random.onUnitSphere * (planetCenter.localScale.x * 30 * 0.5f + 1f);
    //     Vector3 spawnPosition = planetCenter.position + randomPositionOnSphere;
    //     myRB.MovePosition(spawnPosition);
    //     Material newMat = Resources.Load<Material>("M1");
        
    //     if (newMat != null)
    //     {
    //         Renderer playerRenderer = this.GetComponentInChildren<Renderer>();
    //         if (playerRenderer != null && playerRenderer.material != newMat)
    //         {
    //             playerRenderer.material = newMat;
    //             if (myView.IsMine)
    //             {
    //                 FindObjectOfType<UIManager>().SetAsCatcher();
    //             }
    //         }
    //     }
    // }

    // [PunRPC]
    // public void ChangeToWhiteMaterial()
    // {
    //     Material whiMat = Resources.Load<Material>("white");
    //     Renderer renderer = this.GetComponentInChildren<Renderer>();
    //     if (renderer != null)
    //     {
    //         renderer.material = whiMat;
    //         if (myView.IsMine)
    //         {
    //             FindObjectOfType<UIManager>().SetAsRunner();
    //         }
    //     }
    // }

    // [PunRPC]
    // public void ApplyRepulsorEffect(Vector3 repulsorPosition) {
    //     //Vector3 directionFromRepulsor = (transform.position - repulsorPosition).normalized;
    //     //repulsorForceDirection = directionFromRepulsor;
    //     repulsorForcePosition = repulsorPosition;
    //     isUnderRepulsorEffect = true;
    //     if (repulsorSE != null) {
    //         repulsorSE.Play();
    //     }
    //     // Stop the effect after a delay
    //     StartCoroutine(StopRepulsorEffectAfterDelay(repulsorEffectDuration));
    // }

    // IEnumerator StopRepulsorEffectAfterDelay(float delay) {
    //     yield return new WaitForSeconds(delay);
    //     isUnderRepulsorEffect = false;
    // }

    // [PunRPC]
    // public void ApplyTractorEffect(Vector3 repulsorPosition) {
    //     //Vector3 directionFromRepulsor = (transform.position - repulsorPosition).normalized;
    //     //repulsorForceDirection = -directionFromRepulsor;
    //     repulsorForcePosition = repulsorPosition;
    //     isUnderTractorEffect = true;
    //     if (repulsorSE != null) {
    //         repulsorSE.Play();
    //     }
    //     // Stop the effect after a delay
    //     StartCoroutine(StopTractorEffectAfterDelay(repulsorEffectDuration));
    // }

    // IEnumerator StopTractorEffectAfterDelay(float delay) {
    //     yield return new WaitForSeconds(delay);
    //     isUnderTractorEffect = false;
    // }
}

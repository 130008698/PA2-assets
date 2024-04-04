using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

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

    bool isUnderRepulsorEffect = false;
    bool isUnderTractorEffect = false;
    public float repulsorForceMagnitude;
    Vector3 repulsorForceDirection;
    Vector3 repulsorForcePosition;
    float repulsorEffectDuration = 5.0f;

    private AudioSource repulsorSE;
    private AudioSource teleportSE;
    public AudioSource fastSE;
    public AudioSource jumpSE;
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

    }

    // Update is called once per frame
    void Update()
    {
        if(myView.IsMine)
        {

            myXRRig.position = myChild.transform.position + Vector3.up * 1f;

            if(inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 movement))
            {
                xInput = movement.x;
                yInput = movement.y;
                
                moveDir = new Vector3(xInput, 0, yInput).normalized;
            }
            
        }
    }

    private void FixedUpdate()
    {

            //PlanetGravity();
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
            moveDir = new Vector3(movement.x, 0, movement.y).normalized;
            myRB.MovePosition(myRB.position + myChild.transform.TransformDirection(moveDir) * movementSpeed * Time.deltaTime);
            //JoyStickMovement(movement.x, movement.y);
            
            // if (isUnderRepulsorEffect) {
            //     repulsorForceDirection = (myChild.transform.position - repulsorForcePosition);
            //     if(myRB.velocity.magnitude < 20){
            //     myRB.AddForce(repulsorForceDirection * repulsorForceMagnitude, ForceMode.Force);
            //     }
            // }

            // if (isUnderTractorEffect){
            //     repulsorForceDirection = -(myChild.transform.position - repulsorForcePosition);
            //     if(myRB.velocity.magnitude < 20){
            //     myRB.AddForce(repulsorForceDirection * repulsorForceMagnitude, ForceMode.Force);
            //     }
                
            // }
            if (inputData.leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool jump)&& jump && isGrounded )
            {
                Jump();
            }

        // if (myView.IsMine)
        // {
        //     if (cameraOffset != null)
        //     {
        //         // Align the camera offset's up direction opposite to gravity.
        //         // This maintains the camera's horizon alignment.
        //         Vector3 cameraUp = myChild.transform.up;
        //         Quaternion cameraTargetOrientation = Quaternion.FromToRotation(cameraOffset.up, cameraUp) * cameraOffset.rotation;
        //         cameraOffset.rotation = Quaternion.Slerp(cameraOffset.rotation, cameraTargetOrientation, Time.deltaTime * 5);
        //         cameraOffset.position = myChild.transform.position + cameraOffset.rotation * Vector3.up * 0.3f;
        //     }
        // }
        
    }



    // private void AlignBodyToCamera()
    // {
    //     // Extract the horizontal (y-axis) rotation from the camera.
    //     float yRotation = myCam.eulerAngles.y;

    //     // Combine current body rotation with camera y-axis rotation. Maintain body's local up vector alignment.
    //     myChild.transform.rotation = Quaternion.Euler(0, yRotation,0);
    // }

    private void Jump()
    {
        // Apply an upward force to the Rigidbody to simulate jumping
        if (myView.IsMine)
        {
            if(tele > 0){
                Vector3 forwardDirection = myXRRig.forward;
                Vector3 newPosition = myRB.position + forwardDirection * 50;
                myRB.MovePosition(newPosition);
                tele -= 1;
                if(teleportSE != null){
                    teleportSE.Play();
                }
                myView.RPC("NetworkedTeleport", RpcTarget.Others, newPosition);
            }
            else{
                myRB.AddForce(myChild.transform.up.normalized * jumpForce, ForceMode.Impulse);
                if(jumpSE != null){
                    jumpSE.Play();
                }
                myView.RPC("NetworkedJump", RpcTarget.Others, myRB.position, myChild.transform.up.normalized * jumpForce);
            }
            isGrounded = false; // Character is now in the air
        }
    }

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

    [PunRPC]
    private void NetworkedTeleport(Vector3 newPosition)
    {
        // This method is called on all other clients to visualize the teleportation
        myRB.MovePosition(newPosition);
    }

    [PunRPC]
    public void ChangePlayerMaterial()
    {
        // Find the material by name (ensure the material is in a Resources folder)
        Vector3 randomPositionOnSphere = Random.onUnitSphere * (planetCenter.localScale.x * 30 * 0.5f + 1f);
        Vector3 spawnPosition = planetCenter.position + randomPositionOnSphere;
        myRB.MovePosition(spawnPosition);
        Material newMat = Resources.Load<Material>("M1");
        
        if (newMat != null)
        {
            Renderer playerRenderer = this.GetComponentInChildren<Renderer>();
            if (playerRenderer != null && playerRenderer.material != newMat)
            {
                playerRenderer.material = newMat;
                if (myView.IsMine)
                {
                    FindObjectOfType<UIManager>().SetAsCatcher();
                }
            }
        }
    }

    [PunRPC]
    public void ChangeToWhiteMaterial()
    {
        Material whiMat = Resources.Load<Material>("white");
        Renderer renderer = this.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = whiMat;
            if (myView.IsMine)
            {
                FindObjectOfType<UIManager>().SetAsRunner();
            }
        }
    }

    [PunRPC]
    public void ApplyRepulsorEffect(Vector3 repulsorPosition) {
        //Vector3 directionFromRepulsor = (transform.position - repulsorPosition).normalized;
        //repulsorForceDirection = directionFromRepulsor;
        repulsorForcePosition = repulsorPosition;
        isUnderRepulsorEffect = true;
        if (repulsorSE != null) {
            repulsorSE.Play();
        }
        // Stop the effect after a delay
        StartCoroutine(StopRepulsorEffectAfterDelay(repulsorEffectDuration));
    }

    IEnumerator StopRepulsorEffectAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        isUnderRepulsorEffect = false;
    }

    [PunRPC]
    public void ApplyTractorEffect(Vector3 repulsorPosition) {
        //Vector3 directionFromRepulsor = (transform.position - repulsorPosition).normalized;
        //repulsorForceDirection = -directionFromRepulsor;
        repulsorForcePosition = repulsorPosition;
        isUnderTractorEffect = true;
        if (repulsorSE != null) {
            repulsorSE.Play();
        }
        // Stop the effect after a delay
        StartCoroutine(StopTractorEffectAfterDelay(repulsorEffectDuration));
    }

    IEnumerator StopTractorEffectAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        isUnderTractorEffect = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Example.Scened;

public class PlayerController : NetworkBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public bool gUp = false;    //key G is down

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    [SerializeField]
    private float cameraYOffset = 0.4f;
    private Transform playerCamera;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServerStarted)
        {
            PlayerManager.instance.players.Add(gameObject.GetInstanceID(), new PlayerManager.Player() { health = 100, playerObject = gameObject, connection = GetComponent<NetworkObject>().Owner });
        }
        print("player started");
        if (base.IsOwner)
        {
            print("player is owner");
            playerCamera = gameObject.transform.GetChild(2);    //camera
        }
        else
        {
            print("player not owner");
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Start()
    {
        print("player spawned");

        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //toggle menu
        if (Input.GetKey("g"))
        {
            if (canMove && gUp)
            {
                print("unlocked");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                canMove = false;
                gUp = false;
            }
            else if (!canMove && gUp)
            {
                print("locked");
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                canMove = true;
                gUp = false;
            }
        }
        else
        {
            gUp = true;
        }

        bool isRunning = false;

        // Press Left Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // We are grounded, so recalculate move direction based on axis
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        Physics.SyncTransforms();
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            //camDirection both vertical and horizontal, player direction only change horizontal
            Quaternion camDirection = Quaternion.Euler(rotationX, 0, 0);
            camDirection *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            playerCamera.transform.localRotation = camDirection;
        }
    }
}

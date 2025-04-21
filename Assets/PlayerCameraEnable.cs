using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerCameraEnable : NetworkBehaviour
{
    public GameObject cam;
    public GameObject playerBody;
    public GameObject playerCube;
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)   //enable camera on client player, disable mesh and colliders
        {
            print("enabling camera for owner");
            cam.SetActive(true);
            playerBody.SetActive(false);
            playerCube.SetActive(false);
        }
    }
}
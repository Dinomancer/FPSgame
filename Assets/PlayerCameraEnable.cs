using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerCameraEnable : NetworkBehaviour
{
    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)   //enable camera on client player, disable mesh and colliders
        {
            GameObject cam = GameObject.Find("Camera");
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            GameObject body = GameObject.Find("Capsule");
            body.GetComponent<CapsuleCollider>().enabled = false;
            body.GetComponent<MeshRenderer>().enabled = false;
            GameObject head = GameObject.Find("Cube");
            head.GetComponent<BoxCollider>().enabled = false;
            head.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

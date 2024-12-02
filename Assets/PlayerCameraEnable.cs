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
            Transform cam = gameObject.transform.GetChild(2);       //camera
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
            Transform body = gameObject.transform.GetChild(0);     //player capsule
            body.GetComponent<CapsuleCollider>().enabled = false;
            body.GetComponent<MeshRenderer>().enabled = false;
            Transform head = gameObject.transform.GetChild(1);     //player cube
            head.GetComponent<BoxCollider>().enabled = false;
            head.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

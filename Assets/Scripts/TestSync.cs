using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class TestSync : NetworkBehaviour
{
    // Start is called before the first frame update
    private readonly SyncVar<float> health = new SyncVar<float>(100);

    // Update is called once per frame
    [ServerRpc]
    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            health.Value = health.Value - 1;
            print("health changed to " + health.Value);
        }

    }
}

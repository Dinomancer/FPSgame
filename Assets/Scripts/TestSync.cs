using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class TestSync : NetworkBehaviour
{
    // Start is called before the first frame update
    public readonly SyncVar<float> health = new SyncVar<float>(100);

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            setHealth(health.Value - 1);
            print("health changed to " + health.Value);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void setHealth(float health)
    {
        this.health.Value = health;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSync : NetworkBehaviour
{
    // Start is called before the first frame update
    private readonly SyncVar<float> health = new SyncVar<float>(100);

    // Update is called once per frame
    private void Update()
    {
        if (!base.IsOwner)
        {
            print("not owner");
            return;

        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            health.value = health.value - 1;
            print("health changed to " + health.value);
        }

    }
}

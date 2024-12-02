using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyManager : NetworkBehaviour
{
    public string color;
    public readonly SyncVar<int> health = new SyncVar<int>(10);
    // Start is called before the first frame update

    [ServerRpc(RequireOwnership = false)]
    public void DamageEnemy(int damage, string color)
    {
        if (color == this.color)
        {
            print("Hit with color " + color + " health is " + health.Value.ToString());
            health.Value -= damage;
            if (health.Value < 0)
            {
                Despawn();
            }
        }
        else
        {
            print("color mismatch");
        }
    }

    [ObserversRpc]
    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}

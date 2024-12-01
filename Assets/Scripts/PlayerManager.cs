using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Object.Synchronizing;

public class PlayerManager : NetworkBehaviour
{
    public readonly SyncVar<int> health = new SyncVar<int>(100);
    public readonly SyncVar<int> damage = new SyncVar<int>(5);


    public static PlayerManager instance;
    private void Awake()
    {
        instance = this;
    }

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [ServerRpc(RequireOwnership = false)]
    public void DamagePlayer(GameObject player)
    {
        if (!base.IsServerStarted)
            return;
        if (player == null)
        {
            print("player is null");
            return;
        }
        print(player.GetComponent<PlayerManager>());
        player.GetComponent<PlayerManager>().health.Value -= damage.Value;
        print("Player health is " + player.GetComponent<PlayerManager>().health.Value);

        if (player.GetComponent<PlayerManager>().health.Value <= 0)
        {
            PlayerKilled(player);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerKilled(GameObject player)
    {
        print("Player was killed");
        //players[playerID].deaths++;
        player.GetComponent<PlayerManager>().health.Value = 100;
        //players[attackerID].kills++;

        //RespawnPlayer(players[playerID].connection, players[playerID].playerObject, Random.Range(0, spawnPoints.Count));
    }

    [TargetRpc]
    void RespawnPlayer(NetworkConnection conn, GameObject player, int spawn)
    {
        player.transform.position = spawnPoints[spawn].position;
    }

    public class Player
    {
        public int health = 100;
        public GameObject playerObject;
        public NetworkConnection connection;
        public int kills = 0;
        public int deaths = 0;
    }
}

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            print(health.Value.ToString());
        }
    }

    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [ServerRpc(RequireOwnership = false)]
    public void DamagePlayer(int damage)
    {
        if (!base.IsServerStarted)
            return;
        GetComponent<PlayerManager>().health.Value -= damage;
        print("Player health is " + GetComponent<PlayerManager>().health.Value);

        if (GetComponent<PlayerManager>().health.Value <= 0)
        {
            PlayerKilled();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerKilled()
    {
        print("Player was killed");
        //players[playerID].deaths++;
        GetComponent<PlayerManager>().health.Value = 100;
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

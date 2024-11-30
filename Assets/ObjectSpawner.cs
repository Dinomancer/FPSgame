using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public GameObject objToSpawn;
    //public GameObject spawnpoint;
    private bool isSpawned = false;
    void Start()
    {
        //UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName("SampleScene"));
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.H) && isSpawned == false){
            spawnObject(objToSpawn, new Vector3(0,0,0)); //spawnpoint.GetComponent<Transform>().position
            isSpawned = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void spawnObject(GameObject obj, Vector3 pos)
    {
        GameObject spawned = Instantiate(obj, pos, Quaternion.identity);
        ServerManager.Spawn(spawned, base.Owner);
        spawnObjectObserver(spawned, this);
    }

    [ObserversRpc]
    void spawnObjectObserver(GameObject spawned, ObjectSpawner script)
    {
        script.objToSpawn = spawned;

    }
}

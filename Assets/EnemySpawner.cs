using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet;
using FishNet.Managing;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject redEnemy;
    public GameObject blueEnemy;
    public GameObject greenEnemy;
    public GameObject yellowEnemy;
    public GameObject magentaEnemy;
    public GameObject cyanEnemy;
    public GameObject whiteEnemy;

    public GameObject gameBase;

    private bool canSpawn = true;

    private GameObject spawnedEnemy;

    // Update is called once per frame
    void Update()
    {


    }

    [ServerRpc(RequireOwnership = false)]
    public void spawnEnemyServer(string enemyName)
    {
        //spawn enemy facing base
        GameObject enemyToSpawn = redEnemy;
        if (enemyName == "redEnemy")
        {
            enemyToSpawn = redEnemy;
        }
        if (enemyName == "blueEnemy")
        {
            enemyToSpawn = blueEnemy;
        }
        if (enemyName == "greenEnemy")
        {
            enemyToSpawn = greenEnemy;
        }
        if (enemyName == "yellowEnemy")
        {
            enemyToSpawn = yellowEnemy;
        }
        if (enemyName == "cyanEnemy")
        {
            enemyToSpawn = cyanEnemy;
        }
        if (enemyName == "magentaEnemy")
        {
            enemyToSpawn = magentaEnemy;
        }
        if (enemyName == "whiteEnemy")
        {
            enemyToSpawn = whiteEnemy;
        }
        Vector3 direction = this.transform.position - gameBase.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        GameObject spawnedEnemy = Instantiate(enemyToSpawn, this.transform.position, targetRotation);
        spawnedEnemy.GetComponent<EnemyManager>().gameBase = gameBase;
        InstanceFinder.ServerManager.Spawn(spawnedEnemy);
        //WaitandActivate(0.1f, spawnedEnemy);
    }

    IEnumerator WaitandActivate(float time, GameObject objToActivate)
    {
        // Wait for 1 second without freezing the game
        yield return new WaitForSeconds(time);

        // This code runs after the 1 second delay
        print("1 second has passed! Doing something now.");
        // Your logic here
        objToActivate.SetActive(true);
    }
}

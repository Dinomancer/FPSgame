using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public GameObject[] spawnpoints;
    private int state = 0;//waiting to start wave

    // Update is called once per frame
    void Update()
    {
        if (state == 0)  //waiting
        {
            //press 5 to start wave
            if (Input.GetKey(KeyCode.Alpha5)  )
            {
                state = 1;
            }
        }else if (state == 1)    //wave 1
        {
            state = 2;
            spawnpoints[2].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
            spawnpoints[10].GetComponent<EnemySpawner>().spawnEnemyServer("greenEnemy");
            spawnpoints[14].GetComponent<EnemySpawner>().spawnEnemyServer("blueEnemy");
            StartCoroutine(SpawnWave2(6f));
        }
    }

    IEnumerator SpawnWave2(float time)
    {
        yield return new WaitForSeconds(time);
        spawnpoints[0].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
        spawnpoints[6].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
        spawnpoints[24].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
        spawnpoints[18].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
        spawnpoints[4].GetComponent<EnemySpawner>().spawnEnemyServer("blueEnemy");
        spawnpoints[8].GetComponent<EnemySpawner>().spawnEnemyServer("blueEnemy");
        spawnpoints[16].GetComponent<EnemySpawner>().spawnEnemyServer("blueEnemy");
        spawnpoints[20].GetComponent<EnemySpawner>().spawnEnemyServer("blueEnemy");
        spawnpoints[12].GetComponent<EnemySpawner>().spawnEnemyServer("magentaEnemy");


    }
}

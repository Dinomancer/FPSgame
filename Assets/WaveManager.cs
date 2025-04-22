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
            spawnpoints[0].GetComponent<EnemySpawner>().spawnEnemyServer("redEnemy");
            state = 2;
        }
    }
}

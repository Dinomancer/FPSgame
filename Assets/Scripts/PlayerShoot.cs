using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class PlayerShoot : NetworkBehaviour
{
    //[SerializeField] int damage = 5;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] LayerMask GameHittable;



    bool canShoot = true;
    WaitForSeconds shootWait;
    public string color = "red";   //color can be 1 red, 2 green, 3 blue

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            GetComponent<PlayerShoot>().enabled = false;

        shootWait = new WaitForSeconds(fireRate);

        
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        //

        if (Input.GetKey(shootKey) && canShoot)
            Shoot();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            switchColor("red");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            switchColor("green");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            switchColor("blue");

    }

    void switchColor(string color)
    {
        print("switched to color " + color);
        this.color = color;

        // 通过UIManager更新UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateColorDisplay(color);
        }

    }
    void Shoot()
    {
        print("Player shot");
        GameObject cam = GameObject.Find("Camera");
        Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), Color.green, 60);
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, GameHittable))
        {
            if(hit.transform.tag == "Player")
            {
                print("Hit player");
                HitPlayer(hit.transform.parent.GetComponent<PlayerManager>(), GetComponent<PlayerManager>().damage.Value, color);
            }else if (hit.transform.tag == "Enemy")
            {
                print("Hit enemy");
                HitEnemy(hit.transform.GetComponent<EnemyManager>(), GetComponent<PlayerManager>().damage.Value, color);
            }
        }

        StartCoroutine(CanShootUpdater());
    }

    [ServerRpc(RequireOwnership = false)]
    void HitPlayer(PlayerManager player, int damage, string color)
    {
        player.DamagePlayer(damage, color);
    }

    [ServerRpc(RequireOwnership = false)]
    void HitEnemy(EnemyManager enemy, int damage, string color)
    {
        enemy.DamageEnemy(damage, color);
    }

    IEnumerator CanShootUpdater()
    {
        canShoot = false;

        yield return shootWait;

        canShoot = true;
    }
}

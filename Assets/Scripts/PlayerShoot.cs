using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerShoot : NetworkBehaviour
{
    //[SerializeField] int damage = 5;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] LayerMask playerLayer;

    bool canShoot = true;
    WaitForSeconds shootWait;
    public string color = "red";   //color can be 1 red, 2 green, 3 blue

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            return;

        shootWait = new WaitForSeconds(fireRate);
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        //

        if (Input.GetKey(shootKey) && canShoot)
            Shoot();
        if (Input.GetKey(KeyCode.Alpha1))
            switchColor("red");
        if (Input.GetKey(KeyCode.Alpha1))
            switchColor("green");
        if (Input.GetKey(KeyCode.Alpha1))
            switchColor("blue");

    }

    void switchColor(string color)
    {
        print("switched to color" + color);
        this.color = color;
    }
    void Shoot()
    {
        print("Player shot");
        GameObject cam = GameObject.Find("Camera");
        Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), Color.green, 60);
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity, playerLayer))
        {
            print("Hit player");
            HitPlayer(hit.transform.parent.GetComponent<PlayerManager>(), GetComponent<PlayerManager>().damage.Value, color);
        }

        StartCoroutine(CanShootUpdater());
    }

    [ServerRpc(RequireOwnership = false)]
    void HitPlayer(PlayerManager player, int damage, string color)
    {
        print(player);
        player.DamagePlayer(damage, color);
    }

    IEnumerator CanShootUpdater()
    {
        canShoot = false;

        yield return shootWait;

        canShoot = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using MonoFN.Cecil;

public class PlayerShoot : NetworkBehaviour
{
    bool canShoot = true;

    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] int damage = 5;

    WaitForSeconds shootWait;
    public override void OnStartClient()
    {
        base.OnStartClient();
        shootWait = new WaitForSeconds(fireRate);

    }

    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }

        if (Input.GetKey(shootKey) && canShoot)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask.NameToLayer("GameHittable")))
        {

        }
        print("Player shot");
        StartCoroutine(CanShootUpdater());
    }

    IEnumerator CanShootUpdater()
    {
        canShoot = false;
        yield return shootWait;
        canShoot = true;
    }
}

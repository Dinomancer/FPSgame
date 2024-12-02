using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;


public class PlayerShoot : NetworkBehaviour
{
    //[SerializeField] int damage = 5;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] LayerMask GameHittable;
    [SerializeField] private MeshRenderer[] playerMeshRenderers;

    // 使用 SyncVar<string> 替代旧的 SyncVarAttribute
    private readonly SyncVar<string> currentColor = new SyncVar<string>("red");

    // 定义颜色映射
    private readonly Dictionary<string, Color> colorMap = new Dictionary<string, Color>()
    {
        { "red", new Color(0xE7/255f, 0x52/255f, 0x62/255f) },
        { "green", new Color(0x4B/255f, 0xD9/255f, 0x54/255f) },
        { "blue", new Color(0x4D/255f, 0x5D/255f, 0xDB/255f) }
    };

    bool canShoot = true;
    WaitForSeconds shootWait;
    //public string color = "red";   //color can be 1 red, 2 green, 3 blue
    public string color { get { return currentColor.Value; } }


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner)
            GetComponent<PlayerShoot>().enabled = false;
        

        shootWait = new WaitForSeconds(fireRate);

        // 设置 SyncVar 的回调
        currentColor.OnChange += OnColorChanged;

        // 初始化时设置颜色
        if (playerMeshRenderers != null)
        {
            UpdatePlayerColor(currentColor.Value);
        }

    }

    private void OnDestroy()
    {
        currentColor.OnChange -= OnColorChanged;
    }

    private void Start()
    {
        if (playerMeshRenderers == null || playerMeshRenderers.Length == 0)
        {
            playerMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        }
    }

    private void Update()
    {
        if (!base.IsOwner)
            return;

        //

        if (Input.GetKey(shootKey) && canShoot)
            Shoot();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchColorServerRpc("red");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchColorServerRpc("green");
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchColorServerRpc("blue");

    }

    [ServerRpc]
    private void SwitchColorServerRpc(string newColor)
    {
        currentColor.Value = newColor;
    }

    // 修改回调签名以匹配 SyncVar<T> 的 OnChange 委托
    private void OnColorChanged(string oldValue, string newValue, bool asServer)
    {
        UpdatePlayerColor(newValue);

        if (UIManager.Instance != null && IsOwner)
        {
            UIManager.Instance.UpdateColorDisplay(newValue);
        }
    }

    private void UpdatePlayerColor(string colorName)
    {
        Debug.Log($"Attempting to update color to: {colorName}");

        if (playerMeshRenderers == null || playerMeshRenderers.Length == 0)
        {
            Debug.LogError("No MeshRenderers found!");
            return;
        }

        if (!colorMap.ContainsKey(colorName))
        {
            Debug.LogError($"Color {colorName} not found in colorMap!");
            return;
        }

        Color newColor = colorMap[colorName];
        foreach (var renderer in playerMeshRenderers)
        {
            if (renderer != null)
            {
                renderer.material.color = newColor;
                Debug.Log($"Updated color for {renderer.gameObject.name} to: R:{newColor.r}, G:{newColor.g}, B:{newColor.b}");
            }
        }
    }


    /*
        void switchColor(string color)
    {
        print("switched to color " + color);
        this.color = color;

        // UIManager to update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateColorDisplay(color);
        }

    }
    */
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

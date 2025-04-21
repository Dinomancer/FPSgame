using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameKit.Dependencies.Utilities;
using Unity.VisualScripting;


public class PlayerShoot : NetworkBehaviour
{
    //[SerializeField] int damage = 5;
    [SerializeField] float fireRate = 0.3f;
    [SerializeField] KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] LayerMask GameHittable;
    [SerializeField] private MeshRenderer[] playerMeshRenderers;

    // 添加激光轨迹相关变量
    [SerializeField] public Transform shootPoint; // 射击点
    [SerializeField] public GameObject laserTrailRed; // 激光轨迹预制体
    [SerializeField] public GameObject laserTrailGreen;
    [SerializeField] public GameObject laserTrailBlue;
    [SerializeField] float laserDuration = 0.2f; // 激光持续时间

    // ʹ�� SyncVar<string> ����ɵ� SyncVarAttribute
    private readonly SyncVar<string> currentColor = new SyncVar<string>("red");

    public Material red;
    public Material green;
    public Material blue;

    public Object playerBody;
    public GameObject playerArmCube;

    // ������ɫӳ��
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

        // ���� SyncVar �Ļص�
        currentColor.OnChange += OnColorChanged;

        // ��ʼ��ʱ������ɫ
        if (playerMeshRenderers != null)
        {
            UpdatePlayerColor(currentColor.Value);
        }

        // 检查射击点
        if (shootPoint == null)
        {
            Debug.LogWarning("Shoot point not assigned. Using player position instead.");
            shootPoint = transform;
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

    [ServerRpc(RequireOwnership = false)]
    private void SwitchColorServerRpc(string newColor)
    {
        currentColor.Value = newColor;
        //change color for arm and body
        if (newColor == "red")
        {
            playerArmCube.GetComponent<MeshRenderer>().material = red;
            playerBody.GetComponent<MeshRenderer>().material = red;
        }else if (newColor == "blue")
        {
            playerArmCube.GetComponent<MeshRenderer>().material = blue;
            playerBody.GetComponent<MeshRenderer>().material = blue;
        }
        else if (newColor == "green")
        {
            playerArmCube.GetComponent<MeshRenderer>().material = green;
            playerBody.GetComponent<MeshRenderer>().material = green;
        }
    }

    // �޸Ļص�ǩ����ƥ�� SyncVar<T> �� OnChange ί��
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

        // 获取射击起点和方向
        Vector3 shootPosition = shootPoint.position;        // : cam.transform.position
        Vector3 shootDirection = cam.transform.TransformDirection(Vector3.forward);

        // 创建激光轨迹
        CreateLaserTrail(cam.transform.position, cam.transform.TransformDirection(Vector3.forward));



        //Debug.DrawRay(cam.transform.position, shootDirection, Color.green, 60);

        //Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), Color.green, 60);
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
            //render 
        }

        StartCoroutine(CanShootUpdater());
    }

    // 添加创建激光轨迹的方法
    void CreateLaserTrail(Vector3 startPos, Vector3 direction)
    {
        if (!IsOwner) return; // 只有本地玩家才能创建视觉效果

        RaycastHit hit;
        Vector3 endPos;

        // 确定激光轨迹的终点
        if (Physics.Raycast(startPos, direction, out hit, Mathf.Infinity, GameHittable))
        {
            endPos = hit.point;
        }
        else
        {
            // 如果没有击中任何物体，设置一个最大距离
            endPos = startPos + direction * 100f;
        }

        // 在本地创建激光轨迹
        CreateLaserTrailServerRpc(shootPoint.position, endPos, currentColor.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    void CreateLaserTrailServerRpc(Vector3 startPos, Vector3 endPos, string colorName)
    {
        // 在服务器上创建激光轨迹，然后通过RPC广播给所有客户端
        CreateLaserTrailClientRpc(startPos, endPos, colorName);
    }

    [ServerRpc(RequireOwnership = false)] // client?
    void CreateLaserTrailClientRpc(Vector3 startPos, Vector3 endPos, string colorName)
    {
        // 在所有客户端上创建激光轨迹
        GameObject laserTrail;
        if(colorName == "red")
        {
            laserTrail = Instantiate(laserTrailRed, Vector3.zero, Quaternion.identity);
        }
        else if (colorName == "blue")
        {
            laserTrail = Instantiate(laserTrailBlue, Vector3.zero, Quaternion.identity);
        }else if (colorName == "green")
        {
            laserTrail = Instantiate(laserTrailGreen, Vector3.zero, Quaternion.identity);
        }
        else
        {
            laserTrail = Instantiate(laserTrailRed, Vector3.zero, Quaternion.identity);
        }
        LineRenderer lineRenderer = laserTrail.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            // 设置线的位置
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            // 设置颜色
            if (colorMap.ContainsKey(colorName))
            {
                Color trailColor = colorMap[colorName];
                lineRenderer.startColor = trailColor;
                lineRenderer.endColor = trailColor;
            }

            // 一段时间后销毁激光轨迹
            Destroy(laserTrail, laserDuration);
        }
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

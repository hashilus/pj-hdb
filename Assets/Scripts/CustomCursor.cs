using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    public RectTransform cursorRect;
    public float sensitivity = 1.0f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public float shootForce = 500f;
    public float shootInterval = 0.2f;
    public Transform cameraTransform;

    [Header("Line Settings")]
    public bool enableLineRendering = true;
    public float startLineWidth = 0.01f;
    public float maxLineWidth = 0.05f;
    public float lineGrowthSpeed = 0.02f;
    public Material lineMaterial;

    [Header("Effects")]
    public GameObject hitParticlePrefab;
    public Transform gunBarrel;

    private Vector2 currentPos;
    private float shootTimer;
    private GameObject lastProjectile = null;
    private float lastShotTime = 0f;

    public ParticleSystem gunsmoke;
    public ParticleSystem gas;
    public ParticleSystem water;

    [Header("Tracker")]
    [SerializeField]
    Transform trackerTransform;

    [SerializeField]
    bool isSecondPlayer;

    [SerializeField]
    AirController airController;

    [SerializeField]
    ReticleController reticle;
    
    void Start()
    {
        Cursor.visible = false;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        currentPos = screenCenter;
        cursorRect.position = screenCenter;

        if (Settings.System.IsUseTracker)
        {
            var trackerBarrelXAngle = 90f;

            gunBarrel.SetParent(trackerTransform);
            gunBarrel.localPosition = new Vector3(0, 0, 0.5f);
            gunBarrel.localRotation = Quaternion.Euler(-29.003f + trackerBarrelXAngle, -0.196f, 0);

            cursorRect.gameObject.SetActive(false);

            // HACCの初期化
            airController.Init(
                isSecondPlayer ? Settings.System.HACCAddressL : Settings.System.HACCAddressR, 
                isSecondPlayer ? Settings.System.HACCPortL : Settings.System.HACCPortR
                );
    
        }
        else if(isSecondPlayer)
        {
            // マウス時は2プレイヤーを無効
            gameObject.SetActive(false);            
        }
    }

    void Update()
    {
        if (Settings.System.IsUseTracker)
        {
            if (AirBlowPermission.CanBlow(isSecondPlayer ? AirBlowPermission.PlayerSelection.Player2 : AirBlowPermission.PlayerSelection.Player1))
            {
                TrackerShooting(isSecondPlayer ? "Fire2" : "Fire1");
            }
        }
        else
        {
            // マウスの移動  
            Vector2 delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            currentPos += delta * sensitivity;
            currentPos.x = Mathf.Clamp(currentPos.x, 0, Screen.width);
            currentPos.y = Mathf.Clamp(currentPos.y, 0, Screen.height);
            cursorRect.position = currentPos;

            // 射撃時に煙を出す
            shootTimer += Time.deltaTime;
            if (Input.GetMouseButton(0) && shootTimer >= shootInterval)
            {
                Shooting();
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                StopShooting();
            }
        }
    }

    void TrackerShooting(string buttonName)
    {
        if(Input.GetButtonDown(buttonName))
        {
            airController.StartBlow();
        }

        if(Input.GetButton(buttonName))
        {
            Shooting();
        }
                
        if(Input.GetButtonUp(buttonName))
        {
            airController.StopBlow();
            StopShooting();
        }
    }

    void Shooting()
    {
        ShootProjectile();
        shootTimer = 0f;

        var emission = gunsmoke.emission;
        emission.rateOverTime = 8f;
        emission = gas.emission;
        emission.rateOverTime = 10f;
        emission = water.emission;
        emission.rateOverTime = 60f;
    }

    void StopShooting()
    {
        var emission = gunsmoke.emission;
        emission.rateOverTime = 0f;
        emission = gas.emission;
        emission.rateOverTime = 0f;
        emission = water.emission;
        emission.rateOverTime = 0f;
    }
  
    void ShootProjectile()
    {        
        Vector3 origin = Settings.System.IsUseTracker 
            ? trackerTransform.position 
            : cameraTransform.position + cameraTransform.TransformDirection(Vector3.down * 0.3f);

        Ray ray = Settings.System.IsUseTracker
            ? new Ray(origin, trackerTransform.forward)
            : Camera.main.ScreenPointToRay(currentPos);

        // 銃身をRayの方向に向ける（回転はしない）
        if (gunBarrel != null)
        {
            gunBarrel.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
        }

        GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.identity);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(ray.direction.normalized * shootForce);
        }

        var destroyer = proj.GetComponent<ProjectileAutoDestroy>();
        if (destroyer != null)
        {
            destroyer.particlePrefab = hitParticlePrefab;
            destroyer.Reticle = reticle;
        }

        float timeSinceLastShot = Time.time - lastShotTime;
        if (enableLineRendering && lastProjectile != null && timeSinceLastShot <= 0.5f)
        {
            CreateLineBetween(lastProjectile, proj);
        }

        lastProjectile = proj;
        lastShotTime = Time.time;

        Destroy(proj, 2f);
    }

    void CreateLineBetween(GameObject from, GameObject to)
    {
        GameObject lineObj = new GameObject("LineBetweenProjectiles");
        lineObj.transform.SetParent(to.transform); // 親子になる（後で一緒に自動で削除）

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = startLineWidth;
        line.endWidth = startLineWidth;

        line.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));

        FollowLine follow = lineObj.AddComponent<FollowLine>();
        follow.Init(from.transform, to.transform, startLineWidth, maxLineWidth, lineGrowthSpeed);
    }
}


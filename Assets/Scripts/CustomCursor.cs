using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        Cursor.visible = false;
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        currentPos = screenCenter;
        cursorRect.position = screenCenter;
    }

    void Update()
    {
        // カーソル移動  
        Vector2 delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        currentPos += delta * sensitivity;
        currentPos.x = Mathf.Clamp(currentPos.x, 0, Screen.width);
        currentPos.y = Mathf.Clamp(currentPos.y, 0, Screen.height);
        cursorRect.position = currentPos;

        // 発射タイミング管理  
        shootTimer += Time.deltaTime;
        if (Input.GetMouseButton(0) && shootTimer >= shootInterval)
        {
            ShootProjectile();
            shootTimer = 0f;

            // 発射時に煙を出す  
            var emission = gunsmoke.emission;
            emission.rateOverTime = 8f;
            emission = gas.emission;
            emission.rateOverTime = 10f;
            emission = water.emission;
            emission.rateOverTime = 60f;
            water.Play();

        }
        else if (!Input.GetMouseButton(0))
        {
            // 発射をやめたら煙を止める  
            var emission = gunsmoke.emission;
            emission.rateOverTime = 0f;
            emission = gas.emission;
            emission.rateOverTime = 0f;
            emission = water.emission;
            emission.rateOverTime = 0f;
        }
    }
  
void ShootProjectile()
{
    Vector3 screenCursorPos = currentPos;
    Ray ray = Camera.main.ScreenPointToRay(screenCursorPos);
    Vector3 origin = cameraTransform.position + cameraTransform.TransformDirection(Vector3.down * 0.3f);

    // 砲身をRayの方向に向ける（回転ゼロにはしない）
    if (gunBarrel != null)
    {
        gunBarrel.rotation = Quaternion.LookRotation(ray.direction, cameraTransform.up);
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
        lineObj.transform.SetParent(to.transform); // 球体が親になる（消滅時に自動で削除）

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

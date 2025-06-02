using UnityEngine;

public class PhysicsSimulatedTrajectory : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 initialVelocity = new Vector3(5f, 10f, 0f);
    public int maxSteps = 10;
    public float timeStep = 10f;
    public float lineDuration = 3f;

    private GameObject currentBall;
    private Rigidbody currentRb;
    private Vector3 previousPos;

    void Start()
    {
        Physics.autoSimulation = false; // 手動シミュレーションモードに切り替え
        InitializeSimulation();
    }

    void InitializeSimulation()
    {
        // 既存のボールを削除
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        // 新しいボールを生成
        currentBall = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        currentRb = currentBall.GetComponent<Rigidbody>();
        currentRb.velocity = initialVelocity;
        previousPos = currentBall.transform.position;
    }

    void Update()
    {
        // 1フレームでシミュレーションを完了
        for (int i = 0; i < maxSteps; i++)
        {
            Physics.Simulate(timeStep); // 手動で物理を進める

            Vector3 currentPos = currentBall.transform.position;
            Debug.DrawLine(previousPos, currentPos, Color.red, lineDuration * 50f);

            // 衝突をRaycastで確認
            if (Physics.Raycast(previousPos, currentPos - previousPos, out RaycastHit hit, (currentPos - previousPos).magnitude))
            {
                Debug.DrawLine(previousPos, hit.point, Color.green, lineDuration);
                Debug.Log("衝突位置: " + hit.point);
                currentRb.isKinematic = true;
                InitializeSimulation(); // 新しいシミュレーションを開始
                return;
            }

            previousPos = currentPos;
        }

        currentRb.isKinematic = true;
        InitializeSimulation(); // 新しいシミュレーションを開始
    }

    void OnDestroy()
    {
        Physics.autoSimulation = true; // 元に戻す
    }
}
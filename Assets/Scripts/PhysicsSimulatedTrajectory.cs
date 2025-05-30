using UnityEngine;

public class PhysicsSimulatedTrajectory : MonoBehaviour
{
    public GameObject ballPrefab;
    public Vector3 initialVelocity = new Vector3(5f, 10f, 0f);
    public int maxSteps = 100;
    //public float timeStep = 0.02f;
    public float timeStep = 10f;
    public float lineDuration = 3f;

    void Start()
    {
        Physics.autoSimulation = false; // 手動シミュレーションモードに切り替え
        SimulateTrajectory();
        Physics.autoSimulation = true; // 元に戻す
    }

    void SimulateTrajectory()
    {
        // ボールを非表示で生成
        GameObject ball = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        // ball.GetComponent<Renderer>().enabled = false; // 可視化しない
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = initialVelocity;

        Vector3 previousPos = ball.transform.position;

        for (int i = 0; i < maxSteps; i++)
        {
            Physics.Simulate(timeStep); // 手動で物理を進める

            Vector3 currentPos = ball.transform.position;
            Debug.DrawLine(previousPos, currentPos, Color.red, lineDuration * 50f);

            // 衝突をRaycastで確認（または物理イベント監視でも可）
            if (Physics.Raycast(previousPos, currentPos - previousPos, out RaycastHit hit, (currentPos - previousPos).magnitude))
            {
                Debug.DrawLine(previousPos, hit.point, Color.green, lineDuration);
                Debug.Log("衝突位置: " + hit.point);
                break;
            }

            previousPos = currentPos;
        }

        rb.isKinematic = true;

//        Destroy(ball); // シミュレーション用のボールを削除
    }
}
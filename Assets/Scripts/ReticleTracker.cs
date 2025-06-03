using UnityEngine;

public class ReticleTracker : MonoBehaviour
{
    [SerializeField]
    Transform controllerTransform;

    [SerializeField]
    float predictionTime = 2f; // 予測時間（秒）

    [SerializeField]
    int predictionSteps = 20; // 予測の分割数

    [SerializeField]
    ReticleController reticleController;

    void Update()
    {
        // 発射体の初期位置と速度を設定
        Vector3 startPosition = controllerTransform.position;
        Vector3 initialVelocity = controllerTransform.forward * Settings.System.ReticleProjectileSpeed;

        // 予測軌道を計算
        Vector3 predictedPosition = PredictTrajectory(startPosition, initialVelocity);

        reticleController.ShowAt(predictedPosition + Settings.System.ReticleOffset);
    }

    Vector3 PredictTrajectory(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = initialVelocity;
        float timeStep = predictionTime / predictionSteps;

        // 予測時間分の軌道を計算
        for (int i = 0; i < predictionSteps; i++)
        {
            // 重力の影響を加える
            currentVelocity += Physics.gravity * timeStep;
            
            // 位置を更新
            currentPosition += currentVelocity * timeStep;

            // 地面や壁との衝突をチェック
            RaycastHit hit;
            if (Physics.Raycast(currentPosition - currentVelocity * timeStep, currentVelocity.normalized, out hit, currentVelocity.magnitude * timeStep))
            {
                return hit.point;
            }
        }

        return currentPosition;
    }

    // デバッグ用：予測軌道を可視化
    void OnDrawGizmos()
    {
        if (controllerTransform == null)
            return;

        Vector3 startPosition = controllerTransform.position;
        Vector3 initialVelocity = controllerTransform.forward * Settings.System.ReticleProjectileSpeed;
        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = initialVelocity;
        float timeStep = predictionTime / predictionSteps;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < predictionSteps; i++)
        {
            Vector3 previousPosition = currentPosition;
            currentVelocity += Physics.gravity * timeStep;
            currentPosition += currentVelocity * timeStep;
            Gizmos.DrawLine(previousPosition, currentPosition);
        }
    }
}
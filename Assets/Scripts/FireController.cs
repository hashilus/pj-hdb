using UnityEngine;
using System.Collections;

public class FireController : MonoBehaviour
{
    [Header("Fire Particle")]
    public ParticleSystem fireParticle;
    private ParticleSystem.EmissionModule fireEmission;

    [Header("Ember Particle")]
    public ParticleSystem emberParticle;
    private ParticleSystem.EmissionModule emberEmission;

    [Header("Life Settings")]
    public float initialLife = 5f;
    public float life = 5f;

    [Header("Scaling")]
    public Vector3 maxScale = Vector3.one;
    public Vector3 minScale = Vector3.zero;

    [Header("Light Intensity")]
    public Light fireLight;
    public float maxIntensity = 3f;
    public float minIntensity = 0f;

    [Header("Smoke Particle")]
    public ParticleSystem smokeParticle; // 事前にアタッチ
    public float smokeStopDelay = 2f;    // 消火後にEmission止めるまでの秒数
    public float destroyDelay = 5f;      // 完全消滅までの秒数

    [Header("Small Particle")]
    public ParticleSystem smallfireParticle;

    public Checkpoint checkpoint; // Inspectorでなくコードから設定

    private ParticleSystem.EmissionModule smokeEmission;

    private bool extinguished = false;

    public void AssignCheckpoint(Checkpoint cp)
    {
        checkpoint = cp;
    }

    private void Start()
    {
        maxScale = transform.localScale; // 初期スケールを最大スケールに設定
        life = initialLife;
        transform.localScale = maxScale;

        if (fireLight != null)
            fireLight.intensity = maxIntensity;

        if (smokeParticle != null)
        {
            smokeEmission = smokeParticle.emission;
            float smokeT = Mathf.InverseLerp(initialLife, 0f, life);
            float maxRate = 30f;
            smokeEmission.rateOverTime = Mathf.Lerp(0f, maxRate, smokeT);
        }

        if (fireParticle != null)
            fireEmission = fireParticle.emission;
        if (emberParticle != null)
            emberEmission = emberParticle.emission;

    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Water") && !extinguished)
        {
            ReduceLife(1f);
        }
    }

    void ReduceLife(float amount)
    {
        life -= amount;

        float t = Mathf.Clamp01(life / initialLife);

        // スケール・ライト調整（そのままでOK）
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);
        if (fireLight != null)
            fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // スモークEmission（火が消えるほど増える）
        if (smokeParticle != null)
        {
            float maxRate = 30f; // 最大煙
            float smokeT = Mathf.InverseLerp(initialLife, 0f, life); // 1 = 煙最大, 0 = 煙ゼロ
            smokeEmission.rateOverTime = Mathf.Lerp(0f, maxRate, smokeT);
        }
        if (emberParticle != null)
        {
            float maxEmberRate = 20f; // 適宜調整
            float emberT = Mathf.InverseLerp(initialLife, 0f, life);
            emberEmission.rateOverTime = Mathf.Lerp(0f, maxEmberRate, emberT);
        }
        if (life <= 0f)
        {
            Extinguish();
        }
    }
    public void ForceExtinguish()
    {
        if (extinguished) return;
        life = 0f;
        ReduceLife(0f); // life=0を処理させる
    }

    // lifeが0になった時
    void Extinguish()
    {
        if (extinguished) return;

        extinguished = true;
        Debug.Log("Extinguishing fire: " + gameObject.name);

        if (fireParticle != null)
        {
            fireParticle.gameObject.SetActive(false);
        }

        if (emberParticle != null)
        {
            emberParticle.gameObject.SetActive(false);
        }

        if (smallfireParticle != null)
        {
            smallfireParticle.transform.parent = null; // 親を外す
            smallfireParticle.gameObject.SetActive(true);
            Destroy(smallfireParticle.gameObject, 2f); // 2秒後に消す
        }

        if (checkpoint != null)
            checkpoint.NotifyFireExtinguished(this);

        //StartCoroutine(HandleSmokeFadeOut());
    }


    IEnumerator HandleSmokeFadeOut()
    {
        // wait before stopping emission
        yield return new WaitForSeconds(smokeStopDelay);

        if (smokeParticle != null)
        {
            smokeEmission.rateOverTime = 0f;
        }

        // wait again before destruction
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}

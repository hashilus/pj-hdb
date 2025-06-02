using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public System.Action<Checkpoint> onCleared;
    private bool isCleared = false;
    
    public float clearTimeLimit = 10f;
    private Coroutine forceClearCoroutine;


    [Header("���C�g������s�����H")]
    public bool controlLights = true;

    private List<FireController> fires = new List<FireController>();

    public float bonusTime = 10f; // ����CP���N���A������ǉ�����鎞��

    public TextMesh debugCountText; // �f�o�b�O�p�̕\��

    int firesCount = 0;

    void Awake()
    {
        fires.Clear();
        foreach (var fire in GetComponentsInChildren<FireController>())
        {
            fires.Add(fire);
            fire.AssignCheckpoint(this); // FireController ���Ɏ�����o�^
        }
        firesCount = fires.Count;

        SetLightActive(false);
    }

    public void NotifyFireExtinguished(FireController fc)
    {
        // �Ă΂�邽�тɃ`�F�b�N
        firesCount--;
        debugCountText.text = "�c��" + firesCount.ToString();
        CheckIfCleared();
    }


    public void ActivateCheckpoint()
    {
        debugCountText.text = "�c��" + fires.Count.ToString();
        SetLightActive(true);
        if (forceClearCoroutine != null)
            StopCoroutine(forceClearCoroutine);

        forceClearCoroutine = StartCoroutine(ForceClearAfterDelay(clearTimeLimit));
    }

    private IEnumerator ForceClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log($"{name}: ���Ԑ؂�ŋ������΂����s");

        foreach (var fire in fires)
        {
            if (fire.life > 0)
            {
                fire.ForceExtinguish(); // �� FireController���ŗp�Ӂi��q�j
            }
        }

        CheckIfCleared(); // �O�̂��߃`�F�b�N
    }

    private void CheckIfCleared()
    {
        if (isCleared) return;

        float totalLife = 0f;
        foreach (var fire in fires)
        {
            totalLife += Mathf.Max(0f, fire.life);
        }

        if (totalLife <= 0f)
        {
            isCleared = true;
            Debug.Log($"{name}: �SFire��Life��0�B�`�F�b�N�|�C���g�N���A�I");
            onCleared?.Invoke(this);
            SetLightActive(false);
            ClearCheckpoint(); // �N���A����
        }
    }

    public void SetLightActive(bool active)
    {
        if (!controlLights) return;

        foreach (Transform child in transform)
        {
            var light = child.GetComponentInChildren<Light>();
            if (light != null) light.enabled = active;
        }
    }

    void ClearCheckpoint()
    {
        TimeManager time = FindObjectOfType<TimeManager>();
        if (time != null)
        {
            time.StopCountdown();
            time.AddBonusTime(bonusTime);
        }

        FindObjectOfType<TimeExtendDisplay>().ShowExtend((int)bonusTime);

        // ���̃N���A����
    }


}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    public int checkPointNo; // �`�F�b�N�|�C���g�ԍ�

    public System.Action<Checkpoint> onCleared;
    private bool isCleared = false;

    public float clearTimeLimit = 10f;
    private Coroutine forceClearCoroutine;

    [Header("���C�g������s�����H")]
    public bool controlLights = true;

    private List<FireController> fires = new List<FireController>();

    public float bonusTime; // ����CP���N���A������ǉ�����鎞��

    public TextMesh debugCountText; // �f�o�b�O�p�̕\��

    void Awake()
    {
        fires.Clear();

        foreach (var fire in GetComponentsInChildren<FireController>(true)) // �� Active=false ���܂߂Ď擾
        {
            fires.Add(fire);
            fire.AssignCheckpoint(this); // FireController ���Ɏ�����o�^
        }

        SetLightActive(false);

        // �{�[�i�X�^�C���擾
        bonusTime = SettingsManager.Instance.settings.checkpointExtendTimes[checkPointNo - 1];
    }

    public void NotifyFireExtinguished(FireController fc)
    {
        debugCountText.text = "�c��" + GetAliveFireCount().ToString();
        CheckIfCleared();
    }

    public void ActivateCheckpoint()
    {
        debugCountText.text = "�c��" + GetAliveFireCount().ToString();
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
            if (fire != null && fire.life > 0f && fire.gameObject.activeInHierarchy)
            {
                fire.ForceExtinguish(); // �� FireController ���ŗp��
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
            if (fire != null && fire.gameObject.activeInHierarchy)
            {
                totalLife += Mathf.Max(0f, fire.life);
            }
        }

        if (totalLife <= 0f)
        {
            isCleared = true;
            Debug.Log($"{name}: �SFire��Life��0�B�`�F�b�N�|�C���g�N���A�I");
            onCleared?.Invoke(this);
            SetLightActive(false);
            ClearCheckpoint();
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
            //time.AddBonusTime(bonusTime);
        }

        FindObjectOfType<TimeExtendDisplay>().ShowExtend((int)bonusTime);

        // ���̃N���A����
    }

    // ���ݎc���Ă���΂̌���Ԃ��i�P���� childCount �͎g��Ȃ��I�j
    public int GetCurrentFireCount()
    {
        int count = 0;

        foreach (var fire in fires)
        {
            if (fire != null)
            {
                count++;
            }
        }

        return count;
    }

    // ���� Alive �� Fire �̐���Ԃ�
    public int GetAliveFireCount()
    {
        int count = 0;

        foreach (var fire in fires)
        {
            if (fire != null && fire.gameObject.activeInHierarchy && fire.life > 0f)
            {
                count++;
            }
        }

        return count;
    }
}

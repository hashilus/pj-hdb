using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    [Header("�c�^�C���\���p TextMesh")]
    public TextMesh timeText;

    [Header("�����N�p GameObjects")]
    public GameObject rankSObject;
    public GameObject rankAObject;
    public GameObject rankBObject;
    public GameObject rankCObject;

    private void OnEnable()
    {
        Debug.Log("ResultDisplay �� �N��");

        // �@ �c�^�C�����擾
        TimeManager timeManager = FindObjectOfType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("ResultDisplay: TimeManager ��������܂���I");
            return;
        }

        float remainingTime = timeManager.GetCurrentTime();
        Debug.Log($"ResultDisplay: �c�^�C�� = {remainingTime:F1} �b");

        // �A �c�^�C���� TextMesh �ɕ\��
        if (timeText != null)
        {
            timeText.text = $"{remainingTime:F1}";
        }

        // �B �����N臒l���擾
        var thresholds = SettingsManager.Instance.settings.resultThresholds;

        // �C �����N���� �� GameObject Active �؂�ւ�
        if (remainingTime >= thresholds.rankS)
        {
            SetRank("S");
        }
        else if (remainingTime >= thresholds.rankA)
        {
            SetRank("A");
        }
        else if (remainingTime >= thresholds.rankB)
        {
            SetRank("B");
        }
        else
        {
            SetRank("C");
        }
    }

    private void SetRank(string rank)
    {
        Debug.Log($"ResultDisplay: �����N = {rank}");

        // �S�����N������OFF
        if (rankSObject != null) rankSObject.SetActive(false);
        if (rankAObject != null) rankAObject.SetActive(false);
        if (rankBObject != null) rankBObject.SetActive(false);
        if (rankCObject != null) rankCObject.SetActive(false);

        // �Ώۃ����N����ON
        switch (rank)
        {
            case "S":
                if (rankSObject != null) rankSObject.SetActive(true);
                break;
            case "A":
                if (rankAObject != null) rankAObject.SetActive(true);
                break;
            case "B":
                if (rankBObject != null) rankBObject.SetActive(true);
                break;
            case "C":
                if (rankCObject != null) rankCObject.SetActive(true);
                break;
        }
    }
}

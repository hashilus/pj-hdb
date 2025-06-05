using UnityEngine;
using System.Collections;

public class TimeExtendDisplay : MonoBehaviour
{
    [Header("TIME EXTEND �֘A")]
    public GameObject extendGroup;     // TIME EXTEND��������܂ސe�I�u�W�F�N�g
    public TextMesh extendText;        // EXTEND���l (TextMesh)
    
    [Header("���ԊǗ�")]
    public TimeManager timeManager;    // TimeManager�X�N���v�g�ւ̎Q��

    [Header("���o�^�C�~���O")]
    public float addInterval = 0.1f;  // 1���Z���Ƃ̊Ԋu
    public float showDelay = 1f;       // �\����ҋ@
    public float hideDelay = 1f;       // ���Z�������\���܂ł̑ҋ@

    private Coroutine extendRoutine;

    void Start()
    {
        if (extendGroup != null)
        {
            extendGroup.SetActive(false);
        }
    }

    /// <summary>
    /// �w��b�������������Z���Ȃ���EXTEND�\�����s��
    /// </summary>
    public void ShowExtend(int amount)
    {
        if(amount == 0)
        {
            return;
        }

        if (extendRoutine != null)
            StopCoroutine(extendRoutine);

        extendRoutine = StartCoroutine(HandleTimeExtend(amount));
    }

    private IEnumerator HandleTimeExtend(int amount)
    {
        extendGroup.SetActive(true);
        extendText.text = amount.ToString();

        yield return new WaitForSeconds(showDelay);

        int remaining = amount;
        while (remaining > 0)
        {
            timeManager.AddBonusTime(1f);
            remaining--;
            extendText.text = remaining.ToString();

            yield return new WaitForSeconds(addInterval);
        }

        yield return new WaitForSeconds(hideDelay);
        extendGroup.SetActive(false);
    }
}

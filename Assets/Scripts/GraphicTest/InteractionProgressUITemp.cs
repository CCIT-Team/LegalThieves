using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionProgressUITemp : MonoBehaviour
{
    [SerializeField] private string middleOfProcessMessages = "���� �Ѽ� ��"; //UI �޼��� 
    [SerializeField] private string processingCompleteMessage = "���� �Ѽ� �Ϸ�!"; //UI ���� �޼��� 

    [SerializeField] private float duration = 10.0f; //UI ���� �ð� 
    [SerializeField] private float closingDuration = 1.5f; //UI ���� ���� �ð� 
    [SerializeField] private AnimationCurve textAnimation; //UI �޼��� �ִϸ��̼�
    [SerializeField] private AnimationCurve endAnimation; //UI �޼��� ���� �ִϸ��̼�
    [SerializeField] private float textBouncingMultiplier = 1f; //UI �޼��� �ִϸ��̼� Scale ���� �ڷ�ƾ

    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    private int textCommaCount;
    private float timeCount;
    private float textFontDefaultSize;

    private void Start()
    {
        progressBar = gameObject.GetComponentInChildren<Image>();
        progressText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        textFontDefaultSize = progressText.fontSize;
    }

    private IEnumerator OnProgressing() //UI ���� �ڷ�ƾ
    {
        progressBar.fillAmount = timeCount / duration;
        progressText.fontSize = textFontDefaultSize * textAnimation.Evaluate(timeCount % 2 * 0.5f) * textBouncingMultiplier;
        timeCount += Time.deltaTime;
        yield return null;

        if (timeCount / duration < 1f)
        {
            StartCoroutine(OnProgressing());
            yield return null;
        }
        else StartEndOfProgress();
        yield return null;
    }

        private IEnumerator OnProgressingText() //UI �޼��� ���� �ڷ�ƾ
    {
        if (textCommaCount == 3)
        {
            progressText.text = middleOfProcessMessages;
            textCommaCount = 0;
        }
        else
        {
            progressText.text += '.';
            textCommaCount++;
        }
        yield return new WaitForSeconds(0.5f);

        if (timeCount / duration < 1f)
        {
            StartCoroutine(OnProgressingText());
            yield return null;
        }
    }

    private IEnumerator OnEndOfProgress() //UI ���� �ڷ�ƾ
    {
        timeCount += Time.deltaTime;
        progressText.fontSize = endAnimation.Evaluate(timeCount / closingDuration) * textFontDefaultSize * textBouncingMultiplier;
        yield return null;

        if (timeCount / closingDuration < 1f) StartCoroutine(OnEndOfProgress());
        else this.gameObject.SetActive(false); // UI ��� ���� ����, UI �Ŵ����� ��ȣ�ۿ� �ε� UI ���� �޼��� ȣ��
    }

    private IEnumerator OnStart() //UI ���� �ڷ�ƾ
    {
        yield return null;
        StartCoroutine(OnProgressingText());
        StartCoroutine(OnProgressing());
        yield return null;
    }

        // UI ���� �ð� �Ϸ�, UI ���� State ��ȯ �޼���
        private void StartEndOfProgress()
    {
        StopAllCoroutines();
        timeCount = 0;
        progressText.text = processingCompleteMessage;
        StartCoroutine(OnEndOfProgress());
    }

    // UI ���� ȣ�� �޼���, UI �Ŵ������� �ش� UI Active True�� �ϸ� ȣ��, ���ڷ� UI ���� �ð�, �⺻ �޼���, ���� �޼��� ����
    public void StartProgress(float duration, string middleOfProgressMessage, string progressCompleteMessage)
    {
        this.duration = duration;
        this.middleOfProcessMessages = middleOfProgressMessage;
        this.processingCompleteMessage = progressCompleteMessage;
        timeCount = 0;
        progressBar.fillAmount = 0f;
        progressText.text = middleOfProcessMessages;
        StartCoroutine(OnStart());
    }
}

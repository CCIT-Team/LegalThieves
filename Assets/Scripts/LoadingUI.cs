using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    private Image progressBarImage;
    private bool isStartEnd = false;
    private bool isReversed = false;
    private float fillAmount;
    private float textFontDefaultSize;
    private float timer = 0f;
    private int commaCounter = 0;

    [SerializeField] private Transform progressBarTransform;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] float duration = 2f;
    [SerializeField] private string[] progressingMessages = { "��ġ ���� ��" };
    [SerializeField] private string progressEndMessage = "��ġ ���� �Ϸ�!";
    [SerializeField] AnimationCurve progressingCurve;
    [SerializeField] AnimationCurve endingCurve;

    [SerializeField] float testLoadingTime = 10f;

    private void Start()
    {
        textFontDefaultSize = progressText.fontSize;
    }

    private void Update()
    {
        if (!isStartEnd)
        {
            progressText.fontSize = textFontDefaultSize * progressingCurve.Evaluate(timer % 1f);
            if (!isReversed)
            {
                fillAmount = (timer / duration) % 1f;
                progressBarImage.fillAmount = fillAmount;
            }
            else
            {
                fillAmount = 1f - ((timer / duration) % 1f);
                progressBarTransform.localEulerAngles = new Vector3(0f, 0f, (timer / duration) % 1f * 360f);
                progressBarImage.fillAmount = fillAmount;
            }
        }
        else progressText.fontSize = textFontDefaultSize * endingCurve.Evaluate((timer / duration) % 1f);

        if (progressBarImage.fillAmount == 1f || progressBarImage.fillAmount == 0f)
        {
            isReversed = !isReversed;
        }

        timer += Time.deltaTime;

        if (!isStartEnd && timer > testLoadingTime) StartEndOfProgress();
        //�̻��� ���°� �����ϰ� �ٸ� ������ �̻��� ���� �����
        if (isStartEnd && timer > duration) gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        timer = 0f;
        fillAmount = 0f;
        commaCounter = 0;
        isStartEnd = false;
        isReversed = false;
        progressBarImage = progressBarTransform.GetComponent<Image>();
        progressBarImage.fillAmount = fillAmount;
        progressBarTransform.localEulerAngles = Vector3.zero;
        int randIndex = Random.Range(0, progressingMessages.Length);
        progressText.text = progressingMessages[randIndex];
        StartCoroutine(TextProgressing());
    }

    //�̻��� �ٸ� ������ ȣ���ؼ� ������ ����ǰ� �����
    private void StartEndOfProgress()
    {
        StopCoroutine(TextProgressing());
        isStartEnd = true;
        timer = 0f;
        progressBarImage.fillAmount = 1f;
        progressText.text = progressEndMessage;
    }

    private IEnumerator TextProgressing()
    {
        yield return new WaitForSeconds(1f);
        if (!isStartEnd && commaCounter < 3)
        {
            progressText.text += ".";
            commaCounter++;
            StartCoroutine(TextProgressing());
        }
        else if (!isStartEnd && commaCounter >= 3)
        {
            int randIndex = Random.Range(0, progressingMessages.Length);
            progressText.text = progressingMessages[randIndex];
            commaCounter = 0;
            StartCoroutine(TextProgressing());
        }
    }
}

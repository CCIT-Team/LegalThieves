using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionProgressUITemp : MonoBehaviour
{
    [SerializeField] private string middleOfProcessMessages = "유물 훼손 중"; //UI 메세지 
    [SerializeField] private string processingCompleteMessage = "유물 훼손 완료!"; //UI 종료 메세지 

    [SerializeField] private float duration = 10.0f; //UI 지속 시간 
    [SerializeField] private float closingDuration = 1.5f; //UI 종료 지속 시간 
    [SerializeField] private AnimationCurve textAnimation; //UI 메세지 애니메이션
    [SerializeField] private AnimationCurve endAnimation; //UI 메세지 종료 애니메이션
    [SerializeField] private float textBouncingMultiplier = 1f; //UI 메세지 애니메이션 Scale 배율 코루틴

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

    private IEnumerator OnProgressing() //UI 진행 코루틴
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

        private IEnumerator OnProgressingText() //UI 메세지 진행 코루틴
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

    private IEnumerator OnEndOfProgress() //UI 종료 코루틴
    {
        timeCount += Time.deltaTime;
        progressText.fontSize = endAnimation.Evaluate(timeCount / closingDuration) * textFontDefaultSize * textBouncingMultiplier;
        yield return null;

        if (timeCount / closingDuration < 1f) StartCoroutine(OnEndOfProgress());
        else this.gameObject.SetActive(false); // UI 기능 완전 종료, UI 매니저의 상호작용 로딩 UI 종료 메서드 호출
    }

    private IEnumerator OnStart() //UI 시작 코루틴
    {
        yield return null;
        StartCoroutine(OnProgressingText());
        StartCoroutine(OnProgressing());
        yield return null;
    }

        // UI 지속 시간 완료, UI 종료 State 전환 메서드
        private void StartEndOfProgress()
    {
        StopAllCoroutines();
        timeCount = 0;
        progressText.text = processingCompleteMessage;
        StartCoroutine(OnEndOfProgress());
    }

    // UI 시작 호출 메서드, UI 매니저에서 해당 UI Active True를 하며 호출, 인자로 UI 지속 시간, 기본 메세지, 종료 메세지 설정
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

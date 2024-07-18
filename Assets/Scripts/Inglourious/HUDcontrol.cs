using TMPro;
using UnityEngine;

public class HUDcontrol : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TMP_Text    tmpText;
    [SerializeField] private float       time = 900;
    [Space] 
    [Header("Compass")]
    [SerializeField] private Transform   player;
    [SerializeField] private Transform   compass;

    #region Singleton

    public static HUDcontrol Singleton
    {
        get => _singleton;
        private set
        {
            if      (value == null)        _singleton = null;
            else if (_singleton == null)   _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
                Debug.LogError($"{nameof(HUDcontrol)}는(은) 단 한번만 인스턴싱되어야 합니다!");
            }
        }
    }
    private static HUDcontrol _singleton;

    #endregion
    
    private bool  _isTimerRun;
    private float _timer;

    private void Awake()
    {
        tmpText.text = "15 : 00";
        _timer = time;
    }

    private void Update()
    {
        TempInput();
        if (_isTimerRun) SetTimer(_timer -= Time.deltaTime);
        RotateCompass();
    }

    private void SetTimer(float t)
    {
        var rt = (int)t;
        tmpText.text = (rt / 60 < 10 ? "0" + rt / 60 : rt / 60) + " : " + (rt % 60 < 10 ? "0" + rt % 60 : rt % 60);
    }

    private void RotateCompass()
    {
        compass.eulerAngles = new Vector3(0, 0, player.eulerAngles.y);
    }

    private void TempInput()
    {
        if (Input.GetKeyDown(KeyCode.F1)) StartTimer();
        else if (Input.GetKeyDown(KeyCode.F2)) PauseTimer();
        else if (Input.GetKeyDown(KeyCode.F3)) ResetTimer();
    }
    
    public void StartTimer()
    {
        _isTimerRun = true;
    }

    public void PauseTimer()
    {
        _isTimerRun = false;
    }

    public void ResetTimer()
    {
        _isTimerRun = false;
        _timer = time;
        SetTimer(time);
    }
}

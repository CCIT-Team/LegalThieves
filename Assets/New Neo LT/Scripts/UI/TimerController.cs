using System;
using TMPro;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class TimerController : MonoBehaviour
    {
        [SerializeField] private TMP_Text round;
        [SerializeField] private TMP_Text timer;

        private void Start()
        {
            timer ??= transform.Find("Time")?.GetComponent<TMP_Text>();
            round ??= transform.Find("Round")?.GetComponent<TMP_Text>();
        }

        public void SetTimer(int time)
        {
            timer.text = (time / 60 < 10 ? "0" + time / 60 : time / 60) + " : " + (time % 60 < 10 ? "0" + time % 60 : time % 60);
        }
        
        public void SetTimer(float time)
        {
            var intTime = time < 0 ? 0 : (int)time;
            timer.text = (intTime / 60 < 10 ? "0" + intTime / 60 : intTime / 60) + " : " + (intTime % 60 < 10 ? "0" + intTime % 60 : intTime % 60);
        }
        
        public void SetTimer(string time)
        {
            timer.text = time;
        }
    }
}
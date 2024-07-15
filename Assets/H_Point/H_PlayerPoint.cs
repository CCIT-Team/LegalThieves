using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class H_PlayerPoint : MonoBehaviour
{
    private void Start()
    {
        _remainPoint = _sumPoint;
    }

    Relic relic;
    H_GameManager.GoldOrRenown PlayerWinPoint;

    [SerializeField]
    int _goldPoint, _renownPoint, _sumPoint, _remainPoint, maxPoint, _winPoint;

    //속성

    public int SumPoint
    {
        get { return _sumPoint; }

    }
    public int RemainPoint { get { return _remainPoint; } }
    public int GoldPoint { get { return _goldPoint; } }
    public int RenownPoint { get { return _renownPoint; } }
    public int WinPoint { get { return _winPoint; } }



    void SetRemain()// 주울수 있는 양을 설정
    {
        _remainPoint = _sumPoint - (_goldPoint + _renownPoint);
        //UI에 바로 표현
    }

    void GetRelic(Relic relic) // 렐릭을 주웠을 때
    {
        if (_remainPoint - relic.goldPoint - relic.renownPoint < 0) // 못주울 때
        { PointOver(); return; }

        _goldPoint += relic.goldPoint;

        _renownPoint += relic.renownPoint;

        SetRemain();
    }


    void PointOver() // 더 못주울 때
    {
        Debug.Log("더이상 주울 수 없습니다");
    }

    void EndRound()
    {
        _sumPoint += _goldPoint + _renownPoint; //총점수 += 현재점수 
        SetRemain();   //남은 스코어 
    }
}
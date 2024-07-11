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

    //�Ӽ�

    public int SumPoint
    {
        get { return _sumPoint; }

    }
    public int RemainPoint { get { return _remainPoint; } }
    public int GoldPoint { get { return _goldPoint; } }
    public int RenownPoint { get { return _renownPoint; } }
    public int WinPoint { get { return _winPoint; } }



    void SetRemain()// �ֿ�� �ִ� ���� ����
    {
        _remainPoint = _sumPoint - (_goldPoint + _renownPoint);
        //UI�� �ٷ� ǥ��
    }

    void GetRelic(Relic relic) // ������ �ֿ��� ��
    {
        if (_remainPoint - relic.goldPoint - relic.renownPoint < 0) // ���ֿ� ��
        { PointOver(); return; }

        _goldPoint += relic.goldPoint;

        _renownPoint += relic.renownPoint;

        SetRemain();
    }


    void PointOver() // �� ���ֿ� ��
    {
        Debug.Log("���̻� �ֿ� �� �����ϴ�");
    }

    void EndRound()
    {
        _sumPoint += _goldPoint + _renownPoint; //������ += �������� 
        SetRemain();   //���� ���ھ� 
    }
}
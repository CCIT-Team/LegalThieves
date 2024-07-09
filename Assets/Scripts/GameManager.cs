using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        StartPlay();
    }

    #region 플레이 시간

    [SerializeField]
    [Range(0f, 1200f)]
    float playTime = 300;
    float currentTime = 0;
    void StartPlay()
    {
        StartCoroutine(PlayTimeCheck());
    }

    IEnumerator PlayTimeCheck()
    {
        while (currentTime <= playTime)
        {
            yield return new WaitForFixedUpdate();
            currentTime += Time.deltaTime;
            Debug.Log((int)currentTime / 60 + ":" + currentTime % 60);
        }
        Debug.Log("GameIsEnd");
        yield return null;
    }
    #endregion

    #region 포인트관리

    int[] goldScore = new int[4];
    int[] renownScore = new int[4];
    [SerializeField]
    int maxScore = 5000;

    public void GetScore(int player, int goldPoint = 0, int renownPoint = 0)
    {
        goldScore[player] += goldPoint;
        renownScore[player] += goldPoint;
    }

    public bool UseScore(int player, int goldPoint = 0, int renownPoint = 0)
    {
        if (goldScore[player] - goldPoint < 0 || renownScore[player] - renownPoint < 0)
            return false;
        else
        {
            goldScore[player] -= goldPoint;
            renownScore[player] -= renownPoint;
            return true;
        }
    }
    #endregion

    [Header("Dungeon")]
    public Vector3 min;
    public Vector3 max;
    public int roomAmount = 10;
    [Space(20)]
    [Header("Relic")]
    public int RelicAmount = 30;
    [Tooltip("X=Gold, Y=Renown")]
    public Vector2 ratio = new Vector2(15,15);
    public int minValue = 100;
    public int maxValue = 3000;
}

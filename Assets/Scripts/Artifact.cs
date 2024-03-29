using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEditor.Experimental;
using UnityEngine.UIElements;


public class Artifact : MonoBehaviour
{

    public GameObject[] artifactPrefabs; // 수정: 기존 Artifact의 프리팹들을 담을 배열, // Artifact 프리팹 배열
    public List<GameObject> artifactItems = new List<GameObject>(); // Artifact 아이템을 저장할 리스트

    private List<Vector3> artifactLocations = new List<Vector3>(); // Artifact 위치 리스트
                                                                   // Start is called before the first frame update

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        GenerateArtifactLocations(); // 수정: Artifact 위치 생성
        MoveArtifactsToRandomLocations(); // Awake()에서 Artifact를 무작위 위치로 이동하는 것으로 변경
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Artifact생성및 설정 부여, 각가의 위치로 이동
    void MoveArtifactsToRandomLocations()
    {
        List<int> randomIndexes = new List<int>();

        if (artifactLocations.Count >= artifactItems.Count)
        {
            for (int i = 0; i < artifactItems.Count; i++)
            {
                int randomIndex = Random.Range(0, artifactLocations.Count);
                artifactItems[i].transform.position = artifactLocations[randomIndex];

                // 
                // 나머지 설정들
                int year = GetRandomYear(); // 무작위 년도 설정
                artifactItems[i].GetComponent<Item>().year = year;
                artifactItems[i].GetComponent<Item>().price = GetRandomPrice(year); // 무작위 가격 설정
                randomIndexes.Add(randomIndex);
            }

            int numMissingArtifacts = artifactLocations.Count - artifactItems.Count;
            for (int i = 0; i < numMissingArtifacts; i++)
            {
                GameObject randomArtifactPrefab = artifactPrefabs[Random.Range(0, artifactPrefabs.Length)];
                GameObject newArtifact = Instantiate(randomArtifactPrefab, GetRandomPosition(), Quaternion.identity);
                newArtifact.tag = "Artifact";
                int year = GetRandomYear(); // 무작위 년도 설정
                newArtifact.GetComponent<Item>().year = year;
                newArtifact.GetComponent<Item>().price = GetRandomPrice(year); // 무작위 가격 설정

                int closestIndex = FindClosestLocationIndex(newArtifact.transform.position, randomIndexes);
                newArtifact.transform.position = artifactLocations[closestIndex];
                randomIndexes.Add(closestIndex);
            }
        }
        else
        {
            for (int i = 0; i < artifactLocations.Count; i++)
            {
                int randomIndex = Random.Range(0, artifactItems.Count);

                while (randomIndexes.Contains(randomIndex))
                {
                    randomIndex = Random.Range(0, artifactItems.Count);
                }

                randomIndexes.Add(randomIndex);

                artifactItems[randomIndex].transform.position = artifactLocations[i];
                int year = GetRandomYear(); // 무작위 년도 설정
                artifactItems[randomIndex].GetComponent<Item>().year = year;
                artifactItems[randomIndex].GetComponent<Item>().price = GetRandomPrice(year); // 무작위 가격 설정
            }
        }

        for (int i = artifactLocations.Count - 1; i >= 0; i--)
        {
            if (!randomIndexes.Contains(i))
            {
                int randomIndex = Random.Range(0, artifactItems.Count);
                artifactItems[randomIndex].transform.position = artifactLocations[i];
                int year = GetRandomYear(); // 무작위 년도 설정
                artifactItems[randomIndex].GetComponent<Item>().year = year;
                artifactItems[randomIndex].GetComponent<Item>().price = GetRandomPrice(year); // 무작위 가격 설정
                randomIndexes.Add(i);
            }
        }
    }


 //무작위 위치에 Artifact를 생성
 Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
        return randomPosition;
    }


 //가장 가까운 ArtifactLocation으로 Artifact를 이동
 int FindClosestLocationIndex(Vector3 artifactPosition, List<int> usedIndexes)
    {
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < artifactLocations.Count; i++)
        {
            if (usedIndexes.Contains(i))
                continue;

            // Vector3에 대한 거리 계산만 필요하므로 transform.position을 제거합니다.
            float distance = Vector3.Distance(artifactPosition, artifactLocations[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }


 //확률적 가격 설정
 int GetRandomYear()
    {
        int randomNumber = Random.Range(1, 101); // 1부터 100까지의 무작위 숫자 생성

        if (randomNumber <= 20) // 20% 확률
        {
            return Random.Range(100, 501); // 100에서 500 사이의 값 반환
        }
        else if (randomNumber <= 60) // 나머지 80% 중 40% 확률
        {
            return Random.Range(501, 1001); // 501에서 1000 사이의 값 반환
        }
        else if (randomNumber <= 80) // 나머지 80% 중 20% 확률
        {
            return Random.Range(1001, 1501); // 1001에서 1500 사이의 값 반환
        }
        else if (randomNumber <= 90) // 나머지 80% 중 10% 확률
        {
            return Random.Range(1501, 2001); // 1501에서 2000 사이의 값 반환
        }
        else if (randomNumber <= 95) // 나머지 80% 중 5% 확률
        {
            return Random.Range(2001, 2501); // 2001에서 2500 사이의 값 반환
        }
        else // 나머지 80% 중 마지막 5% 확률
        {
            return Random.Range(2501, 3001); // 2501에서 3000 사이의 값 반환
        }
    }

  //확률적 연도 생성
  float GetRandomPrice(int year)
    {
        float multiplier = 0f;

        if (year >= 100 && year <= 500)
            multiplier = Random.Range(0.1f, 10f);
        else if (year >= 501 && year <= 1000)
            multiplier = Random.Range(0.1f, 9f);
        else if (year >= 1001 && year <= 1500)
            multiplier = Random.Range(0.1f, 8f);
        else if (year >= 1501 && year <= 2000)
            multiplier = Random.Range(0.1f, 7f);
        else if (year >= 2001 && year <= 2500)
            multiplier = Random.Range(0.1f, 6f);
        else if (year >= 2501 && year <= 3000)
            multiplier = Random.Range(0.1f, 5f);

        float price = year * multiplier;

        // 소수점 이하 반올림
        price = Mathf.Round(price * 10) / 10f;

        return price;
    }

    void GenerateArtifactLocations()
    {
        for (int i = 0; i < 15; i++) // 15개의 무작위 위치 생성
        {
            float randomX = Random.Range(-10f, 15f); // -40부터 40까지 무작위 x 좌표 생성
            float randomZ = Random.Range(-10f, 15f); // -40부터 40까지 무작위 z 좌표 생성
            artifactLocations.Add(new Vector3(randomX, 1f, randomZ)); // 생성된 좌표를 리스트에 추가

        }
    }


}


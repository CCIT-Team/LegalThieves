using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEditor.Experimental;
using UnityEngine.UIElements;


public class Artifact : MonoBehaviour
{

    public GameObject[] artifactPrefabs; // ����: ���� Artifact�� �����յ��� ���� �迭, // Artifact ������ �迭
    public List<GameObject> artifactItems = new List<GameObject>(); // Artifact �������� ������ ����Ʈ

    private List<Vector3> artifactLocations = new List<Vector3>(); // Artifact ��ġ ����Ʈ
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

        GenerateArtifactLocations(); // ����: Artifact ��ġ ����
        MoveArtifactsToRandomLocations(); // Awake()���� Artifact�� ������ ��ġ�� �̵��ϴ� ������ ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Artifact������ ���� �ο�, ������ ��ġ�� �̵�
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
                // ������ ������
                int year = GetRandomYear(); // ������ �⵵ ����
                artifactItems[i].GetComponent<Item>().year = year;
                artifactItems[i].GetComponent<Item>().price = GetRandomPrice(year); // ������ ���� ����
                randomIndexes.Add(randomIndex);
            }

            int numMissingArtifacts = artifactLocations.Count - artifactItems.Count;
            for (int i = 0; i < numMissingArtifacts; i++)
            {
                GameObject randomArtifactPrefab = artifactPrefabs[Random.Range(0, artifactPrefabs.Length)];
                GameObject newArtifact = Instantiate(randomArtifactPrefab, GetRandomPosition(), Quaternion.identity);
                newArtifact.tag = "Artifact";
                int year = GetRandomYear(); // ������ �⵵ ����
                newArtifact.GetComponent<Item>().year = year;
                newArtifact.GetComponent<Item>().price = GetRandomPrice(year); // ������ ���� ����

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
                int year = GetRandomYear(); // ������ �⵵ ����
                artifactItems[randomIndex].GetComponent<Item>().year = year;
                artifactItems[randomIndex].GetComponent<Item>().price = GetRandomPrice(year); // ������ ���� ����
            }
        }

        for (int i = artifactLocations.Count - 1; i >= 0; i--)
        {
            if (!randomIndexes.Contains(i))
            {
                int randomIndex = Random.Range(0, artifactItems.Count);
                artifactItems[randomIndex].transform.position = artifactLocations[i];
                int year = GetRandomYear(); // ������ �⵵ ����
                artifactItems[randomIndex].GetComponent<Item>().year = year;
                artifactItems[randomIndex].GetComponent<Item>().price = GetRandomPrice(year); // ������ ���� ����
                randomIndexes.Add(i);
            }
        }
    }


 //������ ��ġ�� Artifact�� ����
 Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
        return randomPosition;
    }


 //���� ����� ArtifactLocation���� Artifact�� �̵�
 int FindClosestLocationIndex(Vector3 artifactPosition, List<int> usedIndexes)
    {
        float closestDistance = Mathf.Infinity;
        int closestIndex = -1;

        for (int i = 0; i < artifactLocations.Count; i++)
        {
            if (usedIndexes.Contains(i))
                continue;

            // Vector3�� ���� �Ÿ� ��길 �ʿ��ϹǷ� transform.position�� �����մϴ�.
            float distance = Vector3.Distance(artifactPosition, artifactLocations[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }


 //Ȯ���� ���� ����
 int GetRandomYear()
    {
        int randomNumber = Random.Range(1, 101); // 1���� 100������ ������ ���� ����

        if (randomNumber <= 20) // 20% Ȯ��
        {
            return Random.Range(100, 501); // 100���� 500 ������ �� ��ȯ
        }
        else if (randomNumber <= 60) // ������ 80% �� 40% Ȯ��
        {
            return Random.Range(501, 1001); // 501���� 1000 ������ �� ��ȯ
        }
        else if (randomNumber <= 80) // ������ 80% �� 20% Ȯ��
        {
            return Random.Range(1001, 1501); // 1001���� 1500 ������ �� ��ȯ
        }
        else if (randomNumber <= 90) // ������ 80% �� 10% Ȯ��
        {
            return Random.Range(1501, 2001); // 1501���� 2000 ������ �� ��ȯ
        }
        else if (randomNumber <= 95) // ������ 80% �� 5% Ȯ��
        {
            return Random.Range(2001, 2501); // 2001���� 2500 ������ �� ��ȯ
        }
        else // ������ 80% �� ������ 5% Ȯ��
        {
            return Random.Range(2501, 3001); // 2501���� 3000 ������ �� ��ȯ
        }
    }

  //Ȯ���� ���� ����
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

        // �Ҽ��� ���� �ݿø�
        price = Mathf.Round(price * 10) / 10f;

        return price;
    }

    void GenerateArtifactLocations()
    {
        for (int i = 0; i < 15; i++) // 15���� ������ ��ġ ����
        {
            float randomX = Random.Range(-10f, 15f); // -40���� 40���� ������ x ��ǥ ����
            float randomZ = Random.Range(-10f, 15f); // -40���� 40���� ������ z ��ǥ ����
            artifactLocations.Add(new Vector3(randomX, 1f, randomZ)); // ������ ��ǥ�� ����Ʈ�� �߰�

        }
    }


}


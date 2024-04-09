using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class MobPatrol : State
    { private float timer;
        private int PatrolDelay=2;
    
    
        public Transform[] patrolPoints;

        void Awake()
        {
            Monster = GetComponent<Monster>();
            patrolPoints = GameObject.FindWithTag("MobPatrollParent").GetComponentsInChildren<Transform>();
        }

      
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            PatrollDelay();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("MobPatrollPoint")) return;
            Debug.Log("도착");
            PatrollDelay();
        }
        private void PatrollDelay()
        {
            if (Monster.agent.velocity != Vector3.zero) return;
            
            timer += Time.deltaTime;
       
            if (timer > PatrolDelay)
            {
          
                timer = 0;
                NearPointSet(patrolPoints,0,patrolPoints.Length-1);
                Monster.agent.SetDestination(patrolPoints[Random.Range(2,3)].position);
            }
        }

        private void NearPointSet(Transform[] arr, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(arr, low, high);
                NearPointSet(arr, low, pivotIndex - 1);
                NearPointSet(arr, pivotIndex + 1, high);
            }
        }

        private int Partition(Transform[] arr, int low, int high)
        {
            Transform pivot = arr[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (Vector3.Distance(transform.position, arr[j].position) 
                    < Vector3.Distance(transform.position,pivot.position))
                {
                    i++;
                    Swap(arr, i, j);
                }
            }
            Swap(arr, i + 1, high);
            return i + 1;
        }

        private void Swap(Transform[] arr, int a, int b)
        {
            (arr[a], arr[b]) = (arr[b], arr[a]);
        }

    
    }

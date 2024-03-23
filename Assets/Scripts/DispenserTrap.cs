using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

public class DispenserTrap : MonoBehaviour
{
    public GameObject arrowPrefab;
    [SerializeField]
    private GameObject targetObj;  //발판 밟은 오브젝트

    private Transform dispenser;
    private Transform plate;

    private int trapState = 0;  //0 : 대기, 1 : 반응 후 발사준비, 2 : 발사 3 : 대기(잠정)
    [SerializeField]
    private float shootingTime; //발사 지속시간
    [SerializeField]
    private float fireInterval; //발사 간격
    private float plateLerping; //발판 lerp이동 인스펙터
    private Vector3 platePosition;
    private Vector3 initialPlatePosition;
    private Vector3 maxPlatePosition;

    private Vector3 currentDispenserPosition;
    private Vector3 targetDispenserPosition;

    private float delta;




    private void OnEnable()
    {
        dispenser = transform.GetChild(0);
        plate = transform.GetChild(1);

        maxPlatePosition = initialPlatePosition = plate.position;
        maxPlatePosition.y = -0.11f;

        targetDispenserPosition = dispenser.position;
        targetDispenserPosition.y += 2.2f;

         
    }

    private void OnTriggerEnter(Collider other)
    {
        if (trapState == 0)
        {
            trapState = 1;
            targetObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    public void OnTriggerStay(Collider other)
    {
        //발판 내려감
        platePosition = Vector3.Lerp(initialPlatePosition, maxPlatePosition, plateLerping * Time.deltaTime);
        plate.position = platePosition;
        plateLerping++;
    }

    private void Update()
    {
        delta = Time.deltaTime;
        switch (trapState)
        {
            case 1:
                ActivateTrap();
                break;
            case 2:
                InvokeRepeating("SpawnArrow", 0, fireInterval);
                Invoke("StopShootinig", shootingTime);
                trapState = 3;
                break;
        }
    }
    

    private void ActivateTrap()
    {
        if (currentDispenserPosition.y < targetDispenserPosition.y - 0.001f)
        {
            currentDispenserPosition = Vector3.Lerp(dispenser.position, targetDispenserPosition, 3f * Time.deltaTime);
            dispenser.position = currentDispenserPosition;
        }
        else
            trapState = 2;
    }

    private void SpawnArrow()
    {
        // Dispenser 위치에서 targetObj를 바라보게 Arrow 오브젝트를 생성
        GameObject arrow = Instantiate(arrowPrefab, dispenser.position + Vector3.up * 0.5f, Quaternion.LookRotation(targetObj.transform.position, Vector3.forward));
        ArrowManager ar = arrow.GetComponent<ArrowManager>();
        ar.StartCoroutine(ar.ShootArrow(targetObj.transform.position, 5f, delta));
    }

    

    private void StopShootinig()
    {
        CancelInvoke("SpawnArrow");

        Destroy(this);
    }
}

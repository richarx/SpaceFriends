using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CalibrationModule : NetworkBehaviour
{
    [SerializeField] private GameObject nutSelected;
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject bulbOff;
    [SerializeField] private GameObject bulbOn;
    [SerializeField] private GameObject bulbBroken;
    [SerializeField] private GameObject brokenModule;
    [SerializeField] private GameObject smoke;

    public bool isCalibrated => calibrationStep == 0;
    private int calibrationStep = 0;
    
    [HideInInspector] public bool isBroken = false;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            StartCoroutine(UncalibrateModule());
    }

    private IEnumerator UncalibrateModule()
    {
        bool direction = false;

        while (true)
        {
            if (isCalibrated)
                direction = Tools.RandomBool();

            if (calibrationStep < 9 && calibrationStep > -9)
            {
                calibrationStep += direction ? 1 : -1;
                SetCursorPositionRpc(calibrationStep);
            }
            else
            {
                BreakDownModuleRpc();
            }
            
            yield return new WaitForSeconds(5.0f);
        }
    }
    
    [Rpc(SendTo.Everyone)]
    private void BreakDownModuleRpc()
    {
        isBroken = true;
        bulbOff.SetActive(false);
        bulbOn.SetActive(false);
        cursor.gameObject.SetActive(false);
        bulbBroken.SetActive(true);
        smoke.SetActive(true);
        brokenModule.SetActive(true);
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetCursorPositionRpc(int step)
    {
        calibrationStep = step;
        cursor.localPosition = new Vector3(step * 0.1f, 0.42f, 0.0f);
        SetBulbState(step >= 5 || step <= -5);
    }

    private void SetBulbState(bool alarmed)
    {
        bulbOff.SetActive(!alarmed);
        bulbOn.SetActive(alarmed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer && other.CompareTag("Player"))
        {
            SetNutStateRpc(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isQuitting && IsServer && other.CompareTag("Player"))
        {
            SetNutStateRpc(false);
        }
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetNutStateRpc(bool state)
    {
        nutSelected.SetActive(state);
    }

    private bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}

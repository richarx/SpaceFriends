using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CalibrationModule : NetworkBehaviour
{
    [SerializeField] private Animator nutSelected;
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject bulbOff;
    [SerializeField] private GameObject bulbOn;
    [SerializeField] private GameObject bulbBroken;
    [SerializeField] private GameObject brokenModule;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject screen;
    [SerializeField] private SpriteRenderer repairBar;

    public bool isCalibrated => calibrationStep == 0;
    private int calibrationStep = 0;

    private int repairStep = 0;
    
    [HideInInspector] public bool isBroken = false;

    private float uncalibrationSpeed = 5.0f;
    
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

            yield return new WaitWhile(() => isBroken);

            if (calibrationStep < 9 && calibrationStep > -9)
            {
                if (Tools.RandomBool())
                {
                    calibrationStep += direction ? 1 : -1;
                    SetCursorPositionRpc(calibrationStep);
                }
            }
            else
            {
                BreakDownModuleRpc();
            }

            yield return new WaitForSeconds(uncalibrationSpeed);
        }
    }

    public bool UseWrench()
    {
        if (!nutSelected.gameObject.activeSelf)
            return false;
            
        if (isBroken)
        {
            RepairModuleRpc(true);
            return true;
        }

        if (!isCalibrated)
        {
            RecalibrateModuleRpc();
            return true;
        }

        return false;
    }
    
    public bool UseTorch()
    {
        if (!nutSelected.gameObject.activeSelf)
            return false;
            
        if (isBroken)
        {
            RepairModuleRpc(false);
            return true;
        }
        
        return false;
    }

    [Rpc(SendTo.Everyone)]
    private void RepairModuleRpc(bool isUsingWrench)
    {
        repairStep += isUsingWrench ? 1 : 4;

        if (repairStep >= 20)
        {
            BuildBackModule();
            return;
        }
        
        repairBar.size = new Vector2(repairStep * 0.18f, 2.5f);
        
        if (isUsingWrench)
            nutSelected.Play("Wrench_Recalibrate");
    }

    [Rpc(SendTo.Everyone)]
    private void RecalibrateModuleRpc()
    {
        if (!isCalibrated)
        {
            nutSelected.Play("Wrench_Recalibrate");
            calibrationStep -= Math.Sign(calibrationStep);
        }
        
        SetCursorPositionRpc(calibrationStep);
    }

    private void BuildBackModule()
    {
        isBroken = false;
        bulbOff.SetActive(true);
        bulbOn.SetActive(false);
        bulbBroken.SetActive(false);
        cursor.gameObject.SetActive(true);
        screen.SetActive(true);
        smoke.SetActive(false);
        brokenModule.SetActive(false);
        repairStep = 0;
        calibrationStep = 0;
        SetCursorPosition(calibrationStep);
    }
    
    [Rpc(SendTo.Everyone)]
    private void BreakDownModuleRpc()
    {
        isBroken = true;
        bulbOff.SetActive(false);
        bulbOn.SetActive(false);
        cursor.gameObject.SetActive(false);
        screen.SetActive(false);
        bulbBroken.SetActive(true);
        smoke.SetActive(true);
        brokenModule.SetActive(true);
        repairBar.size = new Vector2(0.0f, 2.5f);
        repairStep = 0;
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetCursorPositionRpc(int step)
    {
        calibrationStep = step;
        SetCursorPosition(step);
        SetBulbState(step >= 5 || step <= -5);
    }

    private void SetCursorPosition(int step)
    {
        cursor.localPosition = new Vector3(step * 0.1f, 0.42f, 0.0f);
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
        nutSelected.gameObject.SetActive(state);
    }

    private bool isQuitting = false;

    public override void OnNetworkDespawn()
    {
        isQuitting = true;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }
}

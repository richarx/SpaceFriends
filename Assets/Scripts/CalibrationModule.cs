using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class CalibrationModule : NetworkBehaviour
{
    enum BulbState
    {
        Green,
        Yellow,
        Red,
        Broken
    }
    
    [SerializeField] private Animator nutSelected;
    [SerializeField] private Transform cursor;
    [SerializeField] private GameObject bulbGreen;
    [SerializeField] private GameObject bulbYellow;
    [SerializeField] private GameObject bulbRed;
    [SerializeField] private GameObject bulbBroken;
    [SerializeField] private GameObject brokenModule;
    [SerializeField] private GameObject smoke;
    [SerializeField] private GameObject screen;
    [SerializeField] private SpriteRenderer repairBar;
    [SerializeField] public Transform wrenchUseTarget;

    public bool isCalibrated => calibrationStep == 0;
    private int calibrationStep = 0;
    private int absoluteCalibrationStep => Math.Abs(calibrationStep);

    private int repairStep = 0;
    
    [HideInInspector] public bool isBroken = false;

    private float uncalibrationSpeed = 2.5f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            StartCoroutine(UncalibrateModule());
        
        SetBulbState();
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
        {
            nutSelected.Play("Wrench_Recalibrate");
            audioSource.Play();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void RecalibrateModuleRpc()
    {
        if (!isCalibrated)
        {
            nutSelected.Play("Wrench_Recalibrate");
            audioSource.Play();
            calibrationStep -= Math.Sign(calibrationStep);
        }

        SetCursorPositionRpc(calibrationStep);
    }

    private void BuildBackModule()
    {
        isBroken = false;
        cursor.gameObject.SetActive(true);
        screen.SetActive(true);
        smoke.SetActive(false);
        brokenModule.SetActive(false);
        repairStep = 0;
        calibrationStep = 0;
        SetCursorPosition(calibrationStep);
        SetBulbState();
    }
    
    [Rpc(SendTo.Everyone)]
    private void BreakDownModuleRpc()
    {
        isBroken = true;
        cursor.gameObject.SetActive(false);
        screen.SetActive(false);
        smoke.SetActive(true);
        brokenModule.SetActive(true);
        repairBar.size = new Vector2(0.0f, 2.5f);
        repairStep = 0;
        SetBulbState();
    }
    
    [Rpc(SendTo.Everyone)]
    private void SetCursorPositionRpc(int step)
    {
        calibrationStep = step;
        SetCursorPosition(step);
        SetBulbState();
    }

    private void SetCursorPosition(int step)
    {
        cursor.localPosition = new Vector3(step * 0.1f, 0.42f, 0.0f);
    }

    private void SetBulbState()
    {
        SetBulbState(ComputeBulbState(absoluteCalibrationStep));
    }

    private BulbState ComputeBulbState(int step)
    {
        if (absoluteCalibrationStep > 9)
            return BulbState.Broken;

        if (absoluteCalibrationStep >= 7)
            return BulbState.Red;

        if (absoluteCalibrationStep >= 5)
            return BulbState.Yellow;

        return BulbState.Green;
    }
    
    private void SetBulbState(BulbState state)
    {
        bulbGreen.SetActive(state == BulbState.Green);
        bulbYellow.SetActive(state == BulbState.Yellow);
        bulbRed.SetActive(state == BulbState.Red);
        bulbBroken.SetActive(state == BulbState.Broken);
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

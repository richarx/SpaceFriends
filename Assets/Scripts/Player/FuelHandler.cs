using System;
using Unity.Netcode;
using UnityEngine;

public class FuelHandler : NetworkBehaviour
{
    [SerializeField] private GameObject fuelDisplay;
    [SerializeField] private SpriteRenderer fuelGauge;
    
    private PlayerMovement playerMovement;

    public bool IsFuelEmpty => currentFuel <= 0.0f;
    
    private float maxFuel = 30.0f;
    private float refillSpeed = 3.0f;
    private float consumptionSpeed = 1.0f;
    
    private float currentFuel;
    
    public override void OnNetworkSpawn()
    {
        playerMovement = GetComponent<PlayerMovement>();
        currentFuel = maxFuel;

        if (IsOwner)
        {
            CheatCodes cheatCodes = GetComponent<CheatCodes>();
            cheatCodes.OnSecondaryCheat.AddListener((s) =>
            {
                maxFuel += s * 5.0f;
                currentFuel = maxFuel;
                Debug.Log($"Zuzu : Fuel Tank updated : {maxFuel}");
                UpdateFuelGauge();
            });
            cheatCodes.OnTertiaryCheat.AddListener((() =>
            {
                currentFuel = maxFuel;
                Debug.Log("Zuzu : Fuel Tank refilled");
            }));
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (!playerMovement.IsInSpace)
        {
            currentFuel = maxFuel;
            return;
        }

        if (IsFuelEmpty)
            return;

        currentFuel -= playerMovement.MoveDirection.magnitude * consumptionSpeed * Time.deltaTime;
        currentFuel = Mathf.Clamp(currentFuel, 0.0f, maxFuel);
        UpdateFuelGauge();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("RefuelZone"))
        {
            currentFuel += refillSpeed * Time.deltaTime;
            currentFuel = Mathf.Clamp(currentFuel, 0.0f, maxFuel);
        }
    }

    private void UpdateFuelGauge()
    {
        if (!fuelDisplay.activeSelf)
            return;
        
        float newSize = Tools.NormalizeValueInRange(currentFuel, 0.0f, maxFuel, 0.0f, 0.78f);
        
        fuelGauge.size = new Vector2(newSize, fuelGauge.size.y);
    }

    public void UpdateFuelDisplayState(bool isInSpace)
    {
        fuelDisplay.SetActive(isInSpace);
    }
}

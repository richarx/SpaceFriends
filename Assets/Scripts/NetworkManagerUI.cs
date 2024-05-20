using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private bool useRelay;
    
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField codeTextInput;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            if (useRelay)
                CreateRelay();
            else
                NetworkManager.Singleton.StartHost();
        });
        
        clientButton.onClick.AddListener(() =>
        {
            if (useRelay)
                ConnectToRelay();
            else
                NetworkManager.Singleton.StartClient();
        });
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Zuzu : Signed into Unity Service !");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            
            Debug.Log($"Zuzu : JOIN CODE : {joinCode}");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            
            NetworkManager.Singleton.StartHost();
            DeactivateUI();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void ConnectToRelay()
    {
        string code = codeTextInput.text;
        
        if (string.IsNullOrEmpty(code))
            return;
        
        try
        {
            Debug.Log($"Zuzu : Joining Relay with code : {code}");
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);
            
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            DeactivateUI();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void DeactivateUI()
    {
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        codeTextInput.gameObject.SetActive(false);
    }
}

using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayInstrument : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips;
    
    [HideInInspector] public bool isPlayingBanjo = false;
    private ulong banjoID;

    private PlayerMovement playerMovement;
    private ItemHandler itemHandler;
    private PlayerAnimation playerAnimation;
    
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        itemHandler = GetComponent<ItemHandler>();
        playerAnimation = GetComponentInChildren<PlayerAnimation>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        
        if (!isPlayingBanjo)
            return;

        if (playerMovement.MoveDirection.magnitude > 0.5f || !itemHandler.IsHoldingItem)
            StopPlayingRpc();
    }

    public void PlayBanjo()
    {
        int clip = Random.Range(0, clips.Count);
        PlayBanjoRpc(clip);
    }

    [Rpc(SendTo.Everyone)]
    private void PlayBanjoRpc(int clip)
    {
        isPlayingBanjo = true;
        banjoID = ItemParentingAuthority.Instance.GetItem(itemHandler)!.NetworkObjectId;
        SetBanjoState(false);
        playerAnimation.PlayBanjo();
        audioSource.clip = clips[clip];
        audioSource.Play();
    }

    [Rpc(SendTo.Everyone)]
    private void StopPlayingRpc()
    {
        isPlayingBanjo = false;
        SetBanjoState(true);
        audioSource.Stop();
    }

    private void SetBanjoState(bool state)
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[banjoID].transform.GetChild(0).gameObject.SetActive(state);
    }  
}

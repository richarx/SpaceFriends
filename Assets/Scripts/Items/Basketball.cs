using Unity.Netcode;

public class Basketball : UsableItem
{
    private PickableItem pickableItem;

    private bool isDribbling = false;
    
    private void Start()
    {
        pickableItem = GetComponent<PickableItem>();
    }

    private void Update()
    {
        if (IsServer && !pickableItem.isBeingHeld && isDribbling)
        {
            StopDribblingRpc();
        }   
    }

    public override void UseItem(ItemHandler itemHandler)
    {
        if (isDribbling)
            StopDribblingRpc();
        else
            StartDribblingRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void StartDribblingRpc()
    {
        isDribbling = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }
    
    [Rpc(SendTo.Everyone)]
    private void StopDribblingRpc()
    {
        isDribbling = false;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}

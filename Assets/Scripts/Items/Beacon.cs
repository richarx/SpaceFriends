using System;
using UnityEngine;

public class Beacon : UsableItem
{
    [SerializeField] private Animator pingAnimator;

    private AudioSource audioSource;
    private PickableItem pickableItem;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        pickableItem = GetComponent<PickableItem>();
    }

    public override void UseItem(ItemHandler itemHandler)
    {
        PingPosition();
    }

    private void PingPosition()
    {
        ItemHandler owner = ItemParentingAuthority.Instance.GetOwner(pickableItem);
        bool isInSpace = owner.GetComponent<PlayerMovement>().IsInSpace;

        if (isInSpace)
            return;

        PingHandler.PingBeaconPosition?.Invoke(owner.transform.position);
        pingAnimator.Play("PingActivated");
        audioSource.Play();
    }
}

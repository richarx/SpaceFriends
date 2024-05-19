using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BringItemToRoom : NetworkBehaviour
{
    [SerializeField] private Image firstItem;
    [SerializeField] private Image firstRoom;
    [SerializeField] private Image secondItem;
    [SerializeField] private Image secondRoom;

    [SerializeField] private List<Sprite> items;
    [SerializeField] private List<Sprite> rooms;

    private void Start()
    {
        if (!IsClient)
            ChooseItemAndRoom();
    }

    private void Update()
    {
        if (!IsServer)
            return;
        
        if (PlayerInputs.CheckForResetObjective())
            ChooseItemAndRoom();
    }

    private void ChooseItemAndRoom()
    {
        List<int> indexes = new List<int>() { 0, 1, 2 };

        List<int> selectedItems = indexes.GetRandomElements(2);
        List<int> selectedRooms = indexes.GetRandomElements(2);

        UpdateSpritesRpc(
            selectedItems[0],
            selectedRooms[0],
            selectedItems[1],
            selectedRooms[1]
        );
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateSpritesRpc(int firstItemIndex, int firstRoomIndex, int secondItemIndex, int secondRoomIndex)
    {
        firstItem.sprite = items[firstItemIndex];
        firstRoom.sprite = rooms[firstRoomIndex];
        secondItem.sprite = items[secondItemIndex];
        secondRoom.sprite = rooms[secondRoomIndex];
    }
}

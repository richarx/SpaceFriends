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

    private int mapSwapped = 0;
    
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

        if (PlayerInputs.CheckForSwapMap())
            mapSwapped += 1;

    }

    private void ChooseItemAndRoom()
    {
        List<int> indexes = new List<int>() { 0, 1, 2 };

        List<int> selectedItems = indexes.GetRandomElements(2);
        
        if (mapSwapped > 2)
            indexes.Add(3);
        List<int> selectedRooms = indexes.GetRandomElements(2);

        UpdateSpritesRpc(
            selectedItems[0],
            selectedRooms[0],
            selectedItems[1],
            selectedRooms[1]
        );
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateSpritesRpc(int firstItemIndex, int firstRoomIndex, int secondItemIndex, int secondRoomIndex)
    {
        Debug.Log("Zuzu : UpdateSprites");
        
        if (!firstItem.transform.parent.gameObject.activeSelf)
            firstItem.transform.parent.gameObject.SetActive(true);
        if (!secondItem.transform.parent.gameObject.activeSelf)
            secondItem.transform.parent.gameObject.SetActive(true);

        firstItem.sprite = items[firstItemIndex];
        firstRoom.sprite = rooms[firstRoomIndex];
        secondItem.sprite = items[secondItemIndex];
        secondRoom.sprite = rooms[secondRoomIndex];
    }
}

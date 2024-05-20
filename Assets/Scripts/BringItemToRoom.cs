using System;
using System.Collections.Generic;
using System.Linq;
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
        List<int> itemIndexes = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
        List<int> roomIndexes = new List<int>() { 0, 1, 2 };

        List<int> selectedItems = itemIndexes.GetRandomElements(2);
        
        if (mapSwapped > 2)
            roomIndexes.Add(3);

        List<int> selectedRooms = new List<int>();
        for (int i = 0; i < 2; i++)
        {
            selectedRooms.Add(roomIndexes.GetRandomElements(1).First());
        }

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

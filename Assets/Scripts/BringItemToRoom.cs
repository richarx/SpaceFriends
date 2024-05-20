using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BringItemToRoom : NetworkBehaviour
{
    [SerializeField] private List<Sprite> items;
    [SerializeField] private List<Sprite> rooms;

    private Coroutine gameLoopRoutine = null;
    
    private void OnEnable()
    {
        if (!IsSpawned)
            GetComponent<NetworkObject>().Spawn();
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (PlayerInputs.CheckForResetObjective())
        {
            if (gameLoopRoutine != null)
                StopAllCoroutines();
            gameLoopRoutine = StartCoroutine(MiniGameLoop());
        }
    }

    private IEnumerator MiniGameLoop()
    {
        int count = 0;
        while (count < 5)
        {
            ChooseItemAndRoom();
            Timer.OnTriggerTimer?.Invoke(15.0f, false);
            yield return new WaitForSeconds(15.5f);
            Timer.OnTriggerTimer?.Invoke(10f, true);
            yield return new WaitForSeconds(10.5f);
            count += 1;
        }
    }

    private void ChooseItemAndRoom()
    {
        List<int> itemIndexes = new List<int>();
        for (int i = 0; i < items.Count; i++)
            itemIndexes.Add(i);
        
        List<int> roomIndexes = new List<int>();
        for (int i = 0; i < rooms.Count; i++)
            roomIndexes.Add(i);

        List<int> selectedItems = itemIndexes.GetRandomElements(3);
        
        List<int> selectedRooms = new List<int>();
        for (int i = 0; i < 3; i++)
            selectedRooms.Add(roomIndexes.GetRandomElements(1).First());

        Debug.Log("Zuzu ChooseItemAndRoom");
        
        UpdateSpritesRpc(
            selectedItems[0],
            selectedRooms[0],
            selectedItems[1],
            selectedRooms[1],
            selectedItems[2],
            selectedRooms[2]
        );
    }

    [Rpc(SendTo.Everyone)]
    public void UpdateSpritesRpc(int firstItemIndex, int firstRoomIndex, int secondItemIndex, int secondRoomIndex, int thirdItemIndex, int thirdRoomIndex)
    {
        Debug.Log("Zuzu UpdateSpritesRpc");

        Transform parent = transform.GetChild(0);
        parent.GetChild(1).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = items[firstItemIndex];
        parent.GetChild(1).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = rooms[firstRoomIndex];
        parent.GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = items[secondItemIndex];
        parent.GetChild(2).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = rooms[secondRoomIndex];
        parent.GetChild(3).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = items[thirdItemIndex];
        parent.GetChild(3).GetChild(1).GetChild(0).GetComponent<SpriteRenderer>().sprite = rooms[thirdRoomIndex];
    }
}

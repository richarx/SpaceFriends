using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    [SerializeField] private Transform staticShip;
    [SerializeField] private Transform movingShip;

    private bool isTargetSet => player != null;
    private Transform player = null;

    public bool isFlying = false;
    
    private void Awake()
    {
        PlayerMovement.OnPlayerSpawn.AddListener((newPlayer) => player = newPlayer);
    }

    private void LateUpdate()
    {
        if (isTargetSet && !isFlying)
            FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector2 offset = player.position - staticShip.position;

        transform.position = movingShip.position + offset.ToVector3();
    }
}

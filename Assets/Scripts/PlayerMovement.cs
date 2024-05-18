using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public Vector2 MoveDirection => direction;
    private Vector2 direction;

    void Update()
    {
        if (!IsOwner)
            return;
        
        direction = Vector2.zero;

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
            direction.y = 1.0f;
        if (Input.GetKey(KeyCode.S))
            direction.y = -1.0f;
        if (Input.GetKey(KeyCode.D))
            direction.x = 1.0f;
        if (Input.GetKey(KeyCode.A))
            direction.x = -1.0f;

        float speed = 3.0f;

        transform.position += (Vector3)direction.normalized * (speed * Time.deltaTime);
    }
}

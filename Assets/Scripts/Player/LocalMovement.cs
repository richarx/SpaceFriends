using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMovement : MonoBehaviour
{
    private Rigidbody2D attachedRigidbody;

    private float speed = 6.0f;
    private Vector2 velocity = Vector2.zero;
    
    private void Awake()
    {
        attachedRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (SpawnPosition.Instance != null)
            transform.position = SpawnPosition.Instance.GetSpawnPosition();
    }
    
    private void Update()
    {
        Vector2 inputDirection = PlayerInputs.ComputeInputDirection();
        velocity = inputDirection.normalized;
    }
    
    private void FixedUpdate()
    {
        if (velocity.magnitude <= 0.15f)
            return;
        
        Vector2 newPosition = (Vector2)transform.position + (velocity * (speed * Time.fixedDeltaTime));

        attachedRigidbody.MovePosition(newPosition);
    }
}

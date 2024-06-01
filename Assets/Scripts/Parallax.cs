using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private float speed;

    private Vector2 previousPosition;

    private void Start()
    {
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector2 direction = transform.position.ToVector2() - previousPosition;
        
        image.uvRect = new Rect(image.uvRect.position + direction * (speed * Time.fixedDeltaTime), image.uvRect.size);

        previousPosition = transform.position;
    }
}

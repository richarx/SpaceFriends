using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenTooFar : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.magnitude >= 1000)
            Destroy(gameObject);
    }
}

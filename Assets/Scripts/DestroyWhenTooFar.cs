using UnityEngine;

public class DestroyWhenTooFar : MonoBehaviour
{
    private Vector2 startingPosition = Vector2.zero;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (startingPosition.Distance(transform.position.ToVector2()) >= 100.0f)
        {
            transform.position = startingPosition.ToVector3();
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}

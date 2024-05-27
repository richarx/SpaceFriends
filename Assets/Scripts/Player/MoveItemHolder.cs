using UnityEngine;

public class MoveItemHolder : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    private float vertical;
    private float horizontal;

    private void Start()
    {
        Vector3 localPosition = transform.localPosition;
        vertical = localPosition.y;
        horizontal = localPosition.x;
    }

    private void LateUpdate()
    {
        Vector2 moveDirection = playerMovement.MoveDirection;
        
        if (moveDirection.magnitude < 0.5f)
            return;

        Vector2 holderDirection = ComputeHolderDirection(moveDirection);

        float horizontalPosition = horizontal * (holderDirection == Vector2.left ? 1.0f : -1.0f);

        transform.localPosition = new Vector3(horizontalPosition, vertical, 0.0f);

        transform.localScale = new Vector3(holderDirection == Vector2.left ? 1.0f : -1.0f, 1.0f, 1.0f);
    }
    
    private static Vector2 ComputeHolderDirection(Vector2 direction)
    {
        if (direction.x > 0.5f && direction.y > 0.5f)
            return Vector2.right;
        if (direction.x > 0.5f && direction.y < -0.5f)
            return Vector2.right;
        if (direction.x > 0.5f && direction.y < 0.5f && direction.y > -0.5f)
            return Vector2.right;
        if (direction.x < -0.5f && direction.y > 0.5f)
            return Vector2.left;
        if (direction.x < -0.5f && direction.y < -0.5f)
            return Vector2.left;
        if (direction.x < -0.5f && direction.y < 0.5f && direction.y > -0.5f)
            return Vector2.left;
        if (direction.x < 0.5f && direction.x > -0.5f && direction.y > 0.5f)
            return Vector2.left;
        if (direction.x < 0.5f && direction.x > -0.5f && direction.y < -0.5f)
            return Vector2.right;

        return Vector2.right;
    }
}

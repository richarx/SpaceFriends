using Unity.Netcode;
using UnityEngine;

public class AimHandler : NetworkBehaviour
{
    [SerializeField] private GameObject cursor;
    
    private Camera camera;
    private ItemHandler itemHandler;

    private Vector2 previousDirection = Vector2.right;

    public bool isAiming => cursor.activeSelf;
    
    private void Start()
    {
        if (!IsOwner)
            return;
        
        itemHandler = GetComponent<ItemHandler>();
        camera = FindObjectOfType<Camera>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (itemHandler.IsItemThrowable && PlayerInputs.CheckForAim())
        {
            SetCursorState(true);
            Vector2 direction = ComputeDirection();
            SetCursorDirection(direction);
        
            if (PlayerInputs.CheckForThrowItem())
            {
                Debug.Log("Zuzu : CheckForThrowItem");
                itemHandler.ThrowItem(direction);
            }
        }
        else
            SetCursorState(false);
    }

    private Vector2 ComputeDirection()
    {
        Vector2 direction = PlayerInputs.ComputeAimDirection(cursor.transform.position, camera);

        if (direction == Vector2.zero)
            return previousDirection;

        previousDirection = direction;
        return direction;
    }

    private void SetCursorDirection(Vector2 direction)
    {
        cursor.transform.rotation = direction.ToRotation();
    }

    private void SetCursorState(bool state)
    {
        cursor.SetActive(state);
    }
}

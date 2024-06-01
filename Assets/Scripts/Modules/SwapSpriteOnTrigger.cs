using UnityEngine;

public class SwapSpriteOnTrigger : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite selectedSprite;

    private SpriteRenderer spriteRenderer;
    
    private bool isIdle = true;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isIdle)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isIdle)
            return;
        
        if (other.CompareTag("Player") && other.transform.parent.GetComponent<PlayerMovement>().IsOwner)
            SetSpriteState(true);
    }

    private void SetSpriteState(bool state)
    {
        isIdle = state;
        spriteRenderer.sprite = isIdle ? idleSprite : selectedSprite;
    }
}

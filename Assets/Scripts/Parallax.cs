using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private float speed;

    private Material material;

    private ParallaxFollowTarget parallaxHandler;
    private Vector2 currentVelocity => parallaxHandler.CurrentVelocity;

    private void Start()
    {
        material = image.GetComponent<MeshRenderer>().material;
        parallaxHandler = GetComponent<ParallaxFollowTarget>();
    }

    private void LateUpdate()
    {
        if (parallaxHandler.shipRigidbody == null || parallaxHandler.playerRigidbody == null)
            return;
        
        Vector2 offset = material.mainTextureOffset;
        
        offset += currentVelocity * (speed * Time.deltaTime);

        material.mainTextureOffset = offset;
    }
}

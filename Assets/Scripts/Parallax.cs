using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private float speed;

    private Material material;

    private ParallaxFollowTarget ParallaxHandler;
    private Vector2 currentVelocity => ParallaxHandler.CurrentVelocity;

    private void Start()
    {
        material = image.GetComponent<MeshRenderer>().material;
        ParallaxHandler = GetComponent<ParallaxFollowTarget>();
    }

    private void LateUpdate()
    {
        Vector2 offset = material.mainTextureOffset;
        
        offset += currentVelocity * (speed * Time.deltaTime);

        material.mainTextureOffset = offset;
    }
}

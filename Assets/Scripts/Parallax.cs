using UnityEngine;
using UnityEngine.UI;

public class Parallax : MonoBehaviour
{
    [SerializeField] private GameObject image;
    [SerializeField] private float speed;

    private Material material;
    private Vector2 previousPosition;

    private void Start()
    {
        material = image.GetComponent<MeshRenderer>().material;
        previousPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 direction = transform.position.ToVector2() - previousPosition;

        Vector2 offset = material.mainTextureOffset;

        offset += direction * (speed * Time.deltaTime);

        material.mainTextureOffset = offset;

        previousPosition = transform.position;
    }
}

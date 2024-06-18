using System.Collections;
using UnityEngine;

//TODO
// Delay ping based on distance
// The beacon cannot be used inside the spaceship
// Add spaceship ping back - but not when in the ship

public class DisplayPing : MonoBehaviour
{
    [SerializeField] private Transform centerPosition;
    [SerializeField] private GameObject arrow;
    [SerializeField] private GameObject beacon;
    [SerializeField] private GameObject ship;

    private Coroutine displayThenHide;

    public float arrowDistance;
    public float iconDistance;
    
    public void ComputeAndDisplayPing(Vector2 position, bool isBeacon)
    {
        Debug.Log($"You got pinged the position : {position} / {isBeacon}");

        if (displayThenHide != null)
            StopAllCoroutines();

        displayThenHide = StartCoroutine(DisplayThenHide(position, isBeacon));
    }

    private IEnumerator DisplayThenHide(Vector2 position, bool isBeacon)
    {
        arrow.SetActive(true);
        beacon.SetActive(isBeacon);
        ship.SetActive(!isBeacon);

        Vector2 center = centerPosition.position;
        
        Vector2 direction = (position - center).normalized;
        arrow.transform.position = center + direction * arrowDistance;
        arrow.transform.rotation = direction.AddAngleToDirection(-90).ToRotation();

        if (isBeacon)
            beacon.transform.position = center + direction * iconDistance;
        else
            ship.transform.position = center + direction * iconDistance;

        yield return new WaitForSeconds(1.5f);
        
        arrow.SetActive(false);
        beacon.SetActive(false);
        ship.SetActive(false);
    }
}

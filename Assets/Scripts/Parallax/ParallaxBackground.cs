using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [Range(0f, 2f)]
    public float parallaxFactor = 0.5f;

    private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    private float fixedY; 

    void Start()
    {
        cameraTransform = Camera.main.transform;

        previousCameraPosition = cameraTransform.position;

        fixedY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaX = cameraTransform.position.x - previousCameraPosition.x;

        float newX = transform.position.x + deltaX * parallaxFactor;

        transform.position = new Vector3(newX, fixedY, transform.position.z);

        previousCameraPosition = cameraTransform.position;
    }
}

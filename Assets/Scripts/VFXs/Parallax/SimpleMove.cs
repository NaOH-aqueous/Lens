using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    
    public float speed = -1.5f; //Left movement speed of the object
    
    private SpriteRenderer spriteRenderer;
    private float spriteWidth;
    private float startX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Get the width of the sprite in world units
        spriteWidth = spriteRenderer.bounds.size.x;
        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the object to the right at the specified speed
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        float distanceMoved = transform.position.x - startX;

        if (distanceMoved < -spriteWidth)
        {
           transform.position = new Vector3(startX + spriteWidth, transform.position.y, transform.position.z);
        }
        else if (distanceMoved > spriteWidth)
        {
            transform.position = new Vector3(startX - spriteWidth, transform.position.y, transform.position.z);
        }
    }
}

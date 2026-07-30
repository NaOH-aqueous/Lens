using UnityEngine;

public class CloudAutoLoop : MonoBehaviour
{

    public GameObject cloudPrefab;

    public float speed = -0.8f;

    public int copyCount = 3;

    private GameObject[] clouds;
    private float spriteWidth;
    private float startX;

    void Start()
    {
        SpriteRenderer templateSR = cloudPrefab.GetComponent<SpriteRenderer>();
        spriteWidth = templateSR.bounds.size.x;

        clouds = new GameObject[copyCount];
        for (int i = 0; i < copyCount; i++)
        {
            
            GameObject newCloud = Instantiate(cloudPrefab, transform);

            // Set the position of the new cloud based on its index and the width of the sprite
            float xPos = transform.position.x + i * spriteWidth;
            newCloud.transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
            clouds[i] = newCloud;
        }

        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        float offset = transform.position.x - startX;

        if (offset < -spriteWidth)
        {
            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        }
        else if (offset > spriteWidth)
        {
            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        }
    }
}

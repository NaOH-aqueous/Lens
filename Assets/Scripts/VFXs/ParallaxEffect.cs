using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public Transform player;
    public float parallaxEffect;   // 0 = no movement, 1 = same as player

    private float startPos;
    private float length;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distance = player.position.x * parallaxEffect;
        float movement = player.position.x * (1 - parallaxEffect);

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}

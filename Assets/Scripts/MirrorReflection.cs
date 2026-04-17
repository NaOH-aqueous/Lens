using UnityEngine;

public class MirrorReflection : MonoBehaviour
{
    [SerializeField] private float fadingDistance = 2f;
    private Vector3 initialPos;

    private Animator playerAnimator;
    private Animator mirrorAnimator;

    private Transform player_transform;

    private void Start()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        mirrorAnimator = GetComponent<Animator>();

        player_transform = GameObject.Find("Player").GetComponent<Transform>();
        initialPos = transform.position;
    }
    void LateUpdate()
    {
        float distance = Vector3.Distance(initialPos,
            player_transform.transform.position);

        if (distance > fadingDistance)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        else
        {
            ReflectPlayerAnimator();
            SpriteTransparency(distance);
        }
    }

    private void ReflectPlayerAnimator()
    {
        GetComponent<SpriteRenderer>().enabled = true;

        // Move the x pos of mirror as player moves
        transform.position = new Vector2(playerAnimator.transform.position.x,
            initialPos.y);

        // get the coordinators from player animator
        float moveX = playerAnimator.GetFloat("MoveX");
        float moveY = playerAnimator.GetFloat("MoveY");

        // Mirror the Y direction
        mirrorAnimator.SetFloat("MoveX", moveX);
        mirrorAnimator.SetFloat("MoveY", -moveY);
    }

    private void SpriteTransparency(float distance)
    {
        // Lower transparency as player move away
        float t = Mathf.Clamp01(distance / fadingDistance);
        float alpha = Mathf.Lerp(1f, 0, t);
        // Apply alpha to the sprite
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = alpha;
        GetComponent<SpriteRenderer>().color = c;
    }
}

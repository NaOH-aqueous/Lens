using TMPro;
using UnityEngine;

public class MirrorReflection : MonoBehaviour
{
    public float fadingDistance = 2f;
    private Vector3 initialPos;

    private Animator playerAnimator;
    private Animator mirrorAnimator;

    private Transform player_transform;
    private Transform mirror_transform;

    private void Start()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        mirrorAnimator = GetComponent<Animator>();

        player_transform = GameObject.Find("Player").GetComponent<Transform>();
        mirror_transform = GetComponentInParent<Transform>();
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
            GetComponent<SpriteRenderer>().enabled = true;
            // Mirror across the mirror (invert Y for top-down mirror)
            transform.position = new Vector2(playerAnimator.transform.position.x,
                initialPos.y); 

            float moveX = playerAnimator.GetFloat("MoveX");
            float moveY = playerAnimator.GetFloat("MoveY");

            // Mirror the direction
            mirrorAnimator.SetFloat("MoveX", moveX);
            mirrorAnimator.SetFloat("MoveY", -moveY);

            // Invert so closer = more visible
            float t = Mathf.Clamp01(distance / fadingDistance);
            float alpha = Mathf.Lerp(1f, 0, t);

            // Apply alpha
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = alpha;
            GetComponent<SpriteRenderer>().color = c;
        }

    }
}

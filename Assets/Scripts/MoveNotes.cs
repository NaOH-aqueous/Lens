using UnityEngine;

public class MoveNotes : MonoBehaviour
{
    // Speed at which the notes move
    public float moveSpeed = 100f;

    // Reference to the RectTransform component of the notes
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            Debug.LogError("MoveNotes script requires a RectTransform component.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rectTransform == null) return;

        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move.y = -moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move.y = moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            move.x = -moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move.x = moveSpeed * Time.deltaTime;
        }

        rectTransform.anchoredPosition += move;
    }
}

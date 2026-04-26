using UnityEngine;

public class MoveNotes : MonoBehaviour
{
    // Speed at which the notes move
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private float scaleSpeed = 0.5f;
    [SerializeField] private float minScale = 0.1f;
    [SerializeField] private float maxScale = 5f;

    // Store the original scale of the notes for resetting
    private Vector3 originalScale;

    // Reference to the RectTransform component of the notes
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = transform.localScale;

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

        // Handle scaling with Shift + Up/Down arrows
        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld)
        {
            float scaleChange = 0f;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                scaleChange = scaleSpeed * Time.deltaTime; // Scale up
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                scaleChange = -scaleSpeed * Time.deltaTime; // Scale down
            }

            if (scaleChange != 0f)
            {
                float newScale = rectTransform.localScale.x + scaleChange;

                // Ensure the new scale is within the defined limits
                newScale = Mathf.Clamp(newScale, minScale, maxScale);

                // Update the original scale to the new scale for consistent scaling
                rectTransform.localScale = new Vector3(newScale, newScale, 1f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTransform();
        }

    }

    void ResetTransform()
    {
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localScale = originalScale;
    }

}

using UnityEngine;

public class MoveItem : MonoBehaviour
{
    // Speed at which the notes move
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private float scaleSpeed = 0.1f;
    private float minScale = 1f;
    private float maxScale = 1.5f;
    public float transformLimitX = 100f;
    public float transformLimitY = 100f;

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

        ScaleTransform();

        Vector2 move = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move.y = -moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move.y = moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            move.x = -moveSpeed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move.x = moveSpeed * Time.unscaledDeltaTime;
        }

        Vector3 newTransform =  rectTransform.anchoredPosition += move;

        float currentScale = rectTransform.localScale.x;

        newTransform.x = Mathf.Clamp(newTransform.x, -transformLimitX, transformLimitX);
        newTransform.y = Mathf.Clamp(newTransform.y, -transformLimitY, transformLimitY);
        rectTransform.anchoredPosition = newTransform;



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

    void ScaleTransform()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            if (rectTransform.localScale.x == maxScale)
            {
                return;
            }
            float newScale = rectTransform.localScale.x + scaleSpeed;
            newScale = Mathf.Clamp(newScale, minScale, maxScale);
            rectTransform.localScale = new Vector3(newScale, newScale, 1f);
            transformLimitX += 200f;
            transformLimitY += 200f;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (rectTransform.localScale.x == minScale)
            {
                return;
            }
            float newScale = rectTransform.localScale.x - scaleSpeed;
            newScale = Mathf.Clamp(newScale, minScale, maxScale);
            rectTransform.localScale = new Vector3(newScale, newScale, 1f);
            transformLimitX -= 200f;
            transformLimitY -= 200f;
        }
    }

}

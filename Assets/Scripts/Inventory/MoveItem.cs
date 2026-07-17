using UnityEngine;
using UnityEngine.InputSystem;

public class MoveItem : MonoBehaviour
{
    // Speed at which the notes move
    [SerializeField] private float moveSpeed = 200f;
    [SerializeField] private float scaleSpeed = 0.1f;
    public float transformLimitX = 100f;
    public float transformLimitY = 100f;

    private Vector3 originalScale;

    private RectTransform rectTransform;
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

    void Update()
    {
        if (rectTransform == null) return;

        Vector2 move = -InputManager.Instance.GetNavigation();

        Vector3 newTransform =  rectTransform.anchoredPosition += move;

        float currentScale = rectTransform.localScale.x;

        newTransform.x = Mathf.Clamp(newTransform.x, -transformLimitX, transformLimitX);
        newTransform.y = Mathf.Clamp(newTransform.y, -transformLimitY, transformLimitY);
        rectTransform.anchoredPosition = newTransform;

    }
}

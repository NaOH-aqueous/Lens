using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class LensControl : MonoBehaviour
{
    public float moveSpeed = 4f;
    public RectTransform canvasRect;

    private InputAction navigateAction;
    private RectTransform lensPos;

    private void Awake()
    {
        navigateAction = InputSystem.actions.FindAction("Navigate");
        lensPos = GetComponent<RectTransform>();
    }

    private void Update()
    {
        MoveLens();
        ClamptoScreen();
    }

    private void MoveLens()
    {
        //move the lens according to the input and given speed
        Vector2 moveValue = navigateAction.ReadValue<Vector2>();
        lensPos.anchoredPosition += moveValue * moveSpeed * Time.deltaTime;
    }

    //make sure the position of the lens is limited to the screen
    private void ClamptoScreen()
    {
        Vector2 pos = lensPos.anchoredPosition;

        float halfWidth = lensPos.rect.width * 0.5f;
        float halfHeight = lensPos.rect.height * 0.5f;

        float minX = -canvasRect.rect.width * 0.5f + halfWidth;
        float maxX = canvasRect.rect.width * 0.5f - halfWidth;

        float minY = -canvasRect.rect.height * 0.5f + halfHeight;
        float maxY = canvasRect.rect.height * 0.5f - halfHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        lensPos.anchoredPosition = pos;
    }
}

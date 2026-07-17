using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LensControl : MonoBehaviour
{
    public float moveSpeed = 4f;
    public RectTransform canvasRect;
    public Item testItem;

    private RectTransform lensPos;
    public float requiredStayTime = 1f;

    private LensInteractableItem currentLensItem;
    private Item activeItem;
    private Coroutine stayCoroutine;

    private void Awake()
    {
        lensPos = GetComponent<RectTransform>();
        SetActiveItem(testItem);
    }

    private void Update()
    {
        MoveLens();
        ClamptoScreen();
        CheckTouchItem();
    }

    public void SetActiveItem(Item item)
    {
        if (item != null)
        {
            activeItem = item;
        }
    }

    private void MoveLens()
    {
        //move the lens according to the input and given speed
        Vector2 moveValue = InputManager.Instance.GetNavigation();
        lensPos.anchoredPosition += moveValue * moveSpeed * Time.unscaledDeltaTime;
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
    
    public Item InteractLensItem()
    {
        if(currentLensItem == null)
        {
            return null;
        }
        if (currentLensItem.LensItem == null)
        {
            return null;
        }
        else
        {
            Debug.Log("Interacted with: " + currentLensItem.LensItem.item_Name);
            return currentLensItem.LensItem;
        }
    }

    private bool IsInsideLens(RectTransform item)
    {
        Vector2 lensCenter = lensPos.position;
        Vector2 itemCenter = item.position;

        float lensRadius = lensPos.rect.width * 0.5f;

        return Vector2.Distance(lensCenter, itemCenter) <= lensRadius;
    }

    private void CheckTouchItem()
    {
        Rect lensRectWorld = GetWorldRect(lensPos);

        LensInteractableItem newItem = null;

        foreach (LensInteractableItem interactable in LensInteractableItem.All)
        {
            if (interactable == null)
                continue;

            RectTransform itemRect = interactable.GetComponent<RectTransform>();

            if (itemRect == null)
                continue;

            Rect itemWorldRect = GetWorldRect(itemRect);

            if (lensRectWorld.Overlaps(itemWorldRect))
            {
                newItem = interactable;
                break;
            }
        }

        currentLensItem = newItem;
    }

    private IEnumerator StayAndLog(LensInteractableItem item)
    {
        yield return new WaitForSeconds(requiredStayTime);

        if (item != null && item.LensItem != null)
        {
            Debug.Log("Stayed on item: " + item.LensItem.item_Name);
        }

        stayCoroutine = null;
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];

        rectTransform.GetWorldCorners(corners);
        float minX = corners[0].x;
        float maxX = corners[2].x;
        float minY = corners[0].y;
        float maxY = corners[2].y;

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

}

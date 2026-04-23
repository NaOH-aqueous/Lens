using System.Collections;
using System.Collections.Generic;
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
    public float requiredStayTime = 1f; // 需要停留的时间（秒）

    private List<RectTransform> itemRects = new List<RectTransform>();

    private List<string> itemNames = new List<string>();   // 对应物品名称

    private RectTransform currentItem;
    private Coroutine stayCoroutine;

    private void Awake()
    {
        navigateAction = InputSystem.actions.FindAction("Navigate");
        lensPos = GetComponent<RectTransform>();
    }

    private void Start()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        for (int i = 0; i < items.Length; i++)
        {
            GameObject item = items[i];
            RectTransform rt = item.GetComponent<RectTransform>();
            if (rt != null)
            {
                itemRects.Add(rt);
                itemNames.Add(item.name); // 存储物品名称
            }
        }
    }

    private void Update()
    {
        MoveLens();
        ClamptoScreen();
        CheckTouchItem();
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
    
    private void CheckTouchItem()
    {
        // 获取放大镜的世界矩形
        Rect lensRectWorld = GetWorldRect(lensPos);
        RectTransform newItem = null;

        // 寻找第一个重叠的物品
        for (int i = 0; i < itemRects.Count; i++)
        {
            RectTransform itemRect = itemRects[i];

            if(itemRect == null) continue; // 如果物品被销毁，跳过
            // 获取物品的世界矩形
            Rect itemRectWorld = GetWorldRect(itemRect);
            // 判断是否重叠
            if (lensRectWorld.Overlaps(itemRectWorld))
            {
                // 找到重叠的物品，记录下来
                newItem = itemRect;
                break;
            }
        }

        // 如果当前重叠的物品发生了变化
        if (newItem != currentItem)
        {
            // 停止之前的计时协程
            if (stayCoroutine != null)
            {
                StopCoroutine(stayCoroutine);
                stayCoroutine = null;
            }
            currentItem = newItem;

            // 如果新的物品存在，开始计时
            if (currentItem != null)
            {
                stayCoroutine = StartCoroutine(StayAndLog(currentItem));
            }
        }
    }
    
    private IEnumerator StayAndLog(RectTransform item)
    {
        yield return new WaitForSeconds(requiredStayTime);
        // 停留时间足够，输出日志
        if (item != null)
        {
            string itemName = "";

            // 根据当前物品的RectTransform找到对应的名称
            for (int i = 0; i < itemRects.Count; i++)
            {
                if (itemRects[i] == item)
                {
                    itemName = itemNames[i];
                    break;
                }
            }
            Debug.Log("停留触发物品：" + itemName);
        }
        stayCoroutine = null;
    }

    // 获取RectTransform的世界矩形
    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];// 获取四个角的世界坐标

        // 获取四个角的世界坐标
        rectTransform.GetWorldCorners(corners);
        float minX = corners[0].x;
        float maxX = corners[2].x;// 角点的顺序是：0-左下，1-左上，2-右上，3-右下
        float minY = corners[0].y;// 因此，minX和minY来自左下角，maxX和maxY来自右上角
        float maxY = corners[2].y;// 计算矩形的宽度和高度，并返回一个新的Rect

        // 这个函数将RectTransform转换为一个以世界坐标为基础的矩形，方便进行重叠检测
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookContents : MonoBehaviour
{
    [TextArea(10, 20)]
    [SerializeField] private string contents;

    [TextArea(10, 20)]
    [SerializeField] private string newContents;

    [SerializeField] private TMP_Text leftSide; 
    [SerializeField] private TMP_Text rightSide; 

    [SerializeField] private TMP_Text leftPagination; 
    [SerializeField] private TMP_Text rightPagination;

    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    [SerializeField] private List<GameObject> stickers;
    [SerializeField] private TextAsset diaryBlank;
    [SerializeField] private TextAsset diaryFilled;
    [SerializeField] private TextAsset origamiTutorial;

    private GameObject lastActiveSticker;
    private bool hasWritten = false;
    private bool submitSubscribed;

    private void OnValidate()
    {
        UpdatePagination();

        leftArrow.enabled = false;

        if (leftSide.text == contents)
            return; 

        SetupContent();
    }

    private void Awake()
    {
        SetupContent(); 
        UpdatePagination();
        InitStickers();
    }

    private void OnEnable()
    {
        SubscribeSubmit();
    }

    private void OnDisable()
    {
        UnsubscribeSubmit();
    }

    private void OnDestroy()
    {
        UnsubscribeSubmit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextPage();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousPage();
        }

        // submit handling moved to event handler (SubscribeSubmit)
    }

    private void SubscribeSubmit()
    {
        if (submitSubscribed)
            return;

        if (InputManager.Instance == null)
            return;

        // Defensive unsubscribe then subscribe to avoid duplicates
        InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        InputManager.Instance.SubmitPerformed += OnSubmitPerformed;
        submitSubscribed = true;
    }

    private void UnsubscribeSubmit()
    {
        if (!submitSubscribed)
            return;

        if (InputManager.Instance != null)
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;

        submitSubscribed = false;
    }

    private void OnSubmitPerformed()
    {
        // preserve original guard: only investigate when no dialogue playing
        if (DialogueManager.Instance != null && DialogueManager.Instance.CheckDialoguePlaying())
            return;

        Investigate();
    }

    private void SetupContent()
    {
        leftSide.text = contents; // Set the left side text
        rightSide.text = contents; // Set the right side text
    }

    private void UpdatePagination()
    {
        leftPagination.text = leftSide.pageToDisplay.ToString(); // Set the left page number based on the current page of the left side.
        rightPagination.text = rightSide.pageToDisplay.ToString(); // Set the right page number based on the current page of the right side.
    }

    public void PreviousPage()
    {
        rightArrow.enabled = true;

        if (leftSide.pageToDisplay < 1)
        {
            leftSide.pageToDisplay = 1;
            leftArrow.enabled = false;
            return;
        }

        if (leftSide.pageToDisplay -2 > 1)
        {
            leftArrow.enabled = true;
            leftSide.pageToDisplay -= 2;
        }
        else
        {
            leftSide.pageToDisplay = 1;
            leftArrow.enabled = false;
        }
        
        rightSide.pageToDisplay = leftSide.pageToDisplay + 1; // Set the right page number to be one more than the left page number.

        UpdatePagination(); // Update the pagination after flipping pages
        SetStickers();
    }

    public void NextPage()
    {
        if (rightSide.pageToDisplay == rightSide.textInfo.pageCount - 2)
        {
            Debug.Log("rightside: " + rightSide.pageToDisplay + "pagecount: " + rightSide.textInfo.pageCount);
            rightArrow.enabled = false;
        }

        if (rightSide.pageToDisplay >= rightSide.textInfo.pageCount)
        {
            return;
        } 

        if (leftSide.pageToDisplay >= leftSide.textInfo.pageCount - 1)
        {
            //if the current page of left side is the last page,
            //clamp the leftside display page as well as the right side page display.
            leftArrow.enabled = true;
            rightArrow.enabled = false;
            leftSide.pageToDisplay = leftSide.textInfo.pageCount - 1;
            rightSide.pageToDisplay = leftSide.pageToDisplay + 1;
        }
        else
        {
            //otherwise, do not clamp it
            leftArrow.enabled = true;
            leftSide.pageToDisplay += 2;
            rightSide.pageToDisplay = leftSide.pageToDisplay + 1;
        }
        
        UpdatePagination(); // Update the pagination after changing the page numbers.
        SetStickers();
    }

    private void InitStickers()
    {
        foreach (GameObject sticker in stickers)
        {
            sticker.SetActive(false);
        }

        if (stickers.Count > 0)
        {
            stickers[0].SetActive(true);
            lastActiveSticker = stickers[0];
        }
    }

    private void SetStickers()
    {
        if (lastActiveSticker != null)
        {
            lastActiveSticker.SetActive(false);
        }
        int currentSticker = rightSide.pageToDisplay / 2 - 1;
        if (currentSticker >= 0 && currentSticker < stickers.Count)
        {
            stickers[currentSticker].SetActive(true);
            lastActiveSticker = stickers[currentSticker];
        }
    }

    public void WriteNewDiary()
    {
        if (contents != newContents)
        {
            contents = newContents;
            hasWritten = true;
        }
        SetupContent();
    }

    private void Investigate()
    {
        if(leftSide.pageToDisplay != 9 && leftSide.pageToDisplay != 7)
        {
            return;
        }

        if(leftSide.pageToDisplay == 9)
        {
            if (!hasWritten)
            {
                DialogueManager.Instance.NewStory(diaryBlank);
            }
            else
            {
                DialogueManager.Instance.NewStory(diaryFilled);
            }
        }
        else
        {
            DialogueManager.Instance.NewStory(origamiTutorial);
        }
    }

}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BookContents : MonoBehaviour
{
    [TextArea(10, 20)] 
    [SerializeField]private string contents;

    [SerializeField] private TMP_Text leftSide; // Reference to the TextMeshPro component for the left side of the book.
    [SerializeField] private TMP_Text rightSide; // Reference to the TextMeshPro component for the right side of the book.

    [SerializeField] private TMP_Text leftPagination; // Reference to the TextMeshPro component for the left page number.
    [SerializeField] private TMP_Text rightPagination; // Reference to the TextMeshPro component for the right page number.

    [SerializeField] private Image leftArrow;
    [SerializeField] private Image rightArrow;

    private void OnValidate()
    {
        UpdatePagination();

        leftArrow.enabled = false;

        if (leftSide.text == contents)
            return; 

        SetupContent(); // Set up the content of the book whenever the script is validated in the Unity Editor.
    }

    private void Awake()
    {
        SetupContent(); // Set up the content of the book when the script awakes.
        UpdatePagination(); // Update the pagination when the script awakes.
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
    }

    private void SetupContent()
    {
        leftSide.text = contents; // Set the left side text to the contents of the book.
        rightSide.text = contents; // Set the right side text to the contents of the book.
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

        UpdatePagination(); // Update the pagination after changing the page numbers.
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
    }
}

using TMPro;
using UnityEngine;

public class BookContents : MonoBehaviour
{
    [TextArea(10, 20)] // This attribute allows you to edit the string in a larger text area in the Unity Inspector.
    [SerializeField]private string contents; // This field will hold the contents of the book.

    [SerializeField] private TMP_Text leftSide; // Reference to the TextMeshPro component for the left side of the book.
    [SerializeField] private TMP_Text rightSide; // Reference to the TextMeshPro component for the right side of the book.

    [SerializeField] private TMP_Text leftPagination; // Reference to the TextMeshPro component for the left page number.
    [SerializeField] private TMP_Text rightPagination; // Reference to the TextMeshPro component for the right page number.


    private void OnValidate()
    {
        UpdatePagination(); // Update the pagination whenever the script is validated in the Unity Editor.

        if (leftSide.text == contents)
            return; // If the left side text is already set to the contents, do nothing.

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
        if (leftSide.pageToDisplay < 1)
        {
            leftSide.pageToDisplay = 1; // Ensure the left page number does not go below 1.
            return;
        }

        if (leftSide.pageToDisplay -2 > 1)
        {
            leftSide.pageToDisplay -= 2;
        }
        else
        {
            leftSide.pageToDisplay = 1; // Set the left page number to 1 if it goes below 1.
        }
        
        rightSide.pageToDisplay = leftSide.pageToDisplay + 1; // Set the right page number to be one more than the left page number.

        UpdatePagination(); // Update the pagination after changing the page numbers.
    }

    public void NextPage()
    {
        if (rightSide.pageToDisplay >= rightSide.textInfo.pageCount)
            return;

        if (leftSide.pageToDisplay >= leftSide.textInfo.pageCount - 1)
        {
            leftSide.pageToDisplay = leftSide.textInfo.pageCount - 1; // Set the left page number to the total page count minus one if it exceeds it.
            rightSide.pageToDisplay = leftSide.pageToDisplay + 1; // Set the right page number to be one more than the left page number.
        }
        else
        {
            leftSide.pageToDisplay += 2; // Increment the left page number by 2 to move to the next set of pages.
            rightSide.pageToDisplay = leftSide.pageToDisplay + 1; // Set the right page number to be one more than the left page number.
        }
        
        UpdatePagination(); // Update the pagination after changing the page numbers.
    }
}

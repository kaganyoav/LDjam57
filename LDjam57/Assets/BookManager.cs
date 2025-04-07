using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookManager : MonoBehaviour
{
    public int currentPage = 0;
    public Button nextButton;
    public Button previousButton;
    
    [SerializeField] private List<Sprite> bookImages;
    public Image currentPageImage;

    private void Start()
    {
        currentPage = 0;
        UpdatePage();
    }

    public void NextPage()
    {
        if (currentPage < bookImages.Count - 1)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bookFlipNext, transform.position); 
            currentPage++;
        }

        UpdatePage();
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.bookFlipPrev, transform.position);
            currentPage--;
        }

        UpdatePage();
    }

    private void UpdatePage()
    {
        if (bookImages == null || bookImages.Count == 0)
        {
            Debug.LogWarning("No book pages assigned.");
            currentPageImage.sprite = null;
            previousButton.interactable = false;
            nextButton.interactable = false;
            return;
        }

        currentPageImage.sprite = bookImages[currentPage];
        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < bookImages.Count - 1;
    }
}
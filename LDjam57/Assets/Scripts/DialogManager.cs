using System.Collections;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private float letterDelay = 0.03f;

    private bool isDisplaying = false;

    private void Awake()
    {
        Instance = this;
        dialogBox.SetActive(false);
    }

    public void ShowLine(string line)
    {
        if (isDisplaying) return;
        StartCoroutine(DisplayLine(line));
    }

    private IEnumerator DisplayLine(string line)
    {
        isDisplaying = true;
        dialogBox.SetActive(true);
        dialogText.text = "";

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        dialogBox.SetActive(false);
        isDisplaying = false;
    }

    public bool IsDisplaying() => isDisplaying;
}
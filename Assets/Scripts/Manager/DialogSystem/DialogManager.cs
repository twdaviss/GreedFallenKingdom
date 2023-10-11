using UnityEngine;
using TMPro;

public class DialogManager : SingletonMonobehaviour<DialogManager>
{
    [SerializeField] private Transform dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;

    private string[] dialogLines;
    private int lineIndex;

    //===========================================================================
    private void Start()
    {
        SetDialogPanelActiveState(false);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (dialogPanel.gameObject.activeSelf == false)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            lineIndex++;

            if (lineIndex >= dialogLines.Length)
            {
                lineIndex = 0;

                SetDialogPanelActiveState(false);

                return;
            }

            dialogText.SetText(dialogLines[lineIndex]);
        }
    }

    //===========================================================================
    public void SetDialogPanelActiveState(bool newBool)
    {
        if (newBool == true)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;

        dialogPanel.gameObject.SetActive(newBool);
    }

    public void SetDialogLines(string[] newDialogLines)
    {
        dialogLines = newDialogLines;

        dialogText.SetText(dialogLines[lineIndex]);
    }
}

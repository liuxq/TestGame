using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public abstract class ModalBox : MonoBehaviour
{
    /// <summary>
    /// The title text object that will be used to show the title of the dialog.
    /// </summary>
    [Tooltip("The title text object that will be used to show the title of the dialog.")]
    public Text Title;

    /// <summary>
    /// The message text object that will be used to show the message of the dialog.
    /// </summary>
    [Tooltip("The message text object that will be used to show the message of the dialog.")]
    public Text Message;

    /// <summary>
    /// The botton object that will be used to interact with the dialog. This will be cloned to produce additional options.
    /// </summary>
    [Tooltip("The botton object that will be used to interact with the dialog. This will be cloned to produce additional options.")]
    public Button Button;

    /// <summary>
    /// The RectTransform of the panel that contains the frame of the dialog window. This is needed so that it can be centered correctly after it's size is adjusted to the dialogs contents.
    /// </summary>
    [Tooltip("The RectTransform of the panel that contains the frame of the dialog window. This is needed so that it can be centered correctly after it's size is adjusted to the dialogs contents.")]
    public RectTransform Panel;

    Transform buttonParent;
    bool isSetup;

    // This code has to be run here so that layout has already happened and preferredHeights have been calculated.
    void FixedUpdate()
    {
        if (!isSetup)
        {
            isSetup = true;

            if (Title != null)
            {
                var layoutElement = Title.GetComponent<LayoutElement>();
                layoutElement.minHeight = Title.preferredHeight; // Set the min height to the preferred height so that the parent dialog box vertical layout can resize correctly.
            }

            if (Message != null)
            {
                var layoutElement = Message.GetComponent<LayoutElement>();
                layoutElement.minHeight = Message.preferredHeight; // Set the min height to the preferred height so that the parent dialog box vertical layout can resize correctly.
                if (Message.preferredHeight < 20)
                    Message.alignment = TextAnchor.UpperCenter;
            }

            if (buttonParent != null)
            {
                // Set the min height of each button
                foreach (Transform button in buttonParent)
                {
                    var text = button.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        var buttonLayoutElement = button.GetComponent<LayoutElement>();
                        if (buttonLayoutElement != null)
                            buttonLayoutElement.minHeight = text.preferredHeight + 10;
                    }
                }
            }

            if (Panel != null)
            {
                // Center the panel to it's new height
                var group = Panel.GetComponent<VerticalLayoutGroup>();
                Panel.sizeDelta = new Vector2(Panel.sizeDelta.x, group.preferredHeight);
            }
        }
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public virtual void Close()
    {
        Destroy(gameObject);
    }

    protected void SetText(string message, string title)
    {
        if (Button != null)
            buttonParent = Button.transform.parent;

        if (Title != null)
        {
            if (!String.IsNullOrEmpty(title))
            {
                Title.text = MessageBox.LocalizeTitleAndMessage ? MessageBox.Localize(title) : title;
            }
            else
            {
                Destroy(Title.gameObject);
                Title = null;
            }
        }

        if (Message != null)
        {
            if (!String.IsNullOrEmpty(message))
            {
                Message.text = MessageBox.LocalizeTitleAndMessage ? MessageBox.Localize(message) : message;
            }
            else
            {
                Destroy(Message.gameObject);
                Message = null;
            }
        }
    }
}

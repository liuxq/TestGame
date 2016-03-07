using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Events;

public class MessageBox : ModalBox
{
    /// <summary>
    /// Set this to the name of the prefab that should be loaded when a menu box is shown.
    /// </summary>
    [Tooltip("Set this to the name of the prefab that should be loaded when a menu box is shown.")]
    public static string PrefabResourceName = "Message Box";

    /// <summary>
    /// Set this to a custom function that will be used to localize the button texts.
    /// </summary>
    /// <remarks>
    /// You can hook into your existing localization system here to lookup and return a translated string using the original as a key.
    /// For instance using "Localization package" from the asset store you would do this:
    /// MessageBox.Localize = (originalString) => { return Language.Get(originalString); };
    /// 
    /// Current text strings that need to be localized are "OK", "Yes", "No", "Cancel", "Abort", "Retry", "Ignore"
    /// 
    /// This function only needs to be set once at game startup.
    /// 
    /// See MessageBoxExample.TestLocalization for an example.
    /// </remarks>
    [Tooltip("Set this to a custom function that will be used to localize the button texts.")]
    public static Func<string, string> Localize = (sourceString) => { return sourceString; };

    /// <summary>
    /// Set to true to send the title and message of message boxes and menus thru the Localize function.
    /// </summary>
    [Tooltip("Set to true to send the title and message of message boxes and menus thru the Localize function.")]
    public static bool LocalizeTitleAndMessage = false;

    DialogResult result;
    Action<DialogResult> onFinish;

    /// <summary>
    /// Display a message box.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="onFinished">An action to be called when the dialog is closed.</param>
    /// <param name="buttons">Selects what buttons are shown on the dialog.</param>
    /// <returns>The message box game object.</returns>
    public static MessageBox Show(string message, Action<DialogResult> onFinished, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        return Show(message, null, onFinished, buttons);
    }

    /// <summary>
    /// Display a message box.
    /// </summary>
    /// <param name="message">The message to be displayed.</param>
    /// <param name="title">A title for the message box.</param>
    /// <param name="onFinished">An action to be called when the dialog is closed.</param>
    /// <param name="buttons">Selects what buttons are shown on the dialog.</param>
    /// <returns>The message box game object.</returns>
    public static MessageBox Show(string message, string title = null, Action<DialogResult> onFinished = null, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        var box = (Instantiate(Resources.Load<GameObject>(PrefabResourceName)) as GameObject).GetComponent<MessageBox>();

        box.onFinish = onFinished;

        box.SetUpButtons(buttons);
        box.SetText(message, title);

        return box;
    }

    void SetUpButtons(MessageBoxButtons buttons)
    {
        var button = Button.gameObject;
        switch (buttons)
        {
            case MessageBoxButtons.OK:
                button.GetComponentInChildren<Text>().text = Localize("OK");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.OK; Close(); });
                break;
            case MessageBoxButtons.OKCancel:
                button.GetComponentInChildren<Text>().text = Localize("OK");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.OK; Close(); });

                CreateButton(button, Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); });
                break;
            case MessageBoxButtons.YesNo:
                button.GetComponentInChildren<Text>().text = Localize("Yes");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Yes; Close(); });

                CreateButton(button, Localize("No"), () => { result = DialogResult.No; Close(); });
                break;
            case MessageBoxButtons.RetryCancel:
                button.GetComponentInChildren<Text>().text = Localize("Retry");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Retry; Close(); });

                CreateButton(button, Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); });
                break;
            case MessageBoxButtons.YesNoCancel:
                button.GetComponentInChildren<Text>().text = Localize("Yes");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Yes; Close(); });

                CreateButton(button, Localize("No"), () => { result = DialogResult.No; Close(); });
                CreateButton(button, Localize("Cancel"), () => { result = DialogResult.Cancel; Close(); });
                break;
            case MessageBoxButtons.AbortRetryIgnore:
                button.GetComponentInChildren<Text>().text = Localize("Abort");
                button.GetComponent<Button>().onClick.AddListener(() => { result = DialogResult.Abort; Close(); });

                CreateButton(button, Localize("Retry"), () => { result = DialogResult.Retry; Close(); });
                CreateButton(button, Localize("Ignore"), () => { result = DialogResult.Ignore; Close(); });
                break;
        }
    }

    GameObject CreateButton(GameObject buttonToClone, string label, UnityAction target)
    {
        var button = Instantiate(buttonToClone) as GameObject;

        button.transform.SetParent(buttonToClone.transform.parent, false);

        button.GetComponentInChildren<Text>().text = label;
        button.GetComponent<Button>().onClick.AddListener(target);

        return button;
    }

    /// <summary>
    /// Closes the dialog.
    /// </summary>
    public override void Close()
    {
        if (onFinish != null)
            onFinish(result);
        base.Close();
    }
}

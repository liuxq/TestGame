using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System;

public class MessageBoxExample : MonoBehaviour
{
    void Start()
    {
        // Remove any testing copies of the prefabs.
        var messageBox = GameObject.Find("Message Box");
        if (messageBox != null)
            Destroy(messageBox);

        var menuBox = GameObject.Find("Menu Box");
        if (menuBox != null)
            Destroy(menuBox);
    }

    public void Test()
    {
        MessageBox.Show("Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");
    }

    public void TestWithTitle()
    {
        MessageBox.Show("This is a message with a title.", "Message Box Title");
    }

    public void TestWithCallback()
    {
        MessageBox.Show(
            "This is a message with a callback.",
            "Message Box Callback Example",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); }
        );
    }

    public void TestOKCancelButtons()
    {
        MessageBox.Show
        (
            "Are you sure you wish to delete your save game?",
            "Delete Save",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); },
            MessageBoxButtons.OKCancel
        );
    }

    public void TestRetryCancelButtons()
    {
        MessageBox.Show
        (
            "This is a message with a set of buttons selected.",
            "Message Box Buttons Example",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); },
            MessageBoxButtons.RetryCancel
        );
    }

    public void TestYesNoButtons()
    {
        MessageBox.Show
        (
            "Give us five stars?",
            "Review Game",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); },
            MessageBoxButtons.YesNo
        );
    }

    public void TestYesNoCancelButtons()
    {
        MessageBox.Show
        (
            "This is a message with a set of buttons selected.",
            "Message Box Buttons Example",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); },
            MessageBoxButtons.YesNoCancel
        );
    }

    public void TestAbortRetryIgnoreButtons()
    {
        MessageBox.Show
        (
            "Not ready reading drive A",
            "Message Box Buttons Example",
            (result) => { MessageBox.Show("You Clicked " + result.ToString(), "Dialog Result"); },
            MessageBoxButtons.AbortRetryIgnore
        );
    }

    public void TestMenu5()
    {
        MenuBox.Show
        (
            new string[]
            {
                "Option 1\nOption description can go here.",
                "Option 2\nTwo",
                "Option 3\nThree",
                "Option 4\nFour",
                "Option 5\nFive of Nine?",
            },
            new UnityAction[]
            {
                () => MessageBox.Show("You clicked on Option 1"),
                () => MessageBox.Show("You clicked on Option 2"),
                () => MessageBox.Show("You clicked on Option 3"),
                () => MessageBox.Show("You clicked on Option 4"),
                () => MessageBox.Show("You clicked on Option 5"),
            }
        );
    }

    public void TestMenu10()
    {
        MenuBox.Show
        (
            new string[]
            {
                "Option 1",
                "Option 2",
                "Option 3",
                "Option 4",
                "Option 5",
                "Option 6",
                "Option 7",
                "Option 8",
                "Option 9",
                "Show an even bigger menu!",
            },
            new UnityAction[]
            {
                () => MessageBox.Show("You clicked on Option 1"),
                () => MessageBox.Show("You clicked on Option 2"),
                () => MessageBox.Show("You clicked on Option 3"),
                () => MessageBox.Show("You clicked on Option 4"),
                () => MessageBox.Show("You clicked on Option 5"),
                () => MessageBox.Show("You clicked on Option 6"),
                () => MessageBox.Show("You clicked on Option 7"),
                () => MessageBox.Show("You clicked on Option 8"),
                () => MessageBox.Show("You clicked on Option 9"),
                TestMenu15,
            },
            "Ten Item Test Menu"
        );
    }

    public void TestMenu15()
    {
        MenuBox.Show
        (
            new string[]
            {
                "Option 1",
                "Option 2",
                "Option 3",
                "Option 4",
                "Option 5",
                "Option 6",
                "Option 7",
                "Option 8",
                "Option 9",
                "Option 10",
                "Option 11",
                "Option 12",
                "Option 13",
                "Option 14",
                "Option 15",
            },
            new UnityAction[]
            {
                () => MessageBox.Show("You clicked on Option 1"),
                () => MessageBox.Show("You clicked on Option 2"),
                () => MessageBox.Show("You clicked on Option 3"),
                () => MessageBox.Show("You clicked on Option 4"),
                () => MessageBox.Show("You clicked on Option 5"),
                () => MessageBox.Show("You clicked on Option 6"),
                () => MessageBox.Show("You clicked on Option 7"),
                () => MessageBox.Show("You clicked on Option 8"),
                () => MessageBox.Show("You clicked on Option 9"),
                () => MessageBox.Show("You clicked on Option 10"),
                () => MessageBox.Show("You clicked on Option 11"),
                () => MessageBox.Show("You clicked on Option 12"),
                () => MessageBox.Show("You clicked on Option 13"),
                () => MessageBox.Show("You clicked on Option 14"),
                () => MessageBox.Show("You clicked on Option 15"),
            },
            "Fifteen Item Test Menu"
        );
    }

    public void TestLocalization()
    {
        var originalLocalizeFunction = MessageBox.Localize;

        MessageBox.Localize =
            (originalString) =>
            {
                // You would normally hook into your existing localization system here to lookup and return a translated string using the original as a key.
                // For instance using "Localization package" from the asset store you would do this:
                // MessageBox.Localize = (originalString) => { return Language.Get(originalString); };

                // Current text strings that need to be localized are "OK", "Yes", "No", "Cancel", "Abort", "Retry", "Ignore"

                // This function only needs to be set once at game startup.

                // For test example replace the original string with X's, so "Hello World" becomes "XXXXXXXXXXX"
                return new String('X', originalString.Length);
            };

        // Set LocalizeTitleAndMessage to true to send the title and message of message boxes and menus thru the Localize function.
        // MessageBox.LocalizeTitleAndMessage = true;

        MessageBox.Show("Button Localization Test", (result) => { MessageBox.Localize = originalLocalizeFunction; }, MessageBoxButtons.AbortRetryIgnore);
    }
}
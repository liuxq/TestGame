using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuBox : ModalBox
{
    /// <summary>
    /// Set this to the name of the prefab that should be loaded when a menu box is shown.
    /// </summary>
    [Tooltip("Set this to the name of the prefab that should be loaded when a menu box is shown.")]
    public static string PrefabResourceName = "Menu Box";

    /// <summary>
    /// Display a menu box.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="onFinished"></param>
    /// <param name="title"></param>
    public static MenuBox Show(IEnumerable<string> options, IEnumerable<UnityAction> actions, string title = "")
    {
        if (options.Count() != actions.Count())
            throw new Exception("MenuBox.Show must be called with an equal number of options and actions.");

        var box = (Instantiate(Resources.Load<GameObject>(PrefabResourceName)) as GameObject).GetComponent<MenuBox>();

        box.SetText(null, title);
        box.SetUpButtons(options, actions);

        return box;
    }

    void SetUpButtons(IEnumerable<string> options, IEnumerable<UnityAction> actions)
    {
        for (var i = 0; i < options.Count(); i++)
        {
            CreateButton(Button.gameObject, options.ElementAt(i), actions.ElementAt(i));
        }

        Destroy(Button.gameObject);
    }

    GameObject CreateButton(GameObject buttonToClone, string label, UnityAction action)
    {
        var button = Instantiate(buttonToClone) as GameObject;

        button.GetComponentInChildren<Text>().text = label;
        button.GetComponent<Button>().onClick.AddListener(action);
        button.GetComponent<Button>().onClick.AddListener(() => { Close(); });

        button.transform.SetParent(buttonToClone.transform.parent, false);

        return button;
    }
}

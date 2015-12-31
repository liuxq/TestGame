using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DesignForInventory : MonoBehaviour
{

    public Text inventoryTitle;
    public Image backgroundInventory;
    public Image backgroundSlot;
    public Text amountSlot;

    // Use this for initialization
    void Start()
    {
        inventoryTitle = transform.GetChild(0).GetComponent<Text>();
        backgroundInventory = GetComponent<Image>();
        backgroundSlot = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        amountSlot = getTextAmountOfItem();
    }

    public Text getTextAmountOfItem()
    {
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).childCount != 0)
                return transform.GetChild(1).GetChild(i).GetChild(0).GetChild(1).GetComponent<Text>();
        }
        return null;
    }




}

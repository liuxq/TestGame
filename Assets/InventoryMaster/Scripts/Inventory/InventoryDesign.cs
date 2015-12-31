using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryDesign : MonoBehaviour
{

    //UIDesign
    [SerializeField]
    public Image slotDesignTemp;
    [SerializeField]
    public Image slotDesign;
    [SerializeField]
    public Image inventoryDesign;
    [SerializeField]
    public bool showInventoryCross;
    [SerializeField]
    public Image inventoryCrossImage;
    [SerializeField]
    public RectTransform inventoryCrossRectTransform;
    [SerializeField]
    public int inventoryCrossPosX;
    [SerializeField]
    public int inventoryCrossPosY;
    [SerializeField]
    public string inventoryTitle;
    [SerializeField]
    public Text inventoryTitleText;
    [SerializeField]
    public int inventoryTitlePosX;
    [SerializeField]
    public int inventoryTitlePosY;
    [SerializeField]
    public int panelSizeX;
    [SerializeField]
    public int panelSizeY;

    public void setVariables()
    {
        inventoryTitlePosX = (int)transform.GetChild(0).GetComponent<RectTransform>().localPosition.x;
        inventoryTitlePosY = (int)transform.GetChild(0).GetComponent<RectTransform>().localPosition.y;
        panelSizeX = (int)GetComponent<RectTransform>().sizeDelta.x;
        panelSizeY = (int)GetComponent<RectTransform>().sizeDelta.y;
        inventoryTitle = transform.GetChild(0).GetComponent<Text>().text;
        inventoryTitleText = transform.GetChild(0).GetComponent<Text>();
        if (GetComponent<Hotbar>() == null)
        {
            inventoryCrossRectTransform = transform.GetChild(2).GetComponent<RectTransform>();
            inventoryCrossImage = transform.GetChild(2).GetComponent<Image>();
        }
        inventoryDesign = GetComponent<Image>();
        slotDesign = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        slotDesignTemp = slotDesign;
        slotDesignTemp.sprite = slotDesign.sprite;
        slotDesignTemp.color = slotDesign.color;
        slotDesignTemp.material = slotDesign.material;
        slotDesignTemp.type = slotDesign.type;
        slotDesignTemp.fillCenter = slotDesign.fillCenter;
    }

    public void updateEverything()
    {
        transform.GetChild(0).GetComponent<RectTransform>().localPosition = new Vector3(inventoryTitlePosX, inventoryTitlePosY, 0);
        transform.GetChild(0).GetComponent<Text>().text = inventoryTitle;
    }

    public void changeCrossSettings()
    {
        GameObject cross = transform.GetChild(2).gameObject;
        if (showInventoryCross)
        {
            cross.SetActive(showInventoryCross);
            inventoryCrossRectTransform.localPosition = new Vector3(inventoryCrossPosX, inventoryCrossPosY, 0);

        }
        else
        {
            cross.SetActive(showInventoryCross);
        }
    }

    public void updateAllSlots()
    {
        Image slot = null;
#if UNITY_EDITOR
        Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/InventoryMaster/Resources/Prefabs/Slot - Inventory.prefab");
#endif

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            slot = transform.GetChild(1).GetChild(i).GetComponent<Image>();
            slot.sprite = slotDesignTemp.sprite;
            slot.color = slotDesignTemp.color;
            slot.material = slotDesignTemp.material;
            slot.type = slotDesignTemp.type;
            slot.fillCenter = slotDesignTemp.fillCenter;
        }
#if UNITY_EDITOR
        PrefabUtility.ReplacePrefab(slot.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif

    }
}

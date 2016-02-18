using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KBEngine;

public enum TooltipType
{
    Inventory = 0,
    Equipment = 1,
    Skill = 2
}

public class Tooltip : MonoBehaviour
{
    
    public Item item;
    private EquipmentSystem eS;

    public TooltipType tooltipType;

    //GUI
    public float tooltipHeight;

    private Image tooltipImageIcon;
    private Text tooltipNameText;
    private Text tooltipDescText;    

    void Start()
    {
        setVariables();
        deactivateTooltip();
        tooltipHeight = this.GetComponent<RectTransform>().rect.height;
    }
    public void setVariables()
    {
        tooltipImageIcon = transform.GetChild(1).GetComponent<Image>();
        tooltipNameText = transform.GetChild(2).GetComponent<Text>();
        tooltipDescText = transform.GetChild(3).GetComponent<Text>();
    }

    public void activateTooltip()               //if you activate the tooltip through hovering over an item
    {
        this.transform.gameObject.SetActive(true);

        tooltipImageIcon.sprite = item.itemIcon;         //and the itemIcon...
        tooltipNameText.text = item.itemName;            //,itemName...
        tooltipDescText.text = item.itemDesc;            //and itemDesc is getting set        
    }

    public void deactivateTooltip()             //deactivating the tooltip after you went out of a slot
    {
        this.transform.gameObject.SetActive(false);
    }
    public void equipTooltip()             //deactivating the tooltip after you went out of a slot
    {
        
    }

    public void dropItem()
    {
        KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
        if (p != null)
        {
            p.dropRequest(item.itemUUID);
        }
    }
    public void equipItem()
    {
        //验证是否可以装备
        if (eS != null)
        {
            for (int i = 0; i < eS.slotsInTotal; i++)
            {
                if (eS.itemTypeOfSlots[i].Equals(item.itemType))
                {
                    //if (eS.transform.GetChild(1).GetChild(i).childCount == 0)
                    //{
                    //    this.gameObject.transform.SetParent(eS.transform.GetChild(1).GetChild(i));
                    //    this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    //    eS.gameObject.GetComponent<Inventory>().updateItemList();
                    //    inventory.updateItemList();
                    //    inventory.EquiptItem(item);
                    //    gearable = true;
                    //    if (duplication != null)
                    //        Destroy(duplication.gameObject);
                    //    break;
                    //}
                    KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
                    if (p != null)
                    {
                        p.equipItemRequest(item.itemIndex, i);
                    }
                    break;
                }
            }
        }
    }

}

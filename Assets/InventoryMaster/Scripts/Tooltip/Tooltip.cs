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
    private Inventory inventory;
    public TooltipType tooltipType;

    public UnityEngine.GameObject btn_equip;
    public UnityEngine.GameObject btn_unEquip;
    public UnityEngine.GameObject btn_use;
    public UnityEngine.GameObject btn_drop;
    public UnityEngine.GameObject btn_hotBar;

    //GUI
    public float tooltipHeight;

    private Image tooltipImageIcon;
    private Text tooltipNameText;
    private Text tooltipDescText;
    private Text tooltipAttrText;  

    void Start()
    {
        setVariables();
        deactivateTooltip();
        tooltipHeight = this.GetComponent<RectTransform>().rect.height;
        //if (UnityEngine.GameObject.FindGameObjectWithTag("EquipmentSystem") != null)
        if (UnityEngine.GameObject.FindGameObjectWithTag("Player"))
            eS = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
        if (UnityEngine.GameObject.FindGameObjectWithTag("Player"))
            inventory = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
    }
    public void setVariables()
    {
        tooltipImageIcon = transform.GetChild(1).GetComponent<Image>();
        tooltipNameText = transform.GetChild(2).GetComponent<Text>();
        tooltipDescText = transform.GetChild(3).GetComponent<Text>();
        tooltipAttrText = transform.GetChild(4).GetComponent<Text>();
    }

    public void activateTooltip()               //if you activate the tooltip through hovering over an item
    {
        this.transform.gameObject.SetActive(true);
        setOperateByType(this.tooltipType);//设置显示的操作按钮

        tooltipImageIcon.sprite = item.itemIcon;         //and the itemIcon...
        tooltipNameText.text = item.itemName;            //,itemName...
        tooltipDescText.text = item.itemDesc;            //and itemDesc is getting set

        tooltipAttrText.text = "";
        foreach(ItemAttribute attr in item.itemAttributes)
        {
            tooltipAttrText.text += attr.attributeName + ": " + attr.attributeValue + "\n";
        }
    }
    private void setOperateByType(TooltipType ttype)
    {
        btn_equip.SetActive(false);
        btn_unEquip.SetActive(false);
        btn_use.SetActive(false);
        btn_drop.SetActive(false);
        btn_hotBar.SetActive(false);
        if (ttype == TooltipType.Inventory)
        {
            btn_drop.SetActive(true);
            if (item.isEquipItem())
                btn_equip.SetActive(true);
            if (item.isConsumeItem())
            {
                btn_use.SetActive(true);
                btn_hotBar.SetActive(true);
            }
        }
        else if (ttype == TooltipType.Equipment)
        {
            btn_unEquip.SetActive(true);
        }
    }

    public void deactivateTooltip()             //deactivating the tooltip after you went out of a slot
    {
        this.transform.gameObject.SetActive(false);
    }
    public void equipTooltip()
    {
        
    }

    public void dropItem()
    {
        KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
        if (p != null)
        {
            p.dropRequest(item.itemUUID);
            deactivateTooltip();
        }
    }
    public void equipItem()
    {
        if (eS == null)
            eS = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
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
                        deactivateTooltip();
                    }
                    break;
                }
            }
        }
    }

    public void unEquipItem()
    {
        if (inventory == null)
            inventory = UnityEngine.GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        int emptyIndex = inventory.getFirstEmptyItemIndex();
        if (emptyIndex >= 0)
        {
            KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
            if (p != null)
            {
                p.equipItemRequest(emptyIndex, item.itemIndex);
                deactivateTooltip();
            }
        }
    }

    public void useItem()
    {
        KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
        if (p != null)
        {
            p.useItemRequest(item.itemIndex);
            deactivateTooltip();
        }
    }

    public void hotBarItem()
    {
        HotBarProcess._instance.upItem(item.itemID);
    }

}

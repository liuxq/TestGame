using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using KBEngine;

public class ItemProcess : MonoBehaviour {

    public Tooltip tooltip;
    public EquipmentSystem eS;

    private Inventory equipInventory;
    private Inventory itemInventory;
	// Use this for initialization
	void Start () {
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        if (canvas.transform.Find("Tooltip - Inventory(Clone)") != null)
            tooltip = canvas.transform.Find("Tooltip - Inventory(Clone)").GetComponent<Tooltip>();

        eS = this.transform.GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();
        equipInventory = this.transform.GetComponent<PlayerInventory>().characterSystem.GetComponent<Inventory>();
        itemInventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();

        KBEngine.Event.registerOut("dropItem_re", this, "dropItem_re");
        KBEngine.Event.registerOut("pickUp_re", this, "pickUp_re");
        KBEngine.Event.registerOut("equipItemRequest_re", this, "equipItemRequest_re");
	}

    public void dropItem_re(Int32 itemIndex)
    {
        if(itemInventory == null)
            itemInventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        if (itemInventory != null)
        {
            itemInventory.deleteItemByIndex(itemIndex);
            itemInventory.updateItemList();
            itemInventory.stackableSettings();
            tooltip.deactivateTooltip();
        }
    }

    public void pickUp_re(ITEM_INFO itemInfo)
    {
        if (itemInventory == null)
            itemInventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        if (itemInventory != null)
        {
            itemInventory.addItemToInventory(itemInfo.itemId, itemInfo.UUID, (int)itemInfo.itemCount, itemInfo.itemIndex);
            itemInventory.updateItemList();
            itemInventory.stackableSettings();
        }
    }

    public void equipItemRequest_re(ITEM_INFO itemInfo, ITEM_INFO equipItemInfo)
    {
        Int32 itemIndex = itemInfo.itemIndex;
        Int32 equipItemIndex = equipItemInfo.itemIndex;
        UInt64 itemUUid = itemInfo.UUID;
        UInt64 equipItemUUid = equipItemInfo.UUID;

        if (itemUUid > 0)
            itemInventory.addItemToInventory(itemInfo.itemId, itemInfo.UUID, 1, itemInfo.itemIndex);
        else
            itemInventory.deleteItemByIndex(itemIndex);

        if (equipItemUUid > 0)
            equipInventory.addItemToInventory(equipItemInfo.itemId, equipItemInfo.UUID, 1, equipItemInfo.itemIndex);
        else
            equipInventory.deleteItemByIndex(equipItemIndex);
        
        itemInventory.updateItemList();
        itemInventory.stackableSettings();
        equipInventory.updateItemList();
        equipInventory.stackableSettings();
    }
}

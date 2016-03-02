using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

    public void pickUp_re(Dictionary<string, object> itemInfo)
    {
        if (itemInventory == null)
            itemInventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        if (itemInventory != null)
        {
            itemInventory.addItemToInventory((Int32)itemInfo["itemId"], (UInt64)itemInfo["UUID"], (int)(UInt32)itemInfo["itemCount"], (Int32)itemInfo["itemIndex"]);
            itemInventory.updateItemList();
            itemInventory.stackableSettings();
        }
    }

    public void equipItemRequest_re(Dictionary<string, object> itemInfo, Dictionary<string, object> equipItemInfo)
    {
        Int32 itemIndex = (Int32)itemInfo["itemIndex"];
        Int32 equipItemIndex = (Int32)equipItemInfo["itemIndex"];
        UInt64 itemUUid = (UInt64)itemInfo["UUID"];
        UInt64 equipItemUUid = (UInt64)equipItemInfo["UUID"];

        if (itemUUid > 0)
            itemInventory.addItemToInventory((Int32)itemInfo["itemId"], (UInt64)itemInfo["UUID"], 1, (Int32)itemInfo["itemIndex"]);
        else
            itemInventory.deleteItemByIndex(itemIndex);

        if (equipItemUUid > 0)
            equipInventory.addItemToInventory((Int32)equipItemInfo["itemId"], (UInt64)equipItemInfo["UUID"], 1, (Int32)equipItemInfo["itemIndex"]);
        else
            equipInventory.deleteItemByIndex(equipItemIndex);
        
        itemInventory.updateItemList();
        itemInventory.stackableSettings();
        equipInventory.updateItemList();
        equipInventory.stackableSettings();
    }
}

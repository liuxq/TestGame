using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemProcess : MonoBehaviour {

    public Tooltip tooltip;
    public EquipmentSystem eS;
	// Use this for initialization
	void Start () {
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        if (canvas.transform.Find("Tooltip - Inventory(Clone)") != null)
            tooltip = canvas.transform.Find("Tooltip - Inventory(Clone)").GetComponent<Tooltip>();
        if (GameObject.FindGameObjectWithTag("EquipmentSystem") != null)
            eS = this.transform.GetComponent<PlayerInventory>().characterSystem.GetComponent<EquipmentSystem>();


        KBEngine.Event.registerOut("dropItem_re", this, "dropItem_re");
        KBEngine.Event.registerOut("pickUp_re", this, "pickUp_re");
	}

    public void dropItem_re(Int32 itemIndex)
    {
        Inventory _inventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        if (_inventory != null)
        {
            _inventory.deleteItemByIndex(itemIndex);
            _inventory.updateItemList();
            _inventory.stackableSettings();
            tooltip.deactivateTooltip();
        }
    }

    public void pickUp_re(Dictionary<string, object> itemInfo)
    {
        Inventory _inventory = this.transform.GetComponent<PlayerInventory>().inventory.GetComponent<Inventory>();
        if (_inventory != null)
        {
            _inventory.addItemToInventory((Int32)itemInfo["itemId"], (UInt64)itemInfo["UUID"], 1, (Int32)itemInfo["itemIndex"]);
            _inventory.updateItemList();
            _inventory.stackableSettings();
        }
    }
}

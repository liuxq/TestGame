using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item
{
    public string itemName;                                     //itemName of the item
    public int itemID;                                          //itemID of the item
    public string itemDesc;                                     //itemDesc of the item
    public Sprite itemIcon;                                     //itemIcon of the item
    public GameObject itemModel;                                //itemModel of the item
    public int itemValue = 1;                                   //itemValue is at start 1
    public ItemType itemType;                                   //itemType of the Item
    public float itemWeight;                                    //itemWeight of the item
    public int maxStack = 1;
    public int indexItemInList = 999;    
    public int rarity;

    [SerializeField]
    public List<ItemAttribute> itemAttributes = new List<ItemAttribute>();    
    
    public Item(){}

    public Item(string name, int id, string desc, Sprite icon, GameObject model, int maxStack, ItemType type, string sendmessagetext, List<ItemAttribute> itemAttributes)                 //function to create a instance of the Item
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemIcon = icon;
        itemModel = model;
        itemType = type;
        this.maxStack = maxStack;
        this.itemAttributes = itemAttributes;
    }

    public Item getCopy()
    {
        return (Item)this.MemberwiseClone();        
    }   
    
    
}



using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemOnObject : MonoBehaviour                   //Saves the Item in the slot
{
    public Item item;                                       //Item 
    private Text text;                                      //text for the itemValue
    private Image image_icon;    //图标
    private Image image_cool;    //冷却

    void Update()
    {
        text.text = "" + item.itemValue;                     //sets the itemValue         
        image_icon.sprite = item.itemIcon;
        //GetComponent<ConsumeItem>().item = item;
        if (item.isConsumeItem())
        {
            if (ConsumeLimitCD.instance.isWaiting())
            {
                image_cool.fillAmount = ConsumeLimitCD.instance.restTime / ConsumeLimitCD.instance.totalTime;
            }
            else
            {
                image_cool.fillAmount = 0;
            }
        }
    }

    void Start()
    {
        image_icon = transform.GetChild(0).GetComponent<Image>();
        transform.GetChild(0).GetComponent<Image>().sprite = item.itemIcon;                 //set the sprite of the Item 
        text = transform.GetChild(1).GetComponent<Text>();                                  //get the text(itemValue GameObject) of the item
        image_cool = transform.GetChild(2).GetComponent<Image>();
    }
}

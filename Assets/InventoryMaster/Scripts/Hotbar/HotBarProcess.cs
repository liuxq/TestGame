using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KBEngine;

public class HotBarProcess : MonoBehaviour, IPointerDownHandler
{
    public static HotBarProcess _instance;
    private Inventory inventory = null;
    private int itemId = -1;

    private Text text;                                      //text for the itemValue
    private Image image_icon;    //图标
    private Image image_cool;    //冷却
	// Use this for initialization
	void Start () {
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        if (canvas.transform.Find("Panel - Inventory(Clone)") != null)
            inventory = canvas.transform.Find("Panel - Inventory(Clone)").GetComponent<Inventory>();

        this.gameObject.SetActive(false);
	}

    void Awake()
    {
        _instance = this;
        image_icon = transform.GetChild(0).GetComponent<Image>();
        text = transform.GetChild(1).GetComponent<Text>();                                  //get the text(itemValue GameObject) of the item
        image_cool = transform.GetChild(2).GetComponent<Image>();
    }
    void Update()
    {
        if (itemId == -1)
            return;
        //如果快捷栏有物品，则刷新物品状态
        if (inventory == null)
        {
            UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
            inventory = canvas.transform.Find("Panel - Inventory(Clone)").GetComponent<Inventory>();
        }
        if (inventory == null)
            return;
        //物品已使用完或者消失
        UnityEngine.GameObject itemobject = inventory.getItemGameObject(itemId);
        if (itemobject == null)
        {
            this.itemId = -1;
            this.gameObject.SetActive(false);
            return;
        }

        image_icon.sprite = itemobject.GetComponent<ItemOnObject>().item.itemIcon;
        text.rectTransform.localPosition = itemobject.transform.GetChild(1).GetComponent<Text>().transform.localPosition;
        text.enabled = true;
        text.text = itemobject.transform.GetChild(1).GetComponent<Text>().text;
        if (ConsumeLimitCD.instance.isWaiting())
        {
            image_cool.fillAmount = ConsumeLimitCD.instance.restTime / ConsumeLimitCD.instance.totalTime;
        }
        else
        {
            image_cool.fillAmount = 0;
        }
    }
    //放到快捷栏
    public void upItem(int itemId)
    {
        this.itemId = itemId;
        this.gameObject.SetActive(true);
    }
    //点击使用
    public void OnPointerDown(PointerEventData data)
    {
        UnityEngine.GameObject itemobject = inventory.getItemGameObject(itemId);
        if (itemobject != null)
        {
            KBEngine.Avatar p = (KBEngine.Avatar)KBEngineApp.app.player();
            if (p != null)
            {
                p.useItemRequest(itemobject.GetComponent<ItemOnObject>().item.itemIndex);
                
            }
        }
    }
}

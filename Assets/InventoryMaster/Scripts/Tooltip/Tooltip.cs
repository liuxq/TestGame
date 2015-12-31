using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Tooltip : MonoBehaviour
{
    public Item item;

    //GUI
    [SerializeField]
    public Image tooltipBackground;
    [SerializeField]
    public Text tooltipNameText;
    [SerializeField]
    public Text tooltipDescText;

    //Tooltip Settings
    [SerializeField]
    public bool showTooltip;
    [SerializeField]
    private bool tooltipCreated;
    [SerializeField]
    private int tooltipWidth;
    [SerializeField]
    public int tooltipHeight;
    [SerializeField]
    private bool showTooltipIcon;
    [SerializeField]
    private int tooltipIconPosX;
    [SerializeField]
    private int tooltipIconPosY;
    [SerializeField]
    private int tooltipIconSize;
    [SerializeField]
    private bool showTooltipName;
    [SerializeField]
    private int tooltipNamePosX;
    [SerializeField]
    private int tooltipNamePosY;
    [SerializeField]
    private bool showTooltipDesc;
    [SerializeField]
    private int tooltipDescPosX;
    [SerializeField]
    private int tooltipDescPosY;
    [SerializeField]
    private int tooltipDescSizeX;
    [SerializeField]
    private int tooltipDescSizeY;

    //Tooltip Objects
    [SerializeField]
    private GameObject tooltip;
    [SerializeField]
    private RectTransform tooltipRectTransform;
    [SerializeField]
    private GameObject tooltipTextName;
    [SerializeField]
    private GameObject tooltipTextDesc;
    [SerializeField]
    private GameObject tooltipImageIcon;

    void Start()
    {
        deactivateTooltip();
    }

#if UNITY_EDITOR
    [MenuItem("Master System/Create/Tooltip")]        //creating the menu item
    public static void menuItemCreateInventory()       //create the inventory at start
    {
        if (GameObject.FindGameObjectWithTag("Tooltip") == null)
        {
            GameObject toolTip = (GameObject)Instantiate(Resources.Load("Prefabs/Tooltip - Inventory") as GameObject);
            toolTip.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            toolTip.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
            toolTip.AddComponent<Tooltip>().setImportantVariables();
        }
    }
#endif
    public void setImportantVariables()
    {
        tooltipRectTransform = this.GetComponent<RectTransform>();

        tooltipTextName = this.transform.GetChild(2).gameObject;
        tooltipTextName.SetActive(false);
        tooltipImageIcon = this.transform.GetChild(1).gameObject;
        tooltipImageIcon.SetActive(false);
        tooltipTextDesc = this.transform.GetChild(3).gameObject;
        tooltipTextDesc.SetActive(false);

        tooltipIconSize = 50;
        tooltipWidth = 150;
        tooltipHeight = 250;
        tooltipDescSizeX = 100;
        tooltipDescSizeY = 100;
    }

    public void setVariables()
    {
        tooltipBackground = transform.GetChild(0).GetComponent<Image>();
        tooltipNameText = transform.GetChild(2).GetComponent<Text>();
        tooltipDescText = transform.GetChild(3).GetComponent<Text>();
    }

    public void activateTooltip()               //if you activate the tooltip through hovering over an item
    {
        tooltipTextName.SetActive(true);
        tooltipImageIcon.SetActive(true);
        tooltipTextDesc.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(true);          //Tooltip getting activated
        transform.GetChild(1).GetComponent<Image>().sprite = item.itemIcon;         //and the itemIcon...
        transform.GetChild(2).GetComponent<Text>().text = item.itemName;            //,itemName...
        transform.GetChild(3).GetComponent<Text>().text = item.itemDesc;            //and itemDesc is getting set        
    }

    public void deactivateTooltip()             //deactivating the tooltip after you went out of a slot
    {
        tooltipTextName.SetActive(false);
        tooltipImageIcon.SetActive(false);
        tooltipTextDesc.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void updateTooltip()
    {
        if (!Application.isPlaying)
        {
            tooltipRectTransform.sizeDelta = new Vector2(tooltipWidth, tooltipHeight);

            if (showTooltipName)
            {
                tooltipTextName.gameObject.SetActive(true);
                this.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(tooltipNamePosX, tooltipNamePosY, 0);
            }
            else
            {
                this.transform.GetChild(2).gameObject.SetActive(false);
            }

            if (showTooltipIcon)
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector3(tooltipIconPosX, tooltipIconPosY, 0);
                this.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipIconSize, tooltipIconSize);
            }
            else
            {
                this.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (showTooltipDesc)
            {
                this.transform.GetChild(3).gameObject.SetActive(true);
                this.transform.GetChild(3).GetComponent<RectTransform>().localPosition = new Vector3(tooltipDescPosX, tooltipDescPosY, 0);
                this.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipDescSizeX, tooltipDescSizeY);
            }
            else
            {
                this.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowTooltip : MonoBehaviour, IPointerDownHandler
{         //Tooltip

    public Tooltip tooltip;                                     //The tooltip script
    public GameObject tooltipGameObject;                        //the tooltip as a GameObject
    public RectTransform canvasRectTransform;                    //the panel(Inventory Background) RectTransform
    public RectTransform tooltipRectTransform;                  //the tooltip RectTransform
    private Item item;


    void Start()
    {
        UnityEngine.GameObject canvas = UnityEngine.GameObject.FindGameObjectWithTag("Canvas");
        if (canvas.transform.Find("Tooltip - Inventory(Clone)") != null)
        {
            tooltip = canvas.transform.Find("Tooltip - Inventory(Clone)").GetComponent<Tooltip>();
            tooltipGameObject = canvas.transform.Find("Tooltip - Inventory(Clone)").gameObject;
            tooltipRectTransform = tooltipGameObject.GetComponent<RectTransform>() as RectTransform;
        }
        canvasRectTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>() as RectTransform;
    }


    public void OnPointerDown(PointerEventData data)
    {
        if (tooltip != null)
        {
            tooltip.tooltipType = TooltipType.Inventory;
            Transform curTransform = this.transform;
            while (curTransform.parent)
            {
                if (curTransform.tag == "MainInventory")
                {
                    tooltip.tooltipType = TooltipType.Inventory;
                    break;
                }
                else if (curTransform.tag == "EquipmentSystem")
                {
                    tooltip.tooltipType = TooltipType.Equipment;
                    break;
                }
                curTransform = curTransform.parent;
            }
            item = GetComponent<ItemOnObject>().item;                   //we get the item
            tooltip.item = item;                                        //set the item in the tooltip
            tooltip.activateTooltip();                                  //set all informations of the item in the tooltip
            if (canvasRectTransform == null)
                return;


            Vector3[] slotCorners = new Vector3[4];                     //get the corners of the slot
            GetComponent<RectTransform>().GetWorldCorners(slotCorners); //get the corners of the slot   

            Vector2 localPointerMinPosition;
            Vector2 localPointerMaxPosition;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[3], data.pressEventCamera, out localPointerMaxPosition) &&
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, slotCorners[1], data.pressEventCamera, out localPointerMinPosition))   // and set the localposition of the tooltip...
            {
                float x = localPointerMaxPosition.x;
                float y = localPointerMinPosition.y;
                if (x + tooltipRectTransform.rect.width > canvasRectTransform.rect.xMax)
                {
                    x = localPointerMinPosition.x - tooltipRectTransform.rect.width;
                }
                if (y - tooltipRectTransform.rect.height < canvasRectTransform.rect.yMin)
                {
                    y += canvasRectTransform.rect.yMin - y + tooltipRectTransform.rect.height;
                }
                
                tooltipRectTransform.localPosition = new Vector3(x, y);
            }

        }

    }

    //public void OnPointerExit(PointerEventData data)                //if we go out of the slot with the item
    //{
    //    if (tooltip != null)
    //        tooltip.deactivateTooltip();            //the tooltip getting deactivated
    //}

}

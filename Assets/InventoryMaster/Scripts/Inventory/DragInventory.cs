using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragInventory : MonoBehaviour, IPointerDownHandler, IDragHandler
{

    private Vector2 pointerOffset;                          //offset of the pointer for dragging
    private RectTransform canvasRectTransform;              //RectTransform of the parent is needed for dragging
    private RectTransform panelRectTransform;               //RectTransform what is getting dragged

    void Awake()
    {
        Canvas canvas = GetComponentInParent<Canvas>();                       //If the canvas is active we instantiate the variables
        if (canvas != null)
        {
            canvasRectTransform = canvas.transform as RectTransform;          //instantiated
            panelRectTransform = transform.parent as RectTransform;           //instantiated
        }
    }

    public void OnPointerDown(PointerEventData data)                          //If you press on the Inventory
    {
        //panelRectTransform.SetAsLastSibling();                                //the Inventory RectTransform is getting set as the last sibling
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);       //and the pointeroffset is getting calculated
    }

    public void OnDrag(PointerEventData data)                               //If you start dragging now
    {
        if (panelRectTransform == null)                                     //and no RectTransform from the inventory is there 
            return;                                                         //the function will break out

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, data.pressEventCamera, out localPointerPosition))
        {
            panelRectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }
}
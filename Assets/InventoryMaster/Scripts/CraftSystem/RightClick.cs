using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RightClick : MonoBehaviour, IPointerDownHandler
{
    CraftResultSlot resultScript;
    CraftSystem craftSystem;

    public void OnPointerDown(PointerEventData data)
    {
        if (craftSystem == null)
        {
            craftSystem = transform.parent.GetComponent<CraftSystem>();
            resultScript = transform.parent.GetChild(3).GetComponent<CraftResultSlot>();
        }
        if (resultScript.temp < (craftSystem.possibleItems.Count - 1))
            resultScript.temp++;
        else
            resultScript.temp = 0;

    }
}

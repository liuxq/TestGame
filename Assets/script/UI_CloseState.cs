using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_CloseState : MonoBehaviour, IPointerDownHandler
{
    UI_AvatarState state;
    void Start()
    {
        state = transform.parent.GetComponent<UI_AvatarState>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            state.closeInventory();
        }
    }
}
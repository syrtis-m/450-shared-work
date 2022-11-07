using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public MovementDice movementDice;
    public AttackDice attackDice;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            GameObject character = eventData.pointerDrag.GetComponent<DragAndDrop>().characterObject;
            character.GetComponent<PlayerCharMvmt>().AssignDiceValues(movementDice.currentDiceSide, attackDice.currentDiceSide);
        }
    }
}

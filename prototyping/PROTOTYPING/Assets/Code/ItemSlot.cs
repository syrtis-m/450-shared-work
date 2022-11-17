using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public MovementDice movementDice;
    public AttackDice attackDice;
    public DragAndDrop slotCharacter = null;

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
        {
            if (!eventData.pointerDrag.GetComponent<DragAndDrop>().diceLocked)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

                if (slotCharacter == null)
                {
                    slotCharacter = eventData.pointerDrag.GetComponent<DragAndDrop>();
                    slotCharacter.GetComponent<DragAndDrop>().SetSlot(this);
                    slotCharacter.characterObject.GetComponent<PlayerCharMvmt>().AssignDiceValues(movementDice.currentDiceSide, attackDice.currentDiceSide);
                    Mind.instance.LockDiceEnabled();
                }
            }
        }
    }
}

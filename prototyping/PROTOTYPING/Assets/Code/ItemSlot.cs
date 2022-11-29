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
                if(slotCharacter != null)
                {
                    slotCharacter.reset_pos();
                    slotCharacter.slot = null;
                    slotCharacter = null;

                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    slotCharacter = eventData.pointerDrag.GetComponent<DragAndDrop>();
                    slotCharacter.GetComponent<DragAndDrop>().SetSlot(this);
                    slotCharacter.characterObject.GetComponent<PlayerCharMvmt>().AssignDiceValues(movementDice.currentDiceSide, attackDice.currentDiceSide);
                    slotCharacter.characterObject.GetComponent<PlayerCharMvmt>().DrawTiles();
                    Mind.instance.LockDiceEnabled();
                }
                else
                {
                    eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
                    slotCharacter = eventData.pointerDrag.GetComponent<DragAndDrop>();
                    slotCharacter.GetComponent<DragAndDrop>().SetSlot(this);
                    slotCharacter.characterObject.GetComponent<PlayerCharMvmt>().AssignDiceValues(movementDice.currentDiceSide, attackDice.currentDiceSide);
                    slotCharacter.characterObject.GetComponent<PlayerCharMvmt>().DrawTiles();
                    Mind.instance.LockDiceEnabled();
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private CanvasGroup canvasGroup;
    public GameObject characterObject;
    public ItemSlot slot = null;
    public bool diceLocked = false;
    Vector2 mousePostion;


    private RectTransform rectTransform; //current location
    private Vector3 originPos; //original location
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
        mousePostion = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        characterObject.GetComponent<PlayerCharMvmt>().selector = this;
    }

    public void SetSlot(ItemSlot itemSlot)
    {
        if (!diceLocked)
        {
            slot = itemSlot;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mousePostion = rectTransform.anchoredPosition;
        if (!diceLocked)
        {
            if (slot != null)
            {
                slot.GetComponent<ItemSlot>().slotCharacter = null;
                slot = null;
            }
            Mind.instance.LockDiceEnabled();
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            characterObject.GetComponent<PlayerCharMvmt>().AssignDiceValues(0, 0);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!diceLocked)
        {

            if( mousePostion.x + (eventData.delta.x / canvas.scaleFactor) < 120 &&
                mousePostion.x + (eventData.delta.x / canvas.scaleFactor) > 40  &&
                mousePostion.y + (eventData.delta.y / canvas.scaleFactor) < 80 &&
                mousePostion.y + (eventData.delta.y / canvas.scaleFactor) > -80
            ){
                mousePostion += eventData.delta / canvas.scaleFactor;
                rectTransform.anchoredPosition = mousePostion;
            }
            else
            {
                mousePostion += eventData.delta / canvas.scaleFactor;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!diceLocked)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
    }
    
    public void reset_pos()
    {//move thing back to original location. called on BeginPlayerTurn
        rectTransform.anchoredPosition = originPos;
    }

    private void OnEnable()
    {
        Mind.BeginPlayerTurnEvent += reset_pos;
    }

    private void OnDisable()
    {
        Mind.BeginPlayerTurnEvent -= reset_pos;

    }

    public void Die()
    {
        Destroy(gameObject);
    }
}

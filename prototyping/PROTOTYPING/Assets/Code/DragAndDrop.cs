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


    private RectTransform rectTransform; //current location
    private Vector3 originPos; //original location
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originPos = rectTransform.anchoredPosition;
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
        if(!diceLocked)
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
        //Debug.Log("OnDrag");
        if (!diceLocked)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
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
    
    private void reset_pos()
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

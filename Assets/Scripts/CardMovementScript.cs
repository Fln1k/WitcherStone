﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovementScript : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler {
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public bool IsDraggable;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];
        TempCardGO = GameObject.Find("TempCardGO");
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);
        DefaultParent = DefaultTempCardParent = transform.parent;
        IsDraggable = DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.Self_Hand;
        if (!IsDraggable)
            return;
        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());
        transform.SetParent(DefaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;
        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + offset;

        if (TempCardGO.transform.parent != DefaultTempCardParent)
            TempCardGO.transform.SetParent(DefaultTempCardParent);

        CheckPosition();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;
        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(5000, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for(int i = 0;i<DefaultTempCardParent.childCount;i++)
        {
            if(transform.position.x<DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;
                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--; 
                break;
            }
        }

        TempCardGO.transform.SetSiblingIndex(newIndex);
    }

}

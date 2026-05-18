using System;
using System.Collections;
using System.Collections.Generic;
using FairyTales.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DashButton : TouchInputBase, IPointerDownHandler
{
    [SerializeField] private RectTransform dashIconRectTransform;
    
    public static readonly UnityEvent PointerDownEvent = new();

    private void OnEnable()
    {
        dashIconRectTransform.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!PlayerScript.Instance.IsDashCooldown)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(dashIconRectTransform, eventData.position,
                eventData.pressEventCamera, out var pressedPosition);
            dashIconRectTransform.position = pressedPosition;
            dashIconRectTransform.gameObject.SetActive(true);
        }
        
        PointerDownEvent.Invoke();
    }

    protected override void OnInputModeChanged(InputMode inputMode)
    {
        switch (inputMode)
        {
            case InputMode.Play:
                gameObject.SetActive(true);
                break;
            case InputMode.Event:
                gameObject.SetActive(false);
                break;
            case InputMode.GameOver:
                gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputMode), inputMode, null);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using FairyTales.EventSystem;
using FairyTales.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ConfirmButton : TouchInputBase, IPointerDownHandler
{
    public static readonly UnityEvent PointerDownEvent = new();

    protected override void OnInputModeChanged(InputMode inputMode)
    {
        switch (inputMode)
        {
            case InputMode.Play:
                gameObject.SetActive(false);
                break;
            case InputMode.Event:
                gameObject.SetActive(true);
                break;
            case InputMode.GameOver:
                gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputMode), inputMode, null);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent.Invoke();
    }
}
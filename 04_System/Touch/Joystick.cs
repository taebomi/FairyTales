using System;
using FairyTales.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Joystick : TouchInputBase, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform baseRectTransform, thumbRectTransform;
    [SerializeField] private float movementRange;

    public static readonly UnityEvent<Vector2> MoveUpdateEvent = new();
    public static readonly UnityEvent MoveFinishedEvent = new();

    private Vector3 _currentPosition, _downPosition;
    private bool _isPointerDownStarted; // OnDrag 유지 방지용도

    private void Start()
    {
        ActivateStick(false);
    }

    private void ActivateStick(bool willActivate)
    {
        baseRectTransform.gameObject.SetActive(willActivate);
        thumbRectTransform.gameObject.SetActive(willActivate);
        _isPointerDownStarted = willActivate;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(baseRectTransform, eventData.position,
            eventData.pressEventCamera, out _downPosition);
        baseRectTransform.position = thumbRectTransform.position = _downPosition;
        ActivateStick(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isPointerDownStarted) return;
        
        RectTransformUtility.ScreenPointToWorldPointInRectangle(thumbRectTransform, eventData.position,
            eventData.pressEventCamera, out _currentPosition);
        var delta = (Vector2)(_currentPosition - _downPosition);
        delta = Vector2.ClampMagnitude(delta, movementRange);
        thumbRectTransform.position = _downPosition + (Vector3)delta;
        MoveUpdateEvent.Invoke(delta / movementRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDisable();
    }

    private void OnDisable()
    {
        ActivateStick(false);
        MoveFinishedEvent.Invoke();
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
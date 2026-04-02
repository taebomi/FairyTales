using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(EmotionableObject),typeof(IInteractable))]
public class InteractableObject : MonoBehaviour
{
    public EmotionBubble.EmotionType emotionType;

    private EmotionableObject _emotionableObject;
    private IInteractable _interactable;

    private CancellationTokenSource _disableCts = new();

    private void Awake()
    {
        _emotionableObject = GetComponent<EmotionableObject>();
        _interactable = GetComponent<IInteractable>();
    }


    public void Interact()
    {
        if (_interactable.CanInteract)
        {
            _interactable.Interact();
        }
    }


    public async UniTaskVoid CheckInteractable()
    {
        _disableCts.CancelAndDispose();
        _disableCts = new CancellationTokenSource();
        while (true)
        {
            if (_interactable.CanInteract)
            {
                _emotionableObject.CreateEmotionBubble(emotionType);
            }
            else
            {
                _emotionableObject.RemoveEmotionBubble();
            }

            await UniTask.WaitUntilValueChanged(this,
                interactableObject => interactableObject._interactable.CanInteract,
                cancellationToken: _disableCts.Token);
        }
    }

    public void StopCheckInteractable()
    {
        _emotionableObject.RemoveEmotionBubble();
        _disableCts.Cancel();
    }

    private void OnDisable()
    {
        _disableCts.Cancel();
    }
}
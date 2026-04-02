using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FairyTales.Input;
using UnityEngine;
using UnityEngine.InputSystem;

// using UnityEngine.InputSystem;

public partial class PlayerScript
{
    public Vector2 CurrentInputDir { get; private set; }
    public Vector2 LastInputDir { get; set; }


    private void InitializeInput()
    {
        CurrentInputDir = Vector2.zero;
        LastInputDir = Vector2.zero;

        InputManager.MovePerformedEvent.AddListener(OnMovePerformed);
        InputManager.MoveCanceledEvent.AddListener(OnMoveCanceled);
        InputManager.DashButtonDownedEvent.AddListener(OnDashButtonDowned);
    }
    
    private void OnMovePerformed(Vector2 dir)
    {
        LastInputDir = CurrentInputDir = dir;
    }
    private void OnMoveCanceled()
    {
        CurrentInputDir = Vector2.zero;
    }
    
    private void OnDashButtonDowned()
    {
        CurrentState.OnDashButtonDown?.Invoke();
    }
}

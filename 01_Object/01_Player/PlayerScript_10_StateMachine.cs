using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerScript
{
    public PlayerStateBase CurrentState { get; private set; }
    public PlayerStateBase LastState { get; private set; }
    private PlayerStateBase[] _playerStates;


    private void InitializeStateMachine()
    {
        PlayerStateBase.Initialize(this);
        var numOfStates = Enum.GetValues(typeof(PlayerState)).Length;
        _playerStates = new PlayerStateBase[numOfStates];
        _playerStates[0] = new PlayerState_Move();
        _playerStates[1] = new PlayerState_Dash();
        _playerStates[2] = new PlayerState_Damaged();
        _playerStates[3] = new PlayerState_Died();
        _playerStates[4] = new PlayerState_Event();

        CurrentState = _playerStates[4];
        LastState = CurrentState;
    }

    public void ChangeState(PlayerState desiredState)
    {
        if (CurrentState.ThisEnum == desiredState)
        {
            Debug.LogAssertion("플레이어 상태 " + CurrentState + "에서 동일 상태로 변경 시도");
            return;
        }
        
        LastState = CurrentState;
        LastState.OnStateExit?.Invoke();

        Debug.Log("플레이어 상태 변경 : " + CurrentState.ThisEnum + " -> " + desiredState);
        
        CurrentState = _playerStates[(int)desiredState];
        CurrentState.OnStateEnter?.Invoke();
    }

    private void Update()
    {
        CurrentState.OnUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        CurrentState.OnFixedUpdate?.Invoke();
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        CurrentState.OnTriggerEnter?.Invoke(col);
    }
}
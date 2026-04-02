using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace FairyTales.Input
{
    public class InputManager : Singleton<InputManager>
    {
        public static readonly UnityEvent<Vector2> MovePerformedEvent = new();
        public static readonly UnityEvent MoveCanceledEvent = new();

        public static readonly UnityEvent DashButtonDownedEvent = new();
        public static readonly UnityEvent<Vector2> EventMovePerformedEvent = new();
        public static readonly UnityEvent ConfirmButtonDownedEvent = new();

        public static readonly UnityEvent<InputMode> InputModeChangedEvent = new();
        
        private PlayerInputActions _playerInputActions;


        protected override void AwakeAfter()
        {
            _playerInputActions = new PlayerInputActions();

            // 이동 입력 performed
            _playerInputActions.Control.Move.performed +=
                context => MovePerformedEvent.Invoke(context.ReadValue<Vector2>());
            Joystick.MoveUpdateEvent.AddListener(dir => MovePerformedEvent.Invoke(dir));

            // 이동 입력 canceled
            _playerInputActions.Control.Move.canceled += _ => MoveCanceledEvent.Invoke();
            Joystick.MoveFinishedEvent.AddListener(() => MoveCanceledEvent.Invoke());

            // 데쉬 입력
            _playerInputActions.Control.Dash.started += _ => DashButtonDownedEvent.Invoke();
            DashButton.PointerDownEvent.AddListener(() => DashButtonDownedEvent.Invoke());

            // 이벤트 키보드 조작
            _playerInputActions.Event.Move.performed +=
                context => EventMovePerformedEvent.Invoke(context.ReadValue<Vector2>());

            // 이벤트 모드 확인 입력
            _playerInputActions.Event.Confirm.started += _ => ConfirmButtonDownedEvent.Invoke();
            ConfirmButton.PointerDownEvent.AddListener(() => ConfirmButtonDownedEvent.Invoke());


            StageManager.StageStateChanged.AddListener(OnStageStateChanged);
        }


        private void OnStageStateChanged(StageState state)
        {
            switch (state)
            {
                case StageState.None:
                case StageState.FirstPlaying:
                    break;
                case StageState.Playing:
                    InputModeChangedEvent.Invoke(InputMode.Play);
                    _playerInputActions.Control.Enable();
                    _playerInputActions.Event.Disable();
                    break;
                case StageState.StaticEvent:
                    InputModeChangedEvent.Invoke(InputMode.Event);
                    _playerInputActions.Control.Disable();
                    _playerInputActions.Event.Enable();
                    break;
                case StageState.GameOver:
                    InputModeChangedEvent.Invoke(InputMode.GameOver);
                    _playerInputActions.Control.Disable();
                    _playerInputActions.Event.Disable();
                    break;
                case StageState.Reset:

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    public enum InputMode
    {
        Play,
        Event,
        GameOver,
    }
}
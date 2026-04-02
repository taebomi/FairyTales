using System;
using FairyTales.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


namespace FairyTales.EventSystem
{
    using EventSystem = UnityEngine.EventSystems.EventSystem;

    public partial class EventManager
    {
        private void InitializeInput()
        {
            InputManager.EventMovePerformedEvent.AddListener(OnEventMovePerformed);
            InputManager.ConfirmButtonDownedEvent.AddListener(OnConfirmButtonDowned);
        }

        private void OnConfirmButtonDowned()
        {
            switch (_eventMode)
            {
                case EventMode.None:
                    break;
                case EventMode.Dialogue:
                    ConfirmDialogue();
                    break;
                case EventMode.Choice:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEventMovePerformed(Vector2 dir)
        {
            switch (_eventMode)
            {
                case EventMode.None:
                    break;
                case EventMode.Dialogue:
                    break;
                case EventMode.Choice:
                    if (EventSystem.current.currentSelectedGameObject) return;

                    if (dir.y < 0 || dir.x > 0)
                    {
                        EventSystem.current.SetSelectedGameObject(choiceButtonArray[0].gameObject);
                    }
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(choiceButtonArray[_currentChoiceCount - 1]
                            .gameObject);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
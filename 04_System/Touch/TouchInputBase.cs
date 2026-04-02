using FairyTales.Input;
using UnityEngine;

public abstract class TouchInputBase : MonoBehaviour
{
    protected void Awake()
    {
        InputManager.InputModeChangedEvent.AddListener(OnInputModeChanged);
    }

    protected abstract void OnInputModeChanged(InputMode inputMode);
}

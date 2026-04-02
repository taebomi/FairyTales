using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject menu, cameraMenu;

    public void ActivateMenu()
    {
        menu.SetActive(!menu.activeSelf);
        cameraMenu.SetActive(false);
    }

    public void ActivateCameraMenu()
    {
        cameraMenu.SetActive(!cameraMenu.activeSelf);
    }

    private bool _isReminiscence = false;
    public void PlayReminiscence()
    {
        if (!_isReminiscence) CameraManager.Instance.PlayEffect_Reminiscence(1f, 1f).Forget();
        else CameraManager.Instance.PlayEffect_Reminiscence(0f,1f).Forget();
        _isReminiscence = !_isReminiscence;
    }
    
    
    
    #region 세이브 관련

    public void Save()
    {
        SaveManager.Instance.Save();
    }

    public void Load()
    {
        SaveManager.Instance.Load();
    }

    #endregion
}

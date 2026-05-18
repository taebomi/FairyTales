using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using FairyTales.EventSystem;
using UnityEngine;

public class SavePoint : StageObject, IInteractable
{
    [field: SerializeField] public string SavePointName { get; private set; }
    [field: SerializeField] public Vector2 PlayerRespawnPosition { get; private set; }
    [SerializeField] private EventInfo interactEventInfo;
    public bool CanInteract { get; private set; }
    private bool _isActivated;

    private CancellationTokenSource _disableCts = new();


    // 세이브 데이터로부터 자신이 마지막 세이브포인트인지 체크 후 활성화, 아닐 경우에는 비활성화 애니메이션
    private void OnEnable()
    {
        CanInteract = true;
        if (SaveManager.Instance.PlayerSaveData.savePointName == SavePointName)
        {
            _isActivated = true;
            SetActAnimation(AnimationType.Act_Activated);
        }
        else
        {
            _isActivated = false;
            SetActAnimation(AnimationType.Act_Deactivated);
        }
    }

    protected override void OnDisable()
    {
        _disableCts.Cancel();
    }

    public void Interact()
    {
        SetInteractableAfterTime(3f).Forget();
        if (_isActivated)
        {
            MainAnimator.SetTrigger(AnimatorHash.ActAgain);
        }
        else
        {
            _isActivated = true;
            SetActAnimation(AnimationType.Act_Activate);
        }

        SoundManager.Instance.PlaySoundEffect(SoundName.SavePoint_Activation).Forget();
        // todo - 체력 회복
        interactEventInfo.Begin();
        SaveManager.Instance.Save(SavePointName, PlayerRespawnPosition);
    }

    private async UniTaskVoid SetInteractableAfterTime(float time)
    {
        _disableCts.CancelAndDispose();
        _disableCts = new CancellationTokenSource();

        CanInteract = false;
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: _disableCts.Token);
        CanInteract = true;
    }
}
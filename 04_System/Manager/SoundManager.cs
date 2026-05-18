using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    #region 사운드 관련 클래스

    // 사운드 정보 딕셔너리
    [Serializable]
    public class SoundDictionary : SerializableDictionaryBase<SoundName, SoundInfo>
    {
    }

    // 사운드 정보
    [Serializable]
    public class SoundInfo
    {
        public AudioClip audioClip;
        public bool isPlaying; // 현재 플레이 중인지
    }

    #endregion


    [SerializeField] private AudioSource bgmMainAudioSource;
    [SerializeField] private AudioSource bgmSubAudioSource;
    [SerializeField] private AudioSource seAudioSource;

    private const float ChangingVolumeSpeed = 1f;
    private CancellationTokenSource _stopBGMCancellationTokenSource = new();

    [SerializeField, ID("audioClip")] private SoundDictionary soundDictionary = new();


    protected override void AwakeAfter()
    {
        Room.RoomChangeStartedEvent.AddListener(OnRoomChanged);
    }

    private void OnRoomChanged(bool instantly)
    {
        PlayBGM(Room.CurrentRoomData.bgm, instantly);
    }

    /// <summary>
    /// 브금 재생시키기
    /// </summary>
    /// <param name="soundName">재생시킬 브금 enum</param>
    /// <param name="playInstantly">즉시 할래?</param>
    public void PlayBGM(SoundName soundName, bool playInstantly)
    {
        if (!soundDictionary.TryGetValue(soundName, out var soundInfo))
        {
            Debug.LogWarning("사운드 존재하지 않는데 시도했어 ! 수정해 빨리 !");
            return;
        }

        if (bgmMainAudioSource.clip == soundInfo.audioClip)
        {
            return;
        }

        // 이전에 페이드 인/아웃 중이던 작업 취소
        _stopBGMCancellationTokenSource.Cancel();
        _stopBGMCancellationTokenSource.Dispose();
        _stopBGMCancellationTokenSource = new CancellationTokenSource();


        // 1. 메인과 서브 SWAP
        (bgmMainAudioSource, bgmSubAudioSource) = (bgmSubAudioSource, bgmMainAudioSource);

        // 2 - 1. 메인 오디오 소스 : 재생할 클립 설정 및 play, 볼륨 값 서서히 높이기
        bgmMainAudioSource.clip = soundInfo.audioClip;
        StartBGM(bgmMainAudioSource, playInstantly).Forget();

        // 2 - 2. 서브 오디오 소스 : 볼륨 값 서서히 낮추기, 0일 시 pause
        StopBgm(bgmSubAudioSource, playInstantly).Forget();
    }

    /// <summary>
    /// 브금 페이드 인
    /// </summary>
    private async UniTaskVoid StartBGM(AudioSource audioSource, bool playInstantly)
    {
        // 페이드 인아웃 중 다시 Play 시키면 처음부터 재생되므로 재생중이 아닐 때에만 Play시키는 조건
        // 다른 클립으로 변경되었을 경우 재생이 멈추었으므로 이 경우에도 Play됨
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        audioSource.volume = playInstantly ? 1f : 0f;

        while (audioSource.volume < 1f)
        {
            audioSource.volume += ChangingVolumeSpeed * Time.unscaledDeltaTime;
            await UniTask.Yield(_stopBGMCancellationTokenSource.Token);
        }
    }

    /// <summary>
    /// 브금 페이드 아웃
    /// </summary>
    private async UniTaskVoid StopBgm(AudioSource audioSource, bool playInstantly)
    {
        if (playInstantly)
        {
            audioSource.volume = 0f;
        }

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= ChangingVolumeSpeed * Time.unscaledDeltaTime;
            await UniTask.Yield(_stopBGMCancellationTokenSource.Token);
        }

        audioSource.Pause();
    }


    /// <summary>
    /// 효과음 재생 및 동시에 재생 방지
    /// </summary>
    public async UniTaskVoid PlaySoundEffect(SoundName soundName)
    {
        if (!soundDictionary.TryGetValue(soundName, out var soundInfo)) return; // 사운드 유무 확인
        if (soundInfo.isPlaying) return; // 동시 재생 방지

        soundInfo.isPlaying = true;
        
        seAudioSource.PlayOneShot(soundInfo.audioClip);
        await UniTask.Yield();

        soundInfo.isPlaying = false;
    }
}
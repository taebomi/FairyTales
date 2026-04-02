using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FairyTales.EventSystem;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace FairyTales.EventSystem
{
    public partial class EventManager
    {
        [Header("Dialogue", order = 2)] [SerializeField]
        private CanvasGroup dialogueCanvasGroup; // 대사 창 + 대사 캔버스 그룹

        [SerializeField] private RectTransform dialogueBoxRectTransform;
        [SerializeField] private TMP_Text dialogueTMPText; // 대사가 출력될 TMP Text

        private DialogueInfo _currentDialogueInfo; // 현재 대사 정보
        private DialogueBoxState _dialogueBoxState; // 현재 대사 상태

        // 대사창 효과
        private Tweener _dialogueActivateTweener; // 대사창 페이드인/아웃 트위너
        private Tweener _dialogueShakeTweener; // 대사창 흔들기
        private Tweener _dialogueFlowTweener; // 대사창 출렁이기

        private enum DialogueBoxState
        {
            Idle, // 유휴 상태
            FadingIn, // 창 서서히 나타남
            Typing, // 대사 출력 중
            Waiting, // 대사 출력 후 대기
            Finished, // 대사 출력 완료
            FadingOut, // 창 서서히 사라짐
        } // 대사 창 상태

        [Serializable]
        public struct DialogueInfo
        {
            public int id; // 대사 id
            public PlayMode playMode; // 대사 출력 방식
            public Effect effect; // 대사 효과
            public TextColor textColor; // 글자 색
            public float printInterval; // 글자 출력 텀
            public bool canSkip; // 스킵 가능한 대사인지
            public float pauseDuration; // + 스킵 가능한 경우 - 대사 출력 후 대기 시간 
            public double timeToJumpTo; // + 스킵 가능한 경우 - 대사 출력 후 클립 마지막 지점으로 이동하기 위함

            public enum PlayMode
            {
                OneByOne, // 한글자씩
                Instant, // 한번에 전체 출력
            }

            public enum Effect
            {
                None, // 효과 없음
                Shake, // 글자 흔들기
                Flow, // 글자 위아래로 오르락내리락
            }

            public enum TextColor
            {
                Player = 1000,
                Red = 100,
                DarkRed = 110,
                Purple = 800,
                White = 900,

                Navi = 1002,
            }
        } // 대사 정보

        private static readonly Dictionary<DialogueInfo.TextColor, Color> TextColorDict = new()
        {
            { DialogueInfo.TextColor.Player, new Color(0.5f, 0.5f, 0.5f) },
            { DialogueInfo.TextColor.Navi, new Color(0.990566f, 0.4065059f, 0.6868345f) },
            { DialogueInfo.TextColor.Red, new Color(0.6627451f, 0.1529412f, 0.2352941f) },
            { DialogueInfo.TextColor.DarkRed, new Color(0.4823529f, 0.05882353f, 0.1254902f) },
            { DialogueInfo.TextColor.Purple, new Color(0.6392157f, 0.3803922f, 0.7647059f) },
            { DialogueInfo.TextColor.White, new Color(1, 1, 1) },
        }; // 대사 색상표


        private const float StaticDialogueHeight = 240f;
        private const float DynamicDialogueHeight = 200f;

        [SerializeField] private TextAnimatorPlayer tAnimPlayer;


        /// <summary>
        /// 대사 시스템 초기화.
        /// 트위너 및 캔버스 꺼두는 용도
        /// </summary>
        private void InitializeDialogueSystem()
        {
            DeactivateDialogueSystem();

            _dialogueActivateTweener = dialogueCanvasGroup
                .DOFade(1, 0.25f)
                .From(0)
                .SetEase(Ease.InOutSine)
                .SetAutoKill(false)
                .OnComplete(() => PrintDialogueText_().Forget())
                .OnRewind(DeactivateDialogueSystem);

            _dialogueShakeTweener = dialogueTMPText.rectTransform
                .DOShakeAnchorPos(8f, new Vector2(7.5f, 7.5f))
                .SetEase(Ease.InOutSine)
                .SetAutoKill(false).Pause()
                .OnPause(() => { dialogueTMPText.rectTransform.anchoredPosition = Vector2.zero; });

            _dialogueFlowTweener = dialogueTMPText.rectTransform
                .DOAnchorPosY(20f, 0.75f)
                .SetLoops(-1)
                .SetEase(Ease.InOutSine)
                .SetAutoKill(false).Pause()
                .OnPause(() => { dialogueTMPText.rectTransform.anchoredPosition = Vector2.zero; });
        }

        private void DeactivateDialogueSystem()
        {
            dialogueCanvasGroup.alpha = 0;
            _dialogueBoxState = DialogueBoxState.Idle;
            dialogueTMPText.text = "";
        }


        /// <summary>
        /// 대사 정보를 받아서 대사 출력. 타임라인으로부터 호출됨.
        /// </summary>
        public void PrintDialogue(DialogueInfo dialogueInfo)
        {
            _currentDialogueInfo = dialogueInfo;
            _eventMode = EventMode.Dialogue;

            if (dialogueInfo.canSkip)
            {
                // playableDirector.Pause(); 로도 멈출 수 있으나 다시 재생시키거나 할 때 문제가 발생하던 것 같음.
                playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }


            switch (_dialogueBoxState)
            {
                case DialogueBoxState.Idle: // 대사창 나타남 (빈 대사창) -> 대사 출력
                    _dialogueBoxState = DialogueBoxState.FadingIn;
                    _dialogueActivateTweener.Restart();
                    break;
                case DialogueBoxState.FadingOut: // 대사창 사라지는 상태 -> 대사창 다시 나타나게 하기 -> 대사 출력
                    _dialogueBoxState = DialogueBoxState.FadingIn;
                    _dialogueActivateTweener.PlayForward();
                    if (_dialogueActivateTweener.IsComplete())
                    {
                        PrintDialogueText_().Forget();
                    }

                    break;
                case DialogueBoxState.FadingIn:
                case DialogueBoxState.Typing:
                case DialogueBoxState.Waiting:
                case DialogueBoxState.Finished:
                    Debug.LogWarning($"{transform.name} - 대사가 여기서 나오면 안되는데???");
                    PrintDialogueText_().Forget();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async UniTaskVoid PrintDialogueText_()
        {
            _dialogueBoxState = DialogueBoxState.Typing;

            var text = _currentEventData.dialogueSet[_currentDialogueInfo.id];

            dialogueTMPText.color = TextColorDict[_currentDialogueInfo.textColor];

            ApplyDialogueEffect();

            switch (_currentDialogueInfo.playMode) // 출력 모드에 따라 대사 출력
            {
                case DialogueInfo.PlayMode.OneByOne:
                    tAnimPlayer.waitForNormalChars = _currentDialogueInfo.printInterval;
                    tAnimPlayer.ShowText(text);
                    await tAnimPlayer.onTextShowed.OnInvokeAsync(CancellationToken.None);
                    break;
                case DialogueInfo.PlayMode.Instant:
                    dialogueTMPText.text = text;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // dialogueTMPText.text = text;
            

            _dialogueWaitCts ??= new CancellationTokenSource();
            switch (_currentDialogueInfo.canSkip) // 스킵 여부에 따라 대기
            {
                case true:
                    _dialogueBoxState = DialogueBoxState.Waiting;
                    await UniTask
                        .Delay(TimeSpan.FromSeconds(_currentDialogueInfo.pauseDuration),
                            cancellationToken: _dialogueWaitCts.Token).SuppressCancellationThrow();
                    playableDirector.time = _currentDialogueInfo.timeToJumpTo;
                    playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
                    break;
                case false:
                    _dialogueBoxState = DialogueBoxState.Finished;
                    break;
            }
        }


        public void FinishDialogue()
        {
            _dialogueBoxState = DialogueBoxState.FadingOut;
            _eventMode = EventMode.None;

            _dialogueActivateTweener.PlayBackwards();
        }

        private void ApplyDialogueEffect()
        {
            switch (_currentDialogueInfo.effect)
            {
                case DialogueInfo.Effect.None:
                    _dialogueShakeTweener.Pause();
                    _dialogueFlowTweener.Pause();
                    break;
                case DialogueInfo.Effect.Shake:
                    _dialogueShakeTweener.Restart();
                    _dialogueFlowTweener.Pause();
                    break;
                case DialogueInfo.Effect.Flow:
                    _dialogueFlowTweener.Restart();
                    _dialogueShakeTweener.Pause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private CancellationTokenSource _dialogueWaitCts = new();

        private void ConfirmDialogue()
        {
            if (!_currentDialogueInfo.canSkip) return;

            switch (_dialogueBoxState)
            {
                case DialogueBoxState.Idle:
                case DialogueBoxState.FadingIn:
                case DialogueBoxState.Finished:
                case DialogueBoxState.FadingOut:
                    break;
                case DialogueBoxState.Typing: // 대사 타이핑 중인 경우 취소하고 전체 대사 출력
                    // _printDialogueTextCts.Cancel();
                    // _printDialogueTextCts = null;
                    tAnimPlayer.SkipTypewriter();
                    break;
                case DialogueBoxState.Waiting: // 대사 전체 출력 후 지속시간동안 대기중인 상태 취소
                    _dialogueWaitCts.Cancel();
                    _dialogueWaitCts = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } // 확인 버튼 눌렀을 때 대사 상태에 따른 처리
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace FairyTales.EventSystem
{
    public partial class EventManager
    {
        [Header("Choice")] [SerializeField] private CanvasGroup choiceCanvasGroup; // 선택지 캔버스 그룹
        [SerializeField] private ChoiceButton[] choiceButtonArray; // 선택지 버튼 배열

        private TweenerCore<float, float, FloatOptions> _choiceActivateTweener; // 선택지 활성화/비활성화 트위너

        private double[] _timeToJumpToSelectedChoice; // 각 선택지에 따른 점프해야되는 시간 저장 배열
        private int _currentChoiceCount;

        private static readonly UnityEvent<int> ChoiceBtnClickEvent = new();

        [Serializable]
        public struct ChoiceInfo
        {
            public ChoiceButton.Color color;
            public ChoiceButton.Icon icon;
        }

        /// <summary>
        /// 선택지 관련 시스템 초기화.
        /// 시작 시 비활성화 및 트위너 초기화
        /// </summary>
        private void InitializeChoiceSystem()
        {
            // 일단 오브젝트 비활성화해두기
            choiceCanvasGroup.alpha = 0;
            choiceCanvasGroup.interactable = false;
            choiceCanvasGroup.blocksRaycasts = false;

            ChoiceBtnClickEvent.AddListener(delegate { OnChoiceBtnClick(); });

            // 버튼 Clicck 이벤트 추가
            for (var index = 0; index < choiceButtonArray.Length; index++)
            {
                var num = index;
                choiceButtonArray[index].GetComponent<Button>().onClick
                    .AddListener(() => ChoiceBtnClickEvent.Invoke(num));
                choiceButtonArray[index].gameObject.SetActive(false);
            }

            // 선택지 활성화/비활성화 트위너 설정
            _choiceActivateTweener =
                choiceCanvasGroup
                    .DOFade(1f, 0.25f)
                    .From(0f)
                    .SetAutoKill(false);
        }

        private void OnChoiceBtnClick()
        {
            choiceCanvasGroup.interactable = false;
            choiceCanvasGroup.blocksRaycasts = false;
            ChangeEventMode(EventMode.None);
            _choiceActivateTweener.PlayBackwards();
        }

        public async UniTaskVoid PrintChoice(int choiceId, ChoiceInfo[] choiceInfos)
        {
            var choiceTexts = _currentEventData.choiceSet[choiceId];
            _currentChoiceCount = choiceTexts.Length;

            // 버튼 개수에 따른 위치설정
            switch (_currentChoiceCount)
            {
                case 1:
                    choiceButtonArray[0].rectTransform.anchoredPosition = new Vector2(0, -300f);
                    break;
                case 2:
                    choiceButtonArray[0].rectTransform.anchoredPosition = new Vector2(0, -200f);
                    choiceButtonArray[1].rectTransform.anchoredPosition = new Vector2(0, -350f);
                    break;
                case 3:
                    choiceButtonArray[0].rectTransform.anchoredPosition = new Vector2(0, -150f);
                    choiceButtonArray[1].rectTransform.anchoredPosition = new Vector2(0, -300f);
                    choiceButtonArray[2].rectTransform.anchoredPosition = new Vector2(0, -450f);
                    break;
                case 4:
                    choiceButtonArray[0].rectTransform.anchoredPosition = new Vector2(0, -100f);
                    choiceButtonArray[1].rectTransform.anchoredPosition = new Vector2(0, -225f);
                    choiceButtonArray[2].rectTransform.anchoredPosition = new Vector2(0, -350f);
                    choiceButtonArray[3].rectTransform.anchoredPosition = new Vector2(0, -475f);
                    break;
            }

            // 활성화 될 선택지들 세팅
            for (var index = 0; index < _currentChoiceCount; index++)
            {
                choiceButtonArray[index]
                    .SetButton(choiceTexts[index], choiceInfos[index].color, choiceInfos[index].icon);
                // index 음수 방지, 이전 버튼 (-1일경우 제일 마지막 버튼으로 워프)
                var prevIndex = ((index - 1) % _currentChoiceCount + _currentChoiceCount) % _currentChoiceCount;
                // index 초과 방지, 다음 버튼 (length+1 경우 제일 처음 버튼으로 워프)
                var nextIndex = (index + 1) % _currentChoiceCount;
                choiceButtonArray[index]
                    .SetNavigation(choiceButtonArray[prevIndex].button, choiceButtonArray[nextIndex].button);
                choiceButtonArray[index].gameObject.SetActive(true);
            }

            // 선택지 없는 버튼들 비활성화
            for (var index = _currentChoiceCount; index < choiceButtonArray.Length; index++)
            {
                choiceButtonArray[index].gameObject.SetActive(false);
            }

            // 선택지 세팅 완료 후 활성화 트위너 실행, 완료 시 버튼 활성화
            _choiceActivateTweener.Restart();
            await _choiceActivateTweener.AwaitForComplete(cancellationToken: CancellationToken.None);
            choiceCanvasGroup.interactable = true;
            choiceCanvasGroup.blocksRaycasts = true;
            ChangeEventMode(EventMode.Choice);
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace FairyTales.EventSystem
{
    public partial class EventManager
    {
        [Header("Letterbox")] [SerializeField] private Canvas letterboxCanvas;
        [SerializeField] private RectTransform[] letterboxRectTransformArr; // 레터박스 위/아래 rectTransform

        private CancellationTokenSource _letterboxCts = new();

        #region 상수값

        private static readonly Vector2 LetterboxSize = new(0, 60f);
        private const float LetterboxSpeed = 0.5f;

        #endregion

        private void InitializeLetterbox()
        {
            letterboxCanvas.enabled = false;
            foreach (var rectTransform in letterboxRectTransformArr)
            {
                rectTransform.sizeDelta = Vector2.zero;
            }
        }

        // 시퀀스 재사용 안하는 이유는 await이랑 사용 시 문제 발생
        private async UniTask ToggleLetterbox(bool willActivate)
        {
            _letterboxCts.Cancel();
            _letterboxCts = new CancellationTokenSource();
            if (willActivate)
            {
                letterboxCanvas.enabled = true;
                await DOTween.Sequence()
                    .Join(letterboxRectTransformArr[0].DOSizeDelta(LetterboxSize, LetterboxSpeed))
                    .Join(letterboxRectTransformArr[1].DOSizeDelta(LetterboxSize, LetterboxSpeed))
                    .SetEase(Ease.InOutBounce)
                    .SetSpeedBased(true)
                    .Play().ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, _letterboxCts.Token);
            }
            else
            {
                await DOTween.Sequence()
                    .Join(letterboxRectTransformArr[0].DOSizeDelta(Vector2.zero, LetterboxSpeed))
                    .Join(letterboxRectTransformArr[1].DOSizeDelta(Vector2.zero, LetterboxSpeed))
                    .SetEase(Ease.InOutBounce)
                    .SetSpeedBased(true)
                    .Play().ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, _letterboxCts.Token);
                letterboxCanvas.enabled = false;
            }
        }
    }
}
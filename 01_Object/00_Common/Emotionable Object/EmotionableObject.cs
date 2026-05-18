using UnityEngine;

/// <summary>
/// 감정표현 이모티콘 나오는 오브젝트일 경우 이 컴포넌트 부착
/// </summary>
public class EmotionableObject : MonoBehaviour
{
    [SerializeField] private Transform emotionBubbleContainerTransform;

    private EmotionBubble _emotionBubble;

    /// <summary>
    /// 감정 풍선 생성시키기
    /// </summary>
    /// <param name="emotionType">보여줄 감정 타입</param>
    public void CreateEmotionBubble(EmotionBubble.EmotionType emotionType)
    {
        if (_emotionBubble) _emotionBubble.Close(); // 기존 감정표현 제거

        _emotionBubble = StageManager.Instance.GetEmotionBubble();
        _emotionBubble.Create(emotionType, emotionBubbleContainerTransform);
    }

    /// <summary>
    /// 감정 풍선 제거
    /// </summary>
    public void RemoveEmotionBubble()
    {
        if (!_emotionBubble) return;

        _emotionBubble.Close();
        _emotionBubble = null;
    }
}
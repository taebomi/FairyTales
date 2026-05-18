using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public partial class StageManager
{
    [field: SerializeField] public Transform PoolContainer { get; private set; }
    
    private Transform _emotionBubbleContainer;

    private void InitializeCommonObject()
    {
        _emotionBubbleContainer = new GameObject("EmotionBubble Container").transform;
        _emotionBubbleContainer.SetParent(PoolContainer);
        _emotionBubblePool = new ObjectPool<EmotionBubble>
            (CreateEmotionBox, OnGetEmotionBubble, OnReleaseEmotionBubble, OnDestroyEmotionBox, maxSize: 3);
    }

    #region Emotion Bubble

    [SerializeField] private EmotionBubble emotionBubblePrefab;
    private IObjectPool<EmotionBubble> _emotionBubblePool;

    private EmotionBubble CreateEmotionBox()
    {
        var emotionBox = Instantiate(emotionBubblePrefab, _emotionBubbleContainer);
        emotionBox.SetManagedPool(_emotionBubblePool);
        return emotionBox;
    }

    private static void OnGetEmotionBubble(EmotionBubble emotionBubble)
    {
        emotionBubble.gameObject.SetActive(true);
    }

    private void OnReleaseEmotionBubble(EmotionBubble emotionBubble)
    {
        emotionBubble.gameObject.SetActive(false);
        emotionBubble.transform.SetParent(_emotionBubbleContainer);
    }

    private static void OnDestroyEmotionBox(EmotionBubble emotionBubble)
    {
        Destroy(emotionBubble.gameObject);
    }

    public EmotionBubble GetEmotionBubble()
    {
        return _emotionBubblePool.Get();
    }

    #endregion
}
using DG.Tweening;
using UnityEngine;

public partial class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public bool IsDebugMode { get; private set; }
    
    
    
    
    protected override void AwakeAfter()
    {
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 60;
        
        
        DOTween.SetTweensCapacity(500,50);
    }
}

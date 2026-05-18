using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FairyTales.EventSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D.IK;
using Random = UnityEngine.Random;

public partial class AlrauneTutorial : StageObject
{
    private CancellationTokenSource _disableCts;

    protected static readonly int EyeBlinkTrigger = Animator.StringToHash(nameof(EyeBlinkTrigger));

    private int _attackedCount;


    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private static bool _readyToAct;
    private static WaitUntil _waitUntilReadyToAct = new(() => _readyToAct);

    private Transform _playerTransform;

    private Vector3 transformPosition;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private GameObject ikManagerGameObject;


    // 풀링
    private static Transform _poolingObjectContainer;

    private void InitializePool()
    {
        _poolingObjectContainer = new GameObject("Alraune Pool").transform;
        _poolingObjectContainer.SetParent(StageManager.Instance.PoolContainer);
        _poolingObjectContainer.position = transformPosition;
    }

    private void Awake()
    {
        _playerTransform = PlayerScript.Instance.transform;
        transformPosition = transform.position;
        InitializePool();
        InitializeLaser();
        InitializeDanmaku();
        InitializeThorn();
        InitializeWall();
    }

    private void OnEnable()
    {
        _disableCts?.Dispose();
        _disableCts = new CancellationTokenSource();
        _readyToAct = true;
        StartCoroutine(BlinkEyes());
        StartCoroutine(CheckPlayerDirection());
        
        EventManager.EventFinishedEvent.AddListener(CheckEvent);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        EventManager.EventFinishedEvent.RemoveListener(CheckEvent);
        _disableCts.Cancel();
    }

    private void LateUpdate()
    {
        LaserLateUpdate();
    }

    private void CheckEvent(string eventName)
    {
        if (eventName == "Navi Kidnapped")
        {
            StartTutorial().Forget();
        }
    }

    private IEnumerator BlinkEyes()
    {
        var cool = Random.Range(0.5f, 3f);

        while (true)
        {
            if (cool < 0)
            {
                cool = Random.Range(0.5f, 3f);
                MainAnimator.SetTrigger(EyeBlinkTrigger);
            }

            cool -= Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator CheckPlayerDirection()
    {
        var isRight = true;
        while (true)
        {
            yield return null;

            if (!_readyToAct)
            {
                continue;
            }

            var direction = PlayerScript.Instance.transform.position.x - transformPosition.x;
            switch (direction)
            {
                case < -0.5f:
                    yield return YieldInstructionCache.WaitForSeconds(0.3f);
                    if (isRight)
                    {
                        MainAnimator.enabled = false;
                        ikManagerGameObject.SetActive(false);

                        yield return mainTransform.DOScale(-1f, 0.25f).SetOptions(AxisConstraint.X).Play()
                            .WaitForCompletion();
                        ikManagerGameObject.SetActive(true);
                        MainAnimator.enabled = true;
                        isRight = false;
                    }

                    break;
                case > 0.5f:
                    yield return YieldInstructionCache.WaitForSeconds(0.3f);
                    if (!isRight)
                    {
                        MainAnimator.enabled = false;
                        ikManagerGameObject.SetActive(false);
                        yield return mainTransform.DOScale(1f, 0.25f).SetOptions(AxisConstraint.X).Play()
                            .WaitForCompletion();
                        ikManagerGameObject.SetActive(true);
                        MainAnimator.enabled = true;
                        isRight = true;
                    }

                    break;
            }
        }
    }

    [SerializeField] private EventInfo laserHitEvent, laserAvoidEvent; 

    public async UniTaskVoid StartTutorial()
    {
        var isPlayerDamaged = false;
        var playerDamageCheck = new UnityAction(() => isPlayerDamaged = true);
        PlayerScript.Instance.HealthSystem.OnPlayerDamaged.AddListener(playerDamageCheck);
        await Attack_LaserQuickShot();
        PlayerScript.Instance.HealthSystem.OnPlayerDamaged.RemoveListener(playerDamageCheck);
        await UniTask.Delay(1500, cancellationToken: _disableCts.Token);
        if (isPlayerDamaged)
        {
            laserHitEvent.Begin();
        }
        else
        {
            laserAvoidEvent.Begin();
        }


        await UniTask.Yield();
    }

    public void StartEvent()
    {
        StartTutorial().Forget();
    }


    #region 애니메이션 관련

    #endregion

    #region 디버깅용도

    private int test = 0;

    public void DebugButton()
    {

        test--;
        if (test < 0)
        {
            test = 8;
        }
        DebugButton3();

    }

    public void DebugButton2()
    {
        test++;
        if (test > 8)
        {
            test = 0;
        }
        DebugButton3();
    }

    public void DebugButton3()
    {
        switch (test)
        {
            case 0:

                Attack_LaserQuickShot().Forget();

                break;
            case 1:
                Attack_LaserXRotate().Forget();
                break;
            case 2:
                Attack_LaserWithWall().Forget();
                break;
            case 3:
                Attack_DanmakuSpread().Forget();
                break;
            case 4:
                Attack_DanmakuCircleCage().Forget();
                break;
            case 5:
                Attack_ThornInLine(1).Forget();
                break;
            case 6:
                Attack_ThornInLine(2).Forget();
                break;
            case 7:
                Attack_ThornInLine(3).Forget();
                break;
            case 8:
                Attack_ThornInCircle().Forget();
                break;
        }
    }
    #endregion
}
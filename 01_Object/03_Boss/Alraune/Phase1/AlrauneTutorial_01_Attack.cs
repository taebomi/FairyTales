using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FairyTales.EventSystem;
using FairyTales.Layer;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

// 공격 패턴 관련
public partial class AlrauneTutorial
{
    [SerializeField] private Transform muzzleTransform; // 무기의 공격 나오는 위치

    private enum AttackPattern
    {
        None = -1, // 정지
        Idle = 0, // 아이들 모션

        // 레이저
        LaserXRotate = 100, // 레이저 4방향 회전 
        LaserWithWall = 110, // 벽 세우고 레이저 연사
        LaserQuickShot = 120, // 빠른 레이저 공격 

        // 가시
        ThornDiffusion = 200, // 가시 확산
        ThornInLine = 210, // 가시 일자 공격

        // 탄막
        DanmakuSpread = 300, // 탄막 퍼뜨리기 및 추적

        DanmakuCircleCage = 310, // 탄막 감옥
        DanmakuCircleCageUp = 311,
        DanmakuCircleCageDown = 312,
        DanmakuCircleCageLeft = 313,
        DanmakuCircleCageRight = 314,
    } // 공격 패턴 종류


    private void OnAttackStarted(AnimationType faceType, AttackPattern attackType, bool willBodyMove = true)
    {
        _readyToAct = false;
        SetFaceAnimation(faceType);
        MainAnimator.SetInteger(AnimatorHash.Attack, (int)attackType);
        MainAnimator.SetBool(AnimatorHash.OptionBool, willBodyMove);
    }

    private void OnAttackFinished()
    {
        SetFaceAnimation(AnimationType.Face_Idle);
        MainAnimator.SetBool(AnimatorHash.OptionBool, false);
        MainAnimator.SetInteger(AnimatorHash.Attack, (int)AttackPattern.Idle);
        _readyToAct = true;
    }

    #region 탄막

    #region 풀링

    [SerializeField] private AlrauneDanmaku danmakuPrefab;
    private static IObjectPool<AlrauneDanmaku> _danmakuPool;

    private void InitializeDanmaku()
    {
        _danmakuPool = new ObjectPool<AlrauneDanmaku>(CreateDanmaku, OnGetDanmaku, OnReleaseDanmaku,
            OnDestroyDanmaku, defaultCapacity: 300, maxSize: 500);
    }

    private AlrauneDanmaku CreateDanmaku()
    {
        var danmaku = Instantiate(danmakuPrefab, _poolingObjectContainer);
        danmaku.SetManagedPool(_danmakuPool);
        return danmaku;
    }

    private static void OnGetDanmaku(AlrauneDanmaku danmaku)
    {
        danmaku.gameObject.SetActive(true);
    }

    private static void OnReleaseDanmaku(AlrauneDanmaku danmaku)
    {
        danmaku.gameObject.SetActive(false);
    }

    private static void OnDestroyDanmaku(AlrauneDanmaku danmaku)
    {
        Destroy(danmaku.gameObject);
    }

    #endregion


    #region 플레이어 탄막 원 안에 가두고 위아래 이동시키기

    private async UniTaskVoid Attack_DanmakuCircleCage()
    {
        // 동그라미 그리는 모션까지 대기
        OnAttackStarted(AnimationType.Face_Laugh, AttackPattern.DanmakuCircleCage, false);
        const float createDanmakuCageNormalizedTime = 0.433333f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, createDanmakuCageNormalizedTime);
        MainAnimator.SetInteger(AnimatorHash.Attack, (int)AttackPattern.Idle);

        var danmakuControlCts = new CancellationTokenSource();

        var danmakus = await CreateDanmakuCage(danmakuControlCts);

        // 잠깐 대기 후 탄막감옥 플레이어 추적 서서히 정지
        await UniTask.Delay(500);

        danmakuControlCts.Cancel();
        foreach (var danmaku in danmakus)
        {
            danmaku.CancelKeepDistanceFromPlayer().Forget();
        }

        CameraManager.Instance.ChangeCameraMode(CameraManager.CameraMode.Manual);
        CameraManager.Instance.MoveTo(transformPosition + Vector3.up, 2f).Forget();

        await UniTask.Delay(3000);
        SetFaceAnimation(AnimationType.Face_HalfClosedEyes);

        for (var i = 0; i < 10; i++)
        {
            ChangeDirectionOfDanmakuCage(danmakus).Forget();
            await UniTask.Delay(Random.Range(750, 1750));
        }

        foreach (var danmaku in danmakus)
        {
            danmaku.ReleaseAfterFadeIn().Forget();
        }
        
        await CameraManager.Instance.MoveTo(PlayerScript.Instance.transform.position, 2f);
        CameraManager.Instance.ChangeCameraMode(CameraManager.CameraMode.ChasePlayer);

        OnAttackFinished();
    }

    private async UniTaskVoid ChangeDirectionOfDanmakuCage(List<AlrauneDanmaku> danmakus)
    {
        var playerPos = PlayerScript.Instance.transform.position;

        var dir = AttackPattern.Idle;
        var velocity = Vector2.up;

        if (playerPos.x > transformPosition.x + 4)
        {
            dir = AttackPattern.DanmakuCircleCageLeft;
        }
        else if (playerPos.x < transformPosition.x - 4)
        {
            dir = AttackPattern.DanmakuCircleCageRight;
        }
        else if (playerPos.y > transformPosition.y + 3)
        {
            dir = AttackPattern.DanmakuCircleCageDown;
        }
        else if (playerPos.y < transformPosition.y - 3)
        {
            dir = AttackPattern.DanmakuCircleCageUp;
        }

        if (dir == AttackPattern.Idle)
        {
            dir = (AttackPattern)Random.Range((int)AttackPattern.DanmakuCircleCageUp,
                (int)AttackPattern.DanmakuCircleCageRight + 1);
        }

        switch (dir)
        {
            case AttackPattern.DanmakuCircleCageUp:
                velocity = Vector2.up;
                break;
            case AttackPattern.DanmakuCircleCageDown:
                velocity = Vector2.down;
                break;
            case AttackPattern.DanmakuCircleCageLeft:
                velocity = Vector2.left;
                break;
            case AttackPattern.DanmakuCircleCageRight:
                velocity = Vector2.right;
                break;
        }

        velocity *= Random.Range(1f, 1.5f);

        MainAnimator.SetInteger(AnimatorHash.Attack, (int)dir);
        await UniTask.Delay(500);

        foreach (var danmaku in danmakus)
        {
            danmaku.SetVelocity(velocity);
        }
    }

    private AttackPattern CheckDanmakuCageDirection(Vector2 currentPos)
    {
        var pos = currentPos - (Vector2)transformPosition;

        if (pos.y >= pos.x)
        {
            return pos.y >= -pos.x ? AttackPattern.DanmakuCircleCageDown : AttackPattern.DanmakuCircleCageRight;
        }
        else
        {
            return pos.y >= -pos.x ? AttackPattern.DanmakuCircleCageLeft : AttackPattern.DanmakuCircleCageUp;
        }
    }

    private async UniTask<List<AlrauneDanmaku>> CreateDanmakuCage(CancellationTokenSource cts)
    {
        var danmakus = new List<AlrauneDanmaku>();

        const int numOfDanmaku = 30;
        const float theta = 2 * Mathf.PI / numOfDanmaku;
        var positionOnCircleRound = new Vector3(0f, 1.5f, 0f);
        for (var i = 0; i < numOfDanmaku; i++)
        {
            var currentTheta = theta * i;
            var r = 0.4f * Mathf.Sin(5 * currentTheta) + 2.4f;
            var x = r * Mathf.Cos(currentTheta);
            var y = r * Mathf.Sin(currentTheta);
            var flowerDanmaku = _danmakuPool.Get();

            flowerDanmaku.SetColor(AlrauneDanmaku.DanmakuColor.Purple);
            flowerDanmaku.FadeOut().Forget();
            flowerDanmaku.KeepDistanceFromPlayer(new Vector2(x, y), cts).Forget();


            positionOnCircleRound = Quaternion.Euler(0, 0, -12f) * positionOnCircleRound;

            var circleDanmaku = _danmakuPool.Get();
            circleDanmaku.SetColor(AlrauneDanmaku.DanmakuColor.Yellow);
            circleDanmaku.FadeOut().Forget();
            circleDanmaku.KeepDistanceFromPlayer(positionOnCircleRound, cts).Forget();

            danmakus.Add(flowerDanmaku);
            danmakus.Add(circleDanmaku);
            await UniTask.Delay(80);
        }

        return danmakus;
    }

    #endregion

    #region 탄막 확산 + 추적패턴

    /// <summary>
    /// 양 옆으로 춤추며 탄막 흩뿌리고 한쪽 탄막은 플레이어 추적하는 패턴 실행
    /// </summary>
    private async UniTaskVoid Attack_DanmakuSpread()
    {
        OnAttackStarted(AnimationType.Face_Idle, AttackPattern.DanmakuSpread);

        // 홀수 라운드는 추적, 짝수 라운드는 일반 탄막 생성
        for (var round = 0; round < 20; round++)
        {
            await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Attack, (round + 1) * 0.5f);

            var willChasePlayer = Convert.ToBoolean(round % 2);
            CreateSpreadingDanmakus(round, willChasePlayer).Forget();
        }

        OnAttackFinished();
    }

    /// <summary>
    /// round에 따라 회전된 퍼지는 탄막 생성, 파라미터에 따라 일정시간 이후 추적
    /// </summary>
    private async UniTaskVoid CreateSpreadingDanmakus(int round, bool willChasePlayer)
    {
        // 라운드에 따라 각도 변경
        var velocity = Quaternion.Euler(0, 0, 10f * round) * Vector3.up;

        // 라운드에 따라 빨강 제외한 색 순회
        const int numOfColor = 4;
        var color = (AlrauneDanmaku.DanmakuColor)(round % (numOfColor - 1) + 1);

        // 탄막 
        const int numOfDanmaku = 12;
        var danmakus = new Stack<AlrauneDanmaku>(numOfDanmaku);
        for (var i = 0; i < numOfDanmaku; i++)
        {
            var danmaku = _danmakuPool.Get();
            danmaku.SetPosition(muzzleTransform.position);
            danmaku.SetVelocity(velocity);
            danmaku.SetColor(color);
            danmaku.SetLightIntensity(1f);
            danmakus.Push(danmaku);

            velocity = Quaternion.Euler(0, 0, 30f) * velocity;
        }

        SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Danmaku_Make).Forget();

        // 추적 탄막인 경우 일정시간 후 추적
        // 일반 탄막 및 추적 이후 일정시간 대기 후 풀링
        if (willChasePlayer)
        {
            await UniTask.Delay(1500);
            var playerPos = PlayerScript.Instance.GetFlyingPosition();
            foreach (var danmaku in danmakus)
            {
                Vector2 chasingVelocity = (playerPos - danmaku.transform.position).normalized * 8f;
                danmaku.SetVelocity(chasingVelocity);
                danmaku.SetColor(AlrauneDanmaku.DanmakuColor.Red);
            }

            SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Danmaku_Move).Forget();
            await UniTask.Delay(3000);
        }
        else
        {
            await UniTask.Delay(13000);
        }

        foreach (var danmaku in danmakus)
        {
            _danmakuPool.Release(danmaku);
        }
    }

    #endregion

    #endregion

    #region 레이저 패턴
    
    [SerializeField] private AnimationCurve laserIntervalProbabilityCurve;

    // 레이저 컨테이너 관련
    [SerializeField] private Transform laserContainerTransform;
    [SerializeField] private Animator laserChargingEffectAnimator;
    [SerializeField] private ParticleSystem laserChargingEffectParticleSystem;
    [SerializeField] private Light2D laserChargingEffectLight;

    #region 레이저 이펙트

    private CancellationTokenSource _laserEffectCts;
    

    #endregion

    private static int _numOfActiveLaser = 0;

    #region 풀링

    [SerializeField] private AlrauneLaser laserPrefab;
    private static IObjectPool<AlrauneLaser> _laserPool;

    private void InitializeLaser()
    {
        _laserPool = new ObjectPool<AlrauneLaser>(CreateLaser, OnGetLaser, actionOnRelease: OnReleaseLaser,
            actionOnDestroy: OnDestroyLaser, defaultCapacity: 12, maxSize: 20);
        laserChargingEffectLight.intensity = 0f;
        _laserEffectCts = new CancellationTokenSource();
    }


    private AlrauneLaser CreateLaser()
    {
        var laser = Instantiate(laserPrefab, laserContainerTransform);
        laser.SetManagedPool(_laserPool);
        return laser;
    }

    private void OnGetLaser(AlrauneLaser laser)
    {
        if (_numOfActiveLaser++ == 0)
        {
            ActivateLaserChargingEffect(true);
        }

        laser.transform.SetParent(laserContainerTransform, false);
        laser.gameObject.SetActive(true);
    }

    private void OnReleaseLaser(AlrauneLaser laser)
    {
        laser.gameObject.SetActive(false);
        laser.transform.SetParent(_poolingObjectContainer, false);
    }

    private static void OnDestroyLaser(AlrauneLaser laser)
    {
        Destroy(laser.gameObject);
    }

    #endregion

    #region 레이저 사용

    private void RemoveLaser(AlrauneLaser laser)
    {
        if (--_numOfActiveLaser == 0)
        {
            ActivateLaserChargingEffect(false);
        }

        laser.Remove().Forget();
    }

    private void ActivateLaserChargingEffect(bool willActivate)
    {
        if (willActivate)
        {
            if (laserChargingEffectParticleSystem.isEmitting)
            {
                return;
            }

            laserChargingEffectParticleSystem.gameObject.SetActive(true);
            PlayLightEffectForLaserCharge().Forget();
            SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Laser_Charge).Forget();
        }
        else
        {
            laserChargingEffectParticleSystem.Stop(true);
        }
    }

    private async UniTaskVoid PlayLightEffectForLaserCharge()
    {
        _laserEffectCts.Cancel();
        _laserEffectCts = new CancellationTokenSource();
        var currentIntensity = laserChargingEffectLight.intensity;
        const float changingSpeed = 4f;

        while (currentIntensity < 1.25f && laserChargingEffectParticleSystem.isEmitting)
        {
            currentIntensity += changingSpeed * Time.deltaTime;
            laserChargingEffectLight.intensity = currentIntensity;
            await UniTask.Yield(_laserEffectCts.Token);
        }

        var currentTime = 0f;
        while (laserChargingEffectParticleSystem.isEmitting)
        {
            currentTime += Time.deltaTime;
            laserChargingEffectLight.intensity = 1.25f + Mathf.Sin(currentTime) * 0.25f;
            await UniTask.Yield(_laserEffectCts.Token);
        }

        while (currentIntensity > 0f)
        {
            currentIntensity -= changingSpeed * Time.deltaTime;
            laserChargingEffectLight.intensity = currentIntensity;
            await UniTask.Yield(_laserEffectCts.Token);
        }

        laserChargingEffectLight.intensity = 0f;
    }

    private void LaserLateUpdate()
    {
        laserContainerTransform.position = muzzleTransform.position;
    }

    #endregion

    #region 레이저 퀵 샷 패턴

    private async UniTask Attack_LaserQuickShot()
    {
        OnAttackStarted(AnimationType.Face_Laugh, AttackPattern.LaserQuickShot);

        var attackingNumber = Random.Range(1, 4);
        for (var attackingCount = 0; attackingCount < attackingNumber; attackingCount++)
        {
            // 대기 애니메이션이 되면 랜덤 딜레이 만큼 대기
            await MainAnimator.AsyncWaitForStart(AnimatorHash.Wait);
            SetFaceAnimation(AnimationType.Face_Scoff);
            var randomDelay = laserIntervalProbabilityCurve.Evaluate(Random.value);
            await UniTask.Delay(TimeSpan.FromSeconds(randomDelay));

            // 공격모션 시작 및 레이저 위험 표시
            MainAnimator.SetTrigger(AnimatorHash.AttackAgain);
            var laser = _laserPool.Get();
            var laserDirection = PlayerScript.Instance.GetFlyingPosition() - muzzleTransform.position;
            laser.SetRotation(laserDirection);
            laser.Warn();

            // 공격 타이밍까지 대기 후 레이저 공격으로 전환
            await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Attack, 0.45f);
            SetFaceAnimation(AnimationType.Face_Damaged);
            laser.Attack(true);
            await UniTask.Delay(250);
            RemoveLaser(laser);
        }

        OnAttackFinished();
    }

    #endregion

    #region 벽 세우고 레이저 패턴

    #region 벽 풀링

    [SerializeField] private AlrauneWall wallPrefab;
    private static IObjectPool<AlrauneWall> _wallPool;

    private void InitializeWall()
    {
        _wallPool = new ObjectPool<AlrauneWall>(CreateWall, OnGetWall, OnReleaseWall, OnDestroyWall,
            defaultCapacity: 2, maxSize: 2);
    }

    private AlrauneWall CreateWall()
    {
        var wall = Instantiate(wallPrefab, _poolingObjectContainer);
        wall.SetManagedPool(_wallPool);
        return wall;
    }

    private static void OnGetWall(AlrauneWall wall)
    {
        wall.gameObject.SetActive(true);
    }

    private static void OnReleaseWall(AlrauneWall wall)
    {
        wall.gameObject.SetActive(false);
    }

    private static void OnDestroyWall(AlrauneWall wall)
    {
        Destroy(wall.gameObject);
    }

    #endregion

    [SerializeField] private EventInfo wallOutsideEvent;
    private async UniTaskVoid Attack_LaserWithWall()
    {
        MainAnimator.speed = 0;
        OnAttackStarted(AnimationType.Face_ClosedEyes, AttackPattern.LaserWithWall);

        // 벽 생성 타이밍에 벽 생성
        const float makingWallNormalizedTime = 0.3076f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, makingWallNormalizedTime);
        SetFaceAnimation(AnimationType.Face_Laugh);
        var toPlayerDirection = PlayerScript.Instance.transform.position - transformPosition;
        var walls = new List<AlrauneWall>
        {
            _wallPool.Get(),
            _wallPool.Get()
        };
        walls[0].Create(Quaternion.Euler(0, 0, -50) * toPlayerDirection).Forget();
        walls[1].Create(Quaternion.Euler(0, 0, 50) * toPlayerDirection).Forget();

        const float checkRangeNormalizedTime = 0.5461f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, checkRangeNormalizedTime);
        var currentPlayerDirection = PlayerScript.Instance.transform.position - transformPosition;
        var dot = Vector2.Dot(toPlayerDirection.normalized, currentPlayerDirection.normalized);
        var theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (theta > 50f) // 범위 바깥에 나가있다면
        {
            MainAnimator.SetFloat(AnimatorHash.AnimationSpeed, 0f);
            wallOutsideEvent.Begin();
            SetFaceAnimation(AnimationType.Face_Angry);
            await EventManager.EventFinishedEvent.OnInvokeAsync(CancellationToken.None);
            walls.Add(_wallPool.Get());
            walls.Add(_wallPool.Get());
            walls.Add(_wallPool.Get());
            walls[2].Create(Quaternion.Euler(0, 0, -100f) * toPlayerDirection).Forget();
            walls[3].Create(Quaternion.Euler(0, 0, 100f) * toPlayerDirection).Forget();
            walls[4].Create(-toPlayerDirection).Forget();

            MainAnimator.SetFloat(AnimatorHash.AnimationSpeed, 1f);
        }


        // 캐스팅 모션
        const float castingNormalizedTime = 0.5846f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, castingNormalizedTime);
        SetFaceAnimation(AnimationType.Face_ClosedEyes);
        ActivateLaserChargingEffect(true);

        // 대기모션까지 대기
        await MainAnimator.AsyncWaitForStart(AnimatorHash.Wait);
        SetFaceAnimation(AnimationType.Face_Scoff);
        var chasingWarningLaser = _laserPool.Get();

        // for (var i = 0; i < 100; i++)
        // {
        //     var danmaku = _danmakuPool.Get();
        //     danmaku.SetColor((AlrauneDanmaku.DanmakuColor)(i%4));
        //     danmaku.SetPosition(muzzleTransform.position);
        //     var direction = PlayerScript.Instance.GetFlyingPosition() - muzzleTransform.position;
        //     danmaku.SetVelocity(direction.normalized * 5f);
        //     SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Danmaku_Make);
        //     await UniTask.Delay(100);
        // }
        
        
        var direction = PlayerScript.Instance.GetFlyingPosition() - muzzleTransform.position;
        chasingWarningLaser.SetRotation(direction);
        chasingWarningLaser.Warn();
        chasingWarningLaser.ChasePlayer().Forget();
        await UniTask.Delay(1000);
        for (var i = 0; i < 10; i++)
        {
            await UniTask.Delay(500);
            CreateLaserForWithWall(chasingWarningLaser.transform.up).Forget();
        }
        
        RemoveLaser(chasingWarningLaser);

        await UniTask.Delay(1500);

        foreach (var wall in walls)
        {
            wall.Remove().Forget();
        }
        await UniTask.Delay(500);

        OnAttackFinished();
    }

    private async UniTaskVoid CreateLaserForWithWall(Vector3 upDirection)
    {
        MainAnimator.SetTrigger(AnimatorHash.AttackAgain);
        
        var laser = _laserPool.Get();
        laser.SetRotation(upDirection);
        laser.Attack(true);
        
        await UniTask.Delay(1000);
        
        RemoveLaser(laser);
    }

    #endregion

    #region X모양 레이저 회전

    public async UniTaskVoid Attack_LaserXRotate()
    {
        OnAttackStarted(AnimationType.Face_Laugh, AttackPattern.LaserXRotate);

        // 차지 이펙트
        const float chargingEffectNormalizedTime = 0.1206f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, chargingEffectNormalizedTime);
        ActivateLaserChargingEffect(true);

        // 경고 레이저
        const float warnLaserNormalizedTime = 0.8448f;
        await MainAnimator.AsyncWaitForNormalizedTime(AnimatorHash.Prepare, warnLaserNormalizedTime);
        var lasers = new AlrauneLaser[4];
        for (var i = 0; i < lasers.Length; i++)
        {
            lasers[i] = _laserPool.Get();
            lasers[i].SetRotation(90f * i);
            lasers[i].Warn();
        }

        // 공격 레이저로 전환
        await MainAnimator.AsyncWaitForStart(AnimatorHash.Attack);
        foreach (var laser in lasers)
        {
            laser.Attack();
        }

        // 1초 대기 후 회전 시작
        await UniTask.Delay(1000);
        var rotationDirection = Random.Range(0, 2) * 2 - 1;
        for (var i = 0; i < 5; i++)
        {
            var rotationAmount = new Vector3(0f, 0f, Random.Range(90f, 270f));
            await laserContainerTransform
                .DORotate(rotationAmount * rotationDirection, 40f)
                .SetRelative(true)
                .SetSpeedBased(true)
                .SetEase(Ease.InOutSine).Play();
            rotationDirection *= -1;
        }

        foreach (var laser in lasers)
        {
            RemoveLaser(laser);
        }

        await UniTask.Delay(500);

        OnAttackFinished();
    }

    #endregion

    #endregion

    #region 가시 패턴
    
    #region 0_가시 풀링

    [SerializeField] private AlrauneThorn thornPrefab;

    private static IObjectPool<AlrauneThorn> _thornPool;

    private void InitializeThorn()
    {
        _thornPool = new ObjectPool<AlrauneThorn>(CreateThorn, OnGetThorn, OnReleaseThorn, OnDestroyThorn
            , defaultCapacity: 100);
    }

    private AlrauneThorn CreateThorn()
    {
        var thorn = Instantiate(thornPrefab, _poolingObjectContainer);
        thorn.SetManagedPool(_thornPool);
        return thorn;
    }

    private static void OnGetThorn(AlrauneThorn thorn)
    {
        thorn.gameObject.SetActive(true);
    }

    private static void OnReleaseThorn(AlrauneThorn thorn)
    {
        thorn.gameObject.SetActive(false);
    }

    private static void OnDestroyThorn(AlrauneThorn thorn)
    {
        Destroy(thorn.gameObject);
    }

    #endregion

    #region 1_가시 라인으로 생성하여 공격하는 패턴
    
    /// <summary>
    /// lineNum 개수만큼 가시 라인 생성
    /// </summary>
    private async UniTaskVoid Attack_ThornInLine(int lineNum)
    {
        OnAttackStarted(AnimationType.Face_Scoff, AttackPattern.ThornInLine);

        var attackDir = PlayerScript.Instance.transform.position - transformPosition;
        switch (lineNum)
        {
            default:
                CreateThornInLine(attackDir).Forget();
                break;
            case 2:
                CreateThornInLine(Quaternion.Euler(0, 0, -15f) * attackDir).Forget();
                CreateThornInLine(Quaternion.Euler(0, 0, 15f) * attackDir).Forget();
                break;
            case 3:
                CreateThornInLine(Quaternion.Euler(0, 0, -30f) * attackDir).Forget();
                CreateThornInLine(attackDir).Forget();
                CreateThornInLine(Quaternion.Euler(0, 0, 30f) * attackDir).Forget();
                break;
        }

        await MainAnimator.AsyncWaitForComplete(AnimatorHash.Attack);
        
        OnAttackFinished();
    }

    /// <summary>
    /// direction 방향으로 벽에 도달할 때까지 0.5간격으로 가시 생성
    /// </summary>
    private async UniTaskVoid CreateThornInLine(Vector3 direction)
    {
        var spawnPosition = transformPosition;
        var gap = direction.normalized * 0.5f;

        var hit = Physics2D.Raycast(spawnPosition, direction,
            20f, LayerCache.GetLayerMask(LayerName.Wall));
        var numberOfThorn = Mathf.RoundToInt(hit.distance * 2);

        for (var i = 0; i < numberOfThorn; i++)
        {
            var thorn = _thornPool.Get();
            spawnPosition += gap;
            thorn.Create(spawnPosition).Forget();
            await UniTask.Delay(30);
        }
    }
    
    #endregion

    #region 1_가시 원형 패턴

    private async UniTaskVoid Attack_ThornInCircle()
    {
        OnAttackStarted(AnimationType.Face_ClosedEyes, AttackPattern.ThornDiffusion);

        await MainAnimator.AsyncWaitForComplete(AnimatorHash.Prepare);
        SetFaceAnimation(AnimationType.Face_Scoff);
        
        const int repeatCount = 16;
        const int numberOfCircle = 5;

        for (var currentRepeat = 0; currentRepeat < repeatCount; currentRepeat++)
        {
            var currentRadius = 0.5f * (currentRepeat % 4 + 1);
            for (var i = 0; i < numberOfCircle; i++)
            {
                SoundManager.Instance.PlaySoundEffect(SoundName.Alraune_Thorn_Make).Forget();
                CreateThornInCircle(currentRadius);
                currentRadius += 2f;
                await UniTask.Delay(100);
            }

            await UniTask.Delay(100);
        }

        OnAttackFinished();
    }
    
    /// <summary>
    /// 반지름이 radius인 원형 가시 생성
    /// </summary>
    private void CreateThornInCircle(float radius)
    {
        var circumference = 2 * radius * Mathf.PI; // 가시가 생성될 원의 둘레
        var numberOfThorn = (int)(circumference * 2); // 생성할 가시 개수, 0.5 간격이므로 *2
        var angle = 360f / numberOfThorn * Mathf.Deg2Rad; // 가시 간 각도
        for (var i = 0; i < numberOfThorn; i++)
        {
            var currentAngle = angle * i;
            var xPos = Mathf.Cos(currentAngle) * radius;
            var yPos = Mathf.Sin(currentAngle) * radius;
            if (xPos is > 9.1f or < -7.1f)
            {
                continue;
            }

            if (yPos is > 5 or < -5f)
            {
                continue;
            }

            var thorn = _thornPool.Get();
            thorn.Create(transform.position + new Vector3(xPos, yPos, 0)).Forget();
        }
    }

    #endregion

    #endregion
}
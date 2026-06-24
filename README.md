# FairyTales
> 써드파티 에셋 라이선스로 인해 전체 프로젝트가 아닌 C# 스크립트만 포함

> 스토리 어드벤처 | Unity, C# | 1인 개발 | PC · WebGL | 미출시 — 프롤로그 챕터(약 15분 · 컷씬 5개 + 보스전 1회) 완성
> 개발 기간 2022.03 – 2023.01


<img src="https://github.com/user-attachments/assets/82d044b3-36a3-4217-88ae-db410dee16b6" width="560" />

### 플레이 영상
🔗 [플레이 영상](https://youtu.be/g-mfo3WBaL0)

### 바로 플레이
- **WebGL 데모** (보스 등장 컷씬 + 보스전 구간): [itch.io](https://taebomico.itch.io/fairy-tales-beta)

---

## 기술 스택

- **Unity, C#**
- **Unity Timeline · Playables API** — 커스텀 `PlayableBehaviour`로 인터랙티브 연출 시스템 구현
- **UniTask** — 선택지 입력 대기 등 비동기 흐름 처리

---

## 핵심 — 인터랙티브 스토리 연출 시스템 (Timeline + 커스텀 Playable)

플레이어의 입력과 인게임 결과(피격 등)에 **실시간으로 반응해 분기되는** 동적 스토리 연출을, Unity Timeline을 확장해 구현했습니다.

| 파일 | 역할 |
|------|------|
| [DialogueBehaviour.cs](02_Event/Custom%20Playable/Dialogue/DialogueBehaviour.cs) | 대사 트랙 `PlayableBehaviour` — 클립 재생/종료 시 EventManager로 신호 |
| [EventManager_06_Timeline.cs](02_Event/01_Script/EventManager_06_Timeline.cs) | 타임라인 제어 + 선택지 분기 점프 (`SetSpeed(0)`) |
| [AnimationBehaviour.cs](02_Event/Custom%20Playable/CharacterAnimation/AnimationBehaviour.cs) | 캐릭터 애니메이션 커스텀 트랙 — Animator를 Timeline 밖으로 분리 |
| [EventManager.cs](02_Event/01_Script/EventManager.cs) | 연출 이벤트 허브 (partial 분리, 트랙 신호 수신부) |

**동기** — '가디언 테일즈'의 인터랙티브 연출에서 영감을 받아, 플레이어 개입에 반응해 분기되는 동적 스토리 연출 시스템을 목표로 잡았습니다.

**첫 시도와 한계 — 하드코딩 연출의 벽** — 처음엔 연출마다 별도 클래스로 직접 구현했으나 한계가 분명했습니다.
- **클래스 폭증** — 컷씬 1개당 클래스 1개가 늘어 로직 재사용 불가
- **느린 이터레이션** — 타이밍 1초만 고쳐도 30초+ 재컴파일, 원하는 지점으로 점프 테스트(Scrubbing) 어려움
- **시각화 부재** — 코드만으로는 전체 연출 흐름·현재 진행 위치를 직관적으로 파악 불가

**해결 — Playable API로 Timeline 확장** — Timeline을 도입하되 기본 기능만으론 인터랙티브 연출에 제약이 있어, `PlayableBehaviour`를 직접 상속·확장하는 아키텍처를 세웠습니다. 대사·선택지·카메라·캐릭터 애니메이션 등 **총 10종의 커스텀 트랙**을 구현했고, 각 클립이 재생되는 타이밍에 `EventManager`로 신호를 보내고 `PlayableGraph` API로 타임라인을 조작합니다.

```csharp
// DialogueBehaviour.cs — 클립 '재생/종료' 타이밍에 EventManager로 신호만 보냄 (연출 로직은 분리)
public override void OnBehaviourPlay(Playable playable, FrameData info)
{
    if (!Application.isPlaying) return;
    _isClipPlayed = true;
    EventManager.Instance.PrintDialogue(dialogueInfo);   // 대사 출력 요청
}

public override void OnBehaviourPause(Playable playable, FrameData info)
{
    if (!Application.isPlaying) return;
    if (!_isClipPlayed) return;
    _isClipPlayed = false;
    EventManager.Instance.FinishDialogue();              // 대사 종료 요청
}
```

**게임플레이 실시간 연동** — 보스의 레이저 공격 시 플레이어 피격 여부에 따라 다른 Timeline 구간을 재생하는 등, 인게임 로직과 연출이 유기적으로 분기되도록 했습니다. 선택지 클릭 또는 게임플레이 결과에 따라 `playableDirector.time`을 조작해 같은 Timeline 안에서 동적으로 점프합니다.

<img src="https://github.com/user-attachments/assets/4fbf568e-f7d6-4241-90a3-18c518f0c797" width="560" />
<img src="https://github.com/user-attachments/assets/b2ae7d0e-794d-4cb7-9bda-95d02b12e73f" width="560" />

```csharp
// EventManager_06_Timeline.cs — 선택지 클릭까지 대기 후, 선택한 분기 지점으로 점프
public async UniTask BranchTimelineByChoice(double[] destTimes)
{
    playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);            // 일시정지 (정지 대신 '시간만' 멈춤)
    var clickedButtonIndex = await ChoiceBtnClickEvent.OnInvokeAsync(_eventCts.Token);
    playableDirector.time = destTimes[clickedButtonIndex];                   // 선택에 맞는 구간으로 점프
    playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);            // 재개
}
```

관련 영상 — 🔗 [선택지 분기](https://www.youtube.com/watch?v=uK7iFyeS7iU) · 🔗 [보스전 피격 분기](https://www.youtube.com/watch?v=szLTYYV-Fzw)

**엔진 제약 극복 (Troubleshooting)** — Timeline 자체 동작은 바꿀 수 없다는 근본 제약을, 다음과 같이 우회·통제했습니다.

| 엔진 제약 | 우회 방식 |
|----------|----------|
| Timeline을 명시적 정지(Pause)하면 내부 **이벤트 호출 순서가 꼬임** | `SetSpeed(0)`으로 *"재생 상태는 유지하되 시간만 멈추는"* 일시정지 → 생명주기 순서 보존 |
| Timeline이 **Animator 트랙을 점유**해 일시정지 시 애니메이션도 강제로 멈춤 | Animator를 Timeline 관리 밖으로 빼고, 커스텀 트랙(`AnimationBehaviour`)이 `StageObject`에 직접 애니메이션 명령 |

**결론** — Unity Timeline을 단순 컷씬 편집 도구를 넘어 **플레이어 개입에 반응하는 인터랙티브 스토리 시스템의 코어**로 활용했습니다. 한 번 갖춘 커스텀 트랙은 GUI에서 직관적으로 편집되어 초기 구축 비용이 이후 작업 효율로 상쇄됐고, *작업자 휴먼 에러를 줄이는 파이프라인(Workflow) 최적화*로 시야를 넓히는 계기가 됐습니다.

---

## 핵심 코드 구조
<pre>
05_FairyTales/
├── 02_Event/                          # ★ 연출 시스템 (핵심)
│   ├── 01_Script/                     # EventManager 패밀리 (partial 분리, 트랙 신호 수신)
│   │   ├── <a href="02_Event/01_Script/EventManager.cs">EventManager.cs</a>             # 이벤트 허브 진입점
│   │   ├── <a href="02_Event/01_Script/EventManager_03_Dialogue.cs">EventManager_03_Dialogue.cs</a> # 대사 출력 + 일시정지
│   │   └── <a href="02_Event/01_Script/EventManager_06_Timeline.cs">EventManager_06_Timeline.cs</a> # 분기 + 점프 (SetSpeed(0)) ★
│   └── <a href="02_Event/Custom%20Playable">Custom Playable/</a>               # 10종 커스텀 트랙 (각 Behaviour/Clip/Track) ★
│       ├── <a href="02_Event/Custom%20Playable/Dialogue/DialogueBehaviour.cs">Dialogue/</a>                  # 대사
│       ├── Choice/                    # 선택지
│       ├── <a href="02_Event/Custom%20Playable/CharacterAnimation/AnimationBehaviour.cs">CharacterAnimation/</a>        # 캐릭터 애니메이션 (Animator 분리)
│       ├── CameraControl/ · CameraEffect/ · OverlayCameraControl/   # 카메라 3종
│       ├── Emotion/ · Sound/          # 감정 버블 · 사운드
│       └── TimeControl/ · TransformControl/                          # 시간 · 위치 제어
├── 01_Object/
│   ├── 01_Player/                     # 플레이어 (상태머신, partial 분리)
│   └── <a href="01_Object/03_Boss/Alraune/Phase1">03_Boss/Alraune/</a>           # 보스 (연출 실시간 연동 사례)
├── 03_Stage/Room/                     # 룸 시스템
└── 04_System/Manager/                 # Camera · Stage · Sound 등 매니저
</pre>

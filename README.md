# FairyTales
> 스토리 어드벤쳐 | Unity | 1인 개발 | PC (Windows, WebGL) | 프롤로그 데모

### 플레이 영상
[![플레이 영상](https://img.youtube.com/vi/g-mfo3WBaL0/0.jpg)](https://youtu.be/g-mfo3WBaL0)

### 플레이 링크
- **WebGL 데모** (보스 등장 컷씬 + 보스전 구간): [itch.io](https://taebomico.itch.io/fairy-tales-beta)

---

## 기술 스택

- **Unity, C#**
- **Unity Timeline / Playables API** — 인터랙티브 연출 시스템의 기반
- **UniTask** — 비동기 처리 (선택지 입력 대기 등)

---

## 핵심 구현

| 파일 | 역할 |
|------|------|
| [DialogueBehaviour.cs](02_Event/Custom%20Playable/Dialogue/DialogueBehaviour.cs) | 대사 트랙 `PlayableBehaviour`, 클립 재생/종료 시 EventManager로 신호 |
| [ChoiceBehaviour.cs](02_Event/Custom%20Playable/Choice/ChoiceBehaviour.cs) | 선택지 트랙 `PlayableBehaviour` |
| [AnimationBehaviour.cs](02_Event/Custom%20Playable/CharacterAnimation/AnimationBehaviour.cs) | 캐릭터 애니메이션 트랙 (Animator를 Timeline 밖으로 분리) |
| [EventManager.cs](02_Event/01_Script/EventManager.cs) | 이벤트 허브 진입점 (partial 분리) |
| [EventManager_03_Dialogue.cs](02_Event/01_Script/EventManager_03_Dialogue.cs) | 대사 출력 및 Timeline 일시정지 (`SetSpeed(0)`) |
| [EventManager_06_Timeline.cs](02_Event/01_Script/EventManager_06_Timeline.cs) | 타임라인 제어 + 선택지 분기 (`BranchTimelineByChoice`) |
| [AlrauneTutorial.cs](01_Object/03_Boss/Alraune/Phase1/AlrauneTutorial.cs) | 보스 상태머신 (인터랙티브 분기 사례) |

### 커스텀 PlayableBehaviour 기반 인터랙티브 연출 시스템

대사, 선택지, 카메라 제어, 캐릭터 애니메이션, 사운드 등 **10종의 커스텀 트랙**을 `PlayableBehaviour` 상속으로 구현. 각 클립이 재생되는 타이밍에 `EventManager`로 신호를 보내고, `PlayableGraph` API로 타임라인을 조작합니다.

```csharp
// 선택지 대기 후 분기점 점프 (EventManager_06_Timeline.cs)
public async UniTask BranchTimelineByChoice(double[] destTimes)
{
    playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0); // 타임라인 일시정지
    var clickedButtonIndex = await ChoiceBtnClickEvent.OnInvokeAsync(_eventCts.Token);
    playableDirector.time = destTimes[clickedButtonIndex]; // 선택에 따라 구간 점프
    playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1); // 재개
}
```

### Timeline API 우회

Timeline 자체의 동작은 변경할 수 없다는 근본적 제약을 우회:
- **일시정지/재개 시 이벤트 순서 문제** → 명시적 정지 대신 `SetSpeed(0)`으로 *"재생 상태를 유지하되 시간만 멈추는"* 방식 사용
- **Timeline의 Animator 독점** → Animator를 Timeline 관리 밖으로 빼고, 커스텀 트랙으로 직접 명령

### 인터랙티브 분기

선택지 클릭 또는 게임플레이 결과(보스전 피격 여부 등)에 따라 `playableDirector.time`을 조작해 다른 구간으로 점프. 동일 Timeline 내에서 동적 분기를 구현했습니다.

- [선택지 분기 영상](https://www.youtube.com/watch?v=uK7iFyeS7iU)
- [보스전 분기 영상(마지막에 플레이어가 레이저를 맞을 시, 특수 대사 출력](https://www.youtube.com/watch?v=szLTYYV-Fzw)

---

## 핵심 코드 구조

```
05_FairyTales/
├── 01_Object/
│   ├── 00_Common/                 # 공통 (HealthSystem, Interactable 등)
│   ├── 01_Player/                 # 플레이어 (상태머신, partial 분리)
│   │   └── State/                 # PlayerState_Move, Dash, Damaged 등
│   ├── 02_NPC/Ant/
│   ├── 03_Boss/Alraune/Phase1/    # 보스 (인터랙티브 연동 사례)
│   └── 04_Thing/
├── 02_Event/                      # ★ 연출 시스템 (핵심)
│   ├── 01_Script/                 # EventManager 패밀리 (partial)
│   │   ├── EventManager.cs
│   │   ├── EventManager_01_Event.cs
│   │   ├── EventManager_02_Letterbox.cs
│   │   ├── EventManager_03_Dialogue.cs   # 대사 출력 + SetSpeed(0)
│   │   ├── EventManager_04_Choice.cs
│   │   ├── EventManager_05_Input.cs
│   │   └── EventManager_06_Timeline.cs   # 분기 + 점프 ★
│   ├── Custom Playable/           # 10종 커스텀 트랙 ★
│   │   ├── Dialogue/              # 대사
│   │   ├── Choice/                # 선택지
│   │   ├── CameraControl/         # 카메라 위치/회전
│   │   ├── CameraEffect/          # 카메라 이펙트
│   │   ├── CharacterAnimation/    # 캐릭터 애니메이션 (Animator 분리)
│   │   ├── Emotion/               # 감정 버블
│   │   ├── OverlayCameraControl/  # 오버레이 카메라
│   │   ├── Sound/                 # 사운드
│   │   ├── TimeControl/           # 시간 제어
│   │   └── TransformControl/      # 위치 제어
│   └── 03_UI/Choice/              # 선택지 UI
├── 03_Stage/Room/                 # 룸 시스템
└── 04_System/Manager/             # 매니저들 (Camera, Stage, Sound 등)
```

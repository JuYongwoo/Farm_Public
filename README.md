# Farm

## 게임 방식

- **플레이어 이동/시점**
  - `PlayerMove.cs`: `CharacterController` 기반 이동 + 발소리(`AudioSource`) 재생
  - `PlayerRotate.cs`: `Camera.main`의 pitch(상하) + 플레이어 yaw(좌우) 회전 처리

- **상호작용(조준 + 키 입력)**
  - `IInteractable.cs`: `promptText` + `interactionKey(기본 E)`를 가진 `AimingIntractionPrompt` 정의, `Interact()` 계약 제공
  - `PlayerItemsInteractor.cs`: 화면 중앙 기준 Raycast(거리 2f) → `GetComponent<IInteractable>()`로 대상 획득 → 안내 문구 표시 → `interactionKey` 입력 시 `Interact()` 호출

- **인벤토리/UI**
  - `PlayerInventory.cs`, `InventoryCanvas.cs`, `InventoryUIScroll.cs` 포함

- **이벤트/연출(EventPlay 연동)**
  - 다수 스크립트가 `JYW.Game.EventPlay` 네임스페이스의 `EventPlayManager`, `EventSO`를 참조합니다.
  - `Chapter1.cs`: 시작 시 `PlayEvent(startEventSO, ...)` 호출 + 커서 Lock 처리
  - `WinScene.cs`, `GameOverScene.cs`: 시작 시 `PlayEvent(eventSO, ...)` 호출
  - `Door.cs`, `Drawer.cs`, `Battery.cs`, `MiniTV.cs`, `TV.cs`, `Shoes.cs`: `EventPlayManager.Instance.AddAction(gameObject, ...)` 형태로 액션 등록

- **적(Enemy)**
  - `IEnemy.cs`: `GetHit()` 계약 제공
  - `Enemy` 폴더에 적 스크립트 10개가 있고, 그 중 9개가 `NavMeshAgent`를 사용합니다.
  - `Ichigo.cs` 등에서 `playerTag = "Player"`를 사용하며, `EventPlayManager.Instance.isLockMove`로 상태를 체크

---

## 아키텍처 요약

- **asmdef 모듈 분리**
  - `JYW.Game.Commons`, `JYW.Game.Enemies`, `JYW.Game.Items`, `JYW.Game.Particles`, `JYW.Game.Players`, `JYW.Game.Scenes`, `JYW.Game.UIs`, `JYW.Game.Utils`

- **공통 계약(Common)**
  - `IEnemy.cs` / `IInteractable.cs`로 상호작용·피격 계약 분리

- **EventPlay 기반 액션/이벤트 트리거**
  - Scene 진입/승리/게임오버, 아이템 액션 등록이 `EventPlayManager` 호출로 연결

- **에디터 툴 분리(Editor)**
  - `GridSpawnerWindow`, `PrefabScatterWindow`, `NamingTool`, `ChangeAllMaterialsShader`, `EventSOMigrationTool`

---

## 폴더 구조 (Scripts)

Scripts
├── Common
│ ├── IEnemy.cs
│ ├── IInteractable.cs
│ └── JYW.Game.Commons.asmdef
├── Editor
│ ├── ChangeAllMaterialsShader.cs
│ ├── EventSOMigrationTool.cs
│ ├── GridSpawnerWindow.cs
│ ├── NamingTool.cs
│ └── PrefabScatterWindow.cs
├── Enemy
│ ├── ExToilet.cs
│ ├── Ichigo.cs
│ ├── JYW.Game.Enemies.asmdef
│ ├── MasSunba.cs
│ ├── MegaSunba.cs
│ ├── MuscleSunba.cs
│ ├── PassportSunba.cs
│ ├── SahurSunba.cs
│ ├── SpeedSunba.cs
│ ├── Zombie.cs
│ └── ZombieSunba.cs
├── Items
│ ├── Battery.cs
│ ├── Door.cs
│ ├── Drawer.cs
│ ├── JYW.Game.Items.asmdef
│ ├── MiniTV.cs
│ ├── Shoes.cs
│ └── TV.cs
├── Managers
│ └── GameManager.cs
├── Particles
│ ├── JYW.Game.Particles.asmdef
│ └── WoodParticle.cs
├── Player
│ ├── CameraFilter.cs
│ ├── JYW.Game.Players.asmdef
│ ├── PlayerEnemyDetector.cs
│ ├── PlayerFlashlight.cs
│ ├── PlayerHand.cs
│ ├── PlayerInventory.cs
│ ├── PlayerItemsInteractor.cs
│ ├── PlayerMove.cs
│ └── PlayerRotate.cs
├── Scenes
│ ├── Chapter1.cs
│ ├── GameOverScene.cs
│ ├── JYW.Game.Scenes.asmdef
│ ├── TitleScene.cs
│ └── WinScene.cs
├── UI
│ ├── ESCCanvas.cs
│ ├── GameOverVideoCanavas.cs
│ ├── InventoryCanvas.cs
│ ├── InventoryUIScroll.cs
│ ├── JYW.Game.UIs.asmdef
│ ├── MemoCanvas.cs
│ ├── MissionCanvas.cs
│ ├── PasswordCanvas.cs
│ ├── SettingsPanel.cs
│ ├── SpeechCanvas.cs
│ ├── TitlePanel.cs
│ ├── UICameraBind.cs
│ ├── UICancel.cs
│ └── WinCanvas.cs
└── Utils
├── DeepCloneHelper.cs
├── JYW.Game.Utils.asmdef
└── Util.cs
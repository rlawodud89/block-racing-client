# Snapshot Rendering

Client는 Server가 전달한 `GameStateSnapshot`을 기준으로 현재 게임 화면을 갱신한다.

Snapshot 자체의 생성과 Server 전송 구조는 Server의 [snapshot.md](https://github.com/rlawodud89/block-racing-server/blob/master/docs/snapshot.md)에서 다루며, 이 문서에서는 Client가 수신한 Snapshot을 어떻게 검증하고 게임 화면에 적용하는지를 다룬다.

---

## Rendering Flow

Client의 Snapshot 처리 흐름은 다음과 같다.

```text
S_GameStatePacket
      ↓
S_GameStateHandler
      ↓
GameStateController.ApplySnapshot()
      ↓
Tick Validation
      ↓
GameStartSequenceController
      ↓
GameStateController.ApplyGameState()
      ↓
┌───────────────┬───────────────┐
↓               ↓               ↓
LaneView       CarView       PlayerUI
↓               ↓               ↓
Blocks       Car Position   Current Piece
FlyingBlocks  Stun State     Mode
Lane Scroll                  Shoot Cooldown
```

Snapshot을 받은 뒤 [`GameStateController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs)가 전체 적용 과정을 조정하고, 각 View Component가 실제 Unity Object를 갱신한다.

---

## Snapshot Reception

게임 상태 Packet은 [`S_GameStateHandler`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/Handlers/S_GameStateHandler.cs)에서 처리한다.

```csharp
public static void Handle(S_GameStatePacket packet)
{
    GameStateController.Instance.ApplySnapshot(
        packet.Snapshot
    );
}
```

Handler는 Snapshot을 직접 렌더링하지 않고 `GameStateController`로 전달한다.

이를 통해 Network Layer와 실제 게임 상태 반영을 분리한다.

---

## Tick Validation

Client는 Snapshot의 Tick을 기준으로 오래된 Snapshot을 무시한다.

```csharp
if (snapshot.Tick <= _lastTick)
    return;

_lastTick = snapshot.Tick;
```

처리 흐름은 다음과 같다.

```text
Received Snapshot
      ↓
snapshot.Tick <= _lastTick ?
      │
 ┌────┴────┐
Yes        No
 │          │
Ignore   Continue
```

이미 처리한 Tick과 같거나 더 이전의 Snapshot은 적용하지 않는다.

---

## Game Start Synchronization

Snapshot의 Tick은 게임 시작 시퀀스에도 사용된다.

```text
S_StartGamePacket
      ↓
_startTick
      ↓
GameStartSequenceController
      ↓
Snapshot.Tick
      ↓
ApplyTick()
      ↓
StartTick 도달
      ↓
Game Started
```

[`GameStateController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs)의 `ApplySnapshot()`은 게임 상태를 적용하기 전에 `GameStartSequenceController.ApplyTick(snapshot.Tick)`을 호출한다.

게임 시작 전에는 Snapshot의 게임 상태를 View에 적용하지 않는다.

```csharp
if (!gameStartSequenceController.IsGameStarted)
    return;
```

이를 통해 Countdown과 실제 게임 상태 렌더링을 Server Tick 기준으로 동기화한다.

---

## Player Separation

Snapshot에는 두 Player의 상태가 포함되며, Client는 자신의 `PlayerId`를 기준으로 자신의 Snapshot과 상대방 Snapshot을 구분한다.

```text
GameStateSnapshot
      │
      └── Players[]
             │
             ├── Id == ClientContext.PlayerId
             │       ↓
             │    MySnapshot
             │
             └── Other Player
                     ↓
                 OpponentSnapshot
```

이후 두 Player의 상태를 각각 자신의 View와 상대방 View에 전달한다.

현재 Client는 자신의 Lane을 항상 왼쪽, 상대방 Lane을 항상 오른쪽에 배치한다.

---

## Lane Rendering

[`LaneView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Lane/LaneView.cs)는 LaneSnapshot을 받아 정착된 Block과 이동 중인 FlyingBlock을 갱신한다.

```text
LaneSnapshot
├── Blocks[]
│     ↓
│  BlockView[]
│
└── FlyingBlocks[]
      ↓
   FlyingBlockView[]
```

### Settled Blocks

Lane의 Block 데이터는 1차원 배열로 전달되며, Client는 미리 생성해 둔 [`BlockView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Block/BlockView.cs) 배열을 순회하면서 상태를 적용한다.

```csharp
for (int i = 0; i < snapshot.Blocks.Length; i++)
{
    _blocks[i].SetBlock(snapshot.Blocks[i]);
}
```

`BlockView.SetBlock()`은 Block 데이터가 비어 있으면 Image를 비활성화하고, Block이 존재하면 Image를 활성화한다.

즉, Snapshot의 Block 상태를 기존 Unity Object에 적용하는 방식으로 렌더링한다.

### Flying Blocks

FlyingBlock은 Snapshot 개수에 맞춰 [`FlyingBlockView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Block/FlyingBlockView.cs)를 생성하고, 현재 필요한 View만 활성화한다.

```text
FlyingBlocks Snapshot
        ↓
필요한 View가 부족한 경우 Instantiate
        ↓
FlyingBlockView.UpdateBlock()
        ↓
남는 View는 SetActive(false)
```

기존 View를 재사용하여 매 Snapshot마다 모든 FlyingBlock을 새로 생성하지 않는다.

FlyingBlockView는 Snapshot의 X, Y, Type, Rotation을 사용하여 위치와 Block Shape를 갱신한다.

---

## Lane Scroll Rendering

[`LaneView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Lane/LaneView.cs)의 `UpdateLane()`은 Snapshot의 Lane 상태와 함께 Car의 Speed를 전달받아 [`LaneScroller`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Lane/LaneScroller.cs)의 Scroll Speed를 갱신한다.

```text
PlayerSnapshot
├── Speed
└── Lane
      │
      └── LaneView.UpdateLane()
              ├── LaneScroller.SetScrollSpeed(Speed)
              ├── BlockView Update
              └── FlyingBlockView Update
```

따라서 Client의 화면상 Lane Scroll 속도는 Snapshot에 포함된 Player Speed를 기준으로 갱신된다.

---

## Car Rendering

Car 상태는 별도의 CarSnapshot 객체를 사용하는 것이 아니라 PlayerSnapshot의 값을 [`CarView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Car/CarView.cs)에 전달한다.

```text
PlayerSnapshot
├── CarX
└── IsStunned
      ↓
CarView.UpdateCar()
      ├── Position
      └── Stun State
```

CarX는 Unity UI 좌표로 변환되어 Car의 위치에 적용된다.

IsStunned가 true이면 Coroutine을 통해 Car의 투명도를 반복적으로 변경하여 Stun 상태를 표현한다.

---

## Player UI Rendering

자신의 PlayerSnapshot은 게임 화면의 [`PlayerUI`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Player/PlayerUI.cs)에도 사용된다.

```text
PlayerSnapshot
├── CurrentPieceType
├── CurrentPieceRotation
├── Mode
└── ShootCooldownRemaining
          ↓
      PlayerUI
```

### Current Piece

CurrentPieceType과 CurrentPieceRotation을 사용하여 현재 사용할 Piece의 Shape를 표시한다.

현재 Piece가 없는 경우 모든 UI Cell을 비활성화한다.

### Mode

PlayMode에 따라 현재 모드를 UI에 표시한다.

```text
Defense → 수비
Attack  → 공격
```

### Shoot Cooldown

ShootCooldownRemaining을 사용하여 공격 쿨타임 UI를 갱신한다.

동시에 같은 값을 [`InputController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Player/InputController.cs)에도 전달한다.

```text
PlayerSnapshot.ShootCooldownRemaining
          │
     ┌────┴─────┐
     ↓          ↓
 PlayerUI   InputController
     │          │
  Cooldown   Shoot 가능 여부
```

Client는 Server가 전달한 쿨타임 상태를 기준으로 UI와 입력 가능 여부를 갱신한다.

---

## State Application

전체 게임 상태 적용은 [`GameStateController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs)의 `ApplyGameState()`에서 수행된다.

```text
GameStateSnapshot
      ↓
PlayerSnapshot 분리
      ↓
┌─────┴────────────────────────────┐
│                                  │
My Player                       Opponent
│                                  │
├── Lane → LaneView             ├── Lane → LaneView
├── Car  → CarView              └── Car  → CarView
├── UI   → PlayerUI
└── Cooldown → InputController
```

Client는 Snapshot 데이터를 별도의 게임 로직으로 재계산하지 않고, 필요한 View Component에 전달하여 화면 상태를 갱신한다.

---

## Server Authoritative Rendering

Client는 Snapshot을 기반으로 화면을 렌더링하며, 게임 결과를 자체적으로 결정하지 않는다.

```text
             Server
                │
        Game Simulation
                │
                ▼
             Snapshot
                │
                ▼
             Client
                │
        GameStateController
                │
        ┌───────┴────────┐
        ↓                ↓
   Game State          View
        │                │
        └───────┬────────┘
                ▼
             Rendering
```

Client에서 처리하는 것은 Server 상태의 적용과 시각적 표현이다.

게임 규칙, 충돌, 거리 계산 및 게임 종료 판정은 Server에서 수행하며, Client는 전달받은 결과를 렌더링한다.

---

## Design Summary

Snapshot Rendering은 다음과 같은 책임 분리를 가진다.

| 단계 | 책임 |
| --- | --- |
| [`S_GameStateHandler`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/Handlers/S_GameStateHandler.cs) | Snapshot 전달 |
| [`GameStateController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs) | Tick 검증 및 전체 상태 적용 조정 |
| [`LaneView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Lane/LaneView.cs) | Lane / Block / FlyingBlock 렌더링 |
| [`CarView`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Car/CarView.cs) | Car 위치 및 Stun 표현 |
| [`PlayerUI`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Player/PlayerUI.cs) | 현재 Piece / Mode / Cooldown UI |
| [`InputController`](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Player/InputController.cs) | Server 상태 기반 입력 가능 여부 반영 |

전체 흐름은 다음과 같다.

```text
Server Snapshot
      ↓
Packet Handler
      ↓
Tick Validation
      ↓
GameStateController
      ↓
Player / Lane State 분리
      ↓
View Components
      ↓
Unity Rendering
```

Client는 Snapshot을 기준으로 화면 상태를 일관되게 갱신하고, 게임의 실제 상태와 판정은 Server가 담당하는 구조를 유지한다.



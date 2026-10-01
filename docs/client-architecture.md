# Client Architecture

Unity Client는 **Network / Data / Events / Game System / View** 영역을 분리하여 구성하였다.

Client는 게임 결과를 직접 판정하지 않고, Server의 입력 처리 결과와 Snapshot을 받아 **게임 상태를 화면에 반영하는 역할**을 담당한다.

---

## Architecture Overview

```text
                         Server
                           │
                           │ TCP
                           ▼
                   NetworkManager
                           │
                           ▼
                    ClientSession
                           │
                    ┌──────┴──────┐
                    │             │
                 Send          Receive
                    │             │
                    ▼             ▼
               PacketWriter   ReceiveBuffer
                                  │
                                  ▼
                            PacketManager
                                  │
                                  ▼
                              Handler
                                  │
                    ┌─────────────┴─────────────┐
                    ▼                           ▼
                  Events                  GameStateController
                    │                           │
             ┌──────┴──────┐             ┌─────┴─────┐
             ▼             ▼             ▼           ▼
            UI        Game Systems      Lane        Car / Player
```

Network Layer는 Server와의 연결 및 Packet 처리를 담당하고, Handler는 수신한 Packet을 Event 또는 Game State 처리로 전달한다.

---

## Layer Responsibilities

| 영역 | 주요 클래스 | 역할 |
| --- | --- | --- |
| Network | [NetworkManager](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/NetworkManager.cs), [ClientSession](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/ClientSession.cs) | TCP 연결, 송수신, Heartbeat, Disconnect |
| Packet | [PacketManager](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/PacketManager.cs), Handlers | Packet 역직렬화 및 Packet별 처리 |
| Data | `ClientContext`, `MatchContext`, `ResultData` | Client 상태 및 화면 전환에 필요한 데이터 보관 |
| Events | `NetworkEvents`, `RoomEvents`, `GameEvents`, `LoginEvents` | Network와 UI / Game System 간 상태 전달 |
| Game System | [GameStateController](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs), [GameStartSequenceController](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GameStart/GameStartSequenceController.cs), [InputController](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Player/InputController.cs) | 게임 상태 적용, 시작 시퀀스, 사용자 입력 처리 |
| View | [LaneView](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Lane/LaneView.cs), [CarView](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Car/CarView.cs), [BlockView](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Block/BlockView.cs), [FlyingBlockView](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/Block/FlyingBlockView.cs), `PlayerUI` | 전달받은 상태를 Unity Object와 UI로 표현 |
| Manager | [SceneLoader](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Managers/SceneLoader.cs), `AudioManager`, `BootstrapLoader` | Scene 및 Client 공통 기능 관리 |

---

## Network Layer

Network Layer는 Server와의 TCP 연결 생명주기와 Packet 송수신을 담당한다.

### NetworkManager

[NetworkManager](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/NetworkManager.cs)는 Client의 네트워크 진입점으로 동작한다.

- `ClientSession` 생성 및 연결 관리
- Packet 전송
- Heartbeat 처리
- Disconnect 처리

```text
NetworkManager
    │
    ├── ConnectLoopAsync()
    │       └── ClientSession.ConnectAsync()
    │
    ├── SendAsync()
    │       └── ClientSession.SendAsync()
    │
    └── HandleDisconnected()
            └── Scene / UI 처리
```

`NetworkManager`는 Scene 전환 이후에도 연결을 유지할 수 있도록 전역 객체로 관리된다.

### ClientSession

[ClientSession](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/ClientSession.cs)은 실제 TCP 연결과 송수신을 담당한다.

```text
ClientSession
├── TcpClient
├── NetworkStream
├── ReceiveBuffer
├── ReceiveLoopAsync()
├── SendAsync()
└── HeartbeatTimeoutLoopAsync()
```

수신 데이터는 `ReceiveBuffer`에서 Packet 단위로 분리한 뒤 `PacketManager`로 전달한다.

---

## Packet Processing

Client는 Packet ID를 기준으로 등록된 Handler를 선택한다.

```text
TCP Receive
    ↓
ReceiveBuffer
    ↓
PacketReader
    ↓
PacketId
    ↓
PacketManager
    ↓
Handler
    ├── Event
    └── GameStateController
```

[PacketManager](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Network/PacketManager.cs)는 Packet ID별 Handler를 등록하고, 등록된 Handler에 `PacketReader`를 전달한다. 각 Handler는 Reader를 사용해 자신의 Packet을 역직렬화한 뒤 처리한다.

게임 상태 Packet은 다음과 같이 처리된다.

```text
S_GameStatePacket
      ↓
S_GameStateHandler
      ↓
GameStateController.ApplySnapshot()
      ↓
Game State / View 갱신
```

---

## Event-driven Architecture

Network Handler와 UI / Game System 사이의 직접적인 의존성을 줄이기 위해 Event를 사용한다.

```text
Packet Handler
      │
      ▼
   Event Raise
      │
 ┌────┴─────────┐
 ▼              ▼
UI          Game System
```

주요 Event 영역:

| Event | 역할 |
| --- | --- |
| `NetworkEvents` | 연결 / 해제 |
| `LoginEvents` | 로그인 결과 |
| `RoomEvents` | Room 생성, 입장, Ready, 취소 |
| `GameEvents` | 게임 시작, 종료, 상대방 종료 |

이를 통해 Packet Handler가 특정 UI Controller를 직접 참조하지 않고 상태 변화를 전달할 수 있다.

---

## Game State Rendering

게임 진행 중 Client는 Server가 전달한 `GameStateSnapshot`을 기준으로 화면을 갱신한다.

```text
S_GameStatePacket
      ↓
S_GameStateHandler
      ↓
GameStateController
      ↓
GameStateSnapshot
      │
      ├── PlayerSnapshot
      └── LaneSnapshot
            ├── Blocks
            └── FlyingBlocks
      ↓
View Components
```

[GameStateController](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GamePlay/GameStateController.cs)는 Snapshot의 Tick을 검증하고, 자신의 `PlayerId`를 기준으로 두 Player의 상태를 분리하여 각 View에 전달한다.

```text
GameStateController
    ├── My Lane       → LaneView
    ├── Opponent Lane → LaneView
    ├── My Car        → CarView
    ├── Opponent Car  → CarView
    ├── Finish Line   → FinishLineView
    └── Player UI     → PlayerUI
```

Snapshot의 생성과 Server 전송 구조는 Server의 [snapshot.md](https://github.com/rlawodud89/block-racing-server/blob/docs/add-docs/docs/snapshot.md)에서 다루며, Client의 실제 Snapshot 적용 과정은 [snapshot-rendering.md](snapshot-rendering.md)에서 별도로 다룬다.

---

## Input Processing

Client의 입력은 게임 상태를 직접 변경하지 않고 Server에 Packet으로 전달한다.

```text
Unity Input
    ↓
InputController
    ↓
C_InputPacket
    ↓
NetworkManager
    ↓
ClientSession
    ↓
Server
```

현재 입력은 이동, 회전, 모드 변경, 공격 등의 `InputType`으로 전달된다.

게임 시작 전에는 입력을 비활성화하고, [GameStartSequenceController](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Systems/GameStart/GameStartSequenceController.cs)가 Server Tick을 기준으로 시작 시퀀스를 진행한 뒤 입력을 활성화한다.

---

## Game Start Flow

게임 시작은 Server가 전달한 `S_StartGamePacket`의 `StartTick`을 기준으로 처리한다.

```text
S_StartGamePacket
      ↓
S_StartGameHandler
      ↓
GameEvents
      ↓
GameStartSequenceController
      ↓
StartTick 기준 Countdown
      ↓
OnGameStarted
      ↓
InputController 활성화
```

Client는 로컬 시간 대신 Server Tick을 기준으로 Countdown과 실제 게임 시작 시점을 처리한다.

---

## Room / Scene Flow

Matchmaking과 Private Room 관련 UI는 Packet 결과를 Event로 전달받아 Scene 및 UI 상태를 변경한다.

```text
Lobby
  │
  ├── Automatic Matchmaking
  │       ↓
  │    Room Ready
  │
  └── Private Room
          ├── Create
          └── Join
                ↓
             Game
                ↓
             Result
                ↓
             Lobby
```

Scene 전환은 [SceneLoader](https://github.com/rlawodud89/block-racing-client/blob/main/Assets/Scripts/Managers/SceneLoader.cs)가 담당한다.

주요 Scene:

- Bootstrap
- Title
- Lobby
- Game
- Result
- MatchCanceled

---

## Server Authoritative Client

Client는 게임 결과를 직접 판정하지 않는다.

```text
Client Input
    │
    ▼
  Server
    │
    ├── Game Simulation
    ├── Collision
    ├── Game Rule
    └── Game End
    │
    ▼
 Snapshot / Result
    │
    ▼
  Client
    │
    ▼
 Rendering
```

Client의 책임은 다음과 같이 분리된다.

- 사용자 입력 수집 및 Server 전달
- Server Packet 수신 및 처리
- Snapshot 기반 게임 상태 반영
- UI / Scene 상태 관리
- 게임 상태의 시각적 표현

게임 규칙과 최종 결과는 Server가 결정하며, Client는 전달받은 상태를 화면에 표현한다.

---

## Project Structure

```text
Assets/Scripts/
├── Common/       # Client / Server Shared Module
├── Core/         # Client 공통 기능
├── Data/         # Client 상태 데이터
├── Events/       # Event 정의
├── Managers/     # Scene / Audio 등 관리
├── Network/      # TCP / Packet 처리
└── Systems/
    ├── Login/
    ├── RoomMaking/
    ├── GameStart/
    ├── GamePlay/
    │   ├── Player/
    │   ├── Car/
    │   ├── Lane/
    │   └── Block/
    └── GameEnd/
```

---

## Design Summary

Client는 **Network와 Game/UI를 분리하고, Server Authoritative 구조에 맞춰 Server 상태를 화면에 반영하는 것**을 중심으로 구성하였다.

```text
Input
  ↓
Network
  ↓
Server
  ↓
Snapshot / Result
  ↓
Handler
  ↓
Event / GameStateController
  ↓
Game / UI
  ↓
Rendering
```

Network Layer는 통신을 담당하고, Game System은 전달받은 상태를 적용하며, View는 최종 상태를 Unity 화면으로 표현한다.

# 🎮 Block Racing Client

> Unity 기반 2인 온라인 블록 레이싱 Client

블록 레이싱의 **게임 화면과 사용자 입력, 서버와의 통신 및 상태 반영**을 담당하는 Unity Client입니다.

Client는 직접 게임의 결과를 판정하지 않고, Server의 게임 상태와 입력 처리 결과를 받아 **게임 화면에 반영하는 역할**을 중심으로 설계했습니다.

---

## 📌 Overview

* Unity 기반 게임 Client
* TCP 기반 Server 통신
* Packet 송수신 및 Handler 처리
* 사용자 입력 전달
* Server Snapshot 기반 게임 상태 반영
* Matchmaking / Room / Game Lifecycle UI
* Connection 및 Disconnect 처리
* Heartbeat 및 Reconnect 처리
* 게임 상태와 UI 간 Event 기반 연결
* 공통 Packet / Snapshot을 Git Submodule로 공유

---

## 🏗️ Architecture

Client는 **Network / Data / Game / UI** 영역을 분리하여 구성했습니다.

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
                       ▼
                 PacketManager
                       │
                ┌──────┴──────┐
                ▼             ▼
             Handler        Events
                │             │
                ▼             ▼
             Client Data    UI / Game
                │
                ▼
          GameStateController
                │
        ┌───────┼────────┐
        ▼       ▼        ▼
      Player   Lane     Block
        │       │        │
        └───────┴────────┘
                │
                ▼
             Rendering
```

---

## 🌐 Network

Client의 Network Layer는 Server와의 TCP 연결과 Packet 처리를 담당합니다.

```text
TCP Connection
      ↓
ClientSession
      ↓
Receive Buffer
      ↓
Packet Parsing
      ↓
Packet ID
      ↓
PacketManager
      ↓
Packet Handler
      ↓
Game / UI Event
```

주요 구성:

| 영역              | 역할                  | 구현                                                              |
| --------------- | ------------------- | --------------------------------------------------------------- |
| Network Manager | Client 네트워크 생명주기 관리 | [`NetworkManager.cs`](Assets/Scripts/Network/NetworkManager.cs) |
| Client Session  | TCP 연결 및 송수신        | [`ClientSession.cs`](Assets/Scripts/Network/ClientSession.cs)   |
| Packet Manager  | 수신 Packet 분배        | [`PacketManager.cs`](Assets/Scripts/Network/PacketManager.cs)   |
| Handlers        | Server Packet 처리    | [`Handlers`](Assets/Scripts/Network/Handlers)                   |

Client는 Server에서 전달된 Packet을 Handler에서 처리하고, 필요한 경우 Event를 발생시켜 UI와 게임 시스템에 전달합니다.

---

## 🔄 State Synchronization

게임 진행 중 Client는 Server가 생성한 **Game State Snapshot**을 전달받습니다.

```text
Server
  │
  │ GameStateSnapshot
  ▼
Packet Handler
  │
  ▼
GameStateController
  │
  ├── Player
  ├── Lane
  ├── Blocks
  └── Game State
  │
  ▼
Game View
```

Client는 게임의 최종 상태를 직접 결정하지 않고 Server의 Snapshot을 기준으로 자신의 게임 화면을 갱신합니다.

관련 구현:

* [`GameStateController.cs`](Assets/Scripts/Systems/GamePlay/GameStateController.cs)
* [`Player`](Assets/Scripts/Systems/GamePlay/Player)
* [`Lane`](Assets/Scripts/Systems/GamePlay/Lane)
* [`Block`](Assets/Scripts/Systems/GamePlay/Block)

---

## 🎮 Game Play

게임 플레이 로직은 `GamePlay` 아래에서 관리합니다.

```text
GamePlay
│
├── Block
├── Car
├── Lane
├── Player
└── GameStateController
```

각 객체는 Server에서 전달받은 상태를 기반으로 게임 화면을 구성하고 갱신합니다.

### Player

Player의 게임 상태와 입력을 관리합니다.

### Car

게임 내 차량의 이동 및 상태를 표현합니다.

### Lane

게임 영역의 Block 상태와 Lane Scroll 등을 화면에 반영합니다.

### Block

Server에서 전달받은 Block 상태를 Client의 GameObject로 표현합니다.

관련 폴더:

* [`GamePlay`](Assets/Scripts/Systems/GamePlay)
* [`Player`](Assets/Scripts/Systems/GamePlay/Player)
* [`Lane`](Assets/Scripts/Systems/GamePlay/Lane)
* [`Block`](Assets/Scripts/Systems/GamePlay/Block)
* [`Car`](Assets/Scripts/Systems/GamePlay/Car)

---

## 🧩 Event-driven Architecture

Network Handler와 UI / Game System 사이의 직접적인 의존성을 줄이기 위해 Event를 사용합니다.

```text
Packet Handler
      │
      ▼
    Event
      │
 ┌────┴────┐
 ▼         ▼
UI       Game System
```

주요 Event:

* `NetworkEvents` — 연결 / 해제
* `LoginEvents` — 로그인
* `RoomEvents` — Room 상태
* `GameEvents` — 게임 진행 및 종료

관련 구현:

* [`Events`](Assets/Scripts/Events)

---

## 🔌 Connection & Disconnect

Client는 TCP 연결 상태를 관리하고, 연결이 끊어진 경우 게임 상태를 정리하고 Lobby / Title 화면으로 전환합니다.

```text
Connected
    │
    ▼
Heartbeat
    │
    ├── Normal
    │
    └── Timeout / Socket Error
              │
              ▼
          Disconnect
              │
              ▼
       Connection Event
              │
              ▼
       UI / Game Cleanup
```

또한 재연결 이후 Login Packet을 다시 전송하여 Server Session을 복구할 수 있도록 구성했습니다.

---

## 🖥️ Scene & UI Flow

Client의 주요 Scene은 게임 상태에 따라 분리되어 있습니다.

```text
Bootstrap
    ↓
Title
    ↓
Lobby
    ↓
Matchmaking
    ↓
Game
    ↓
Result
    ↓
Lobby
```

주요 Scene:

* [`Bootstrap.unity`](Assets/Scenes/Bootstrap.unity)
* [`Title.unity`](Assets/Scenes/Title.unity)
* [`Lobby.unity`](Assets/Scenes/Lobby.unity)
* [`Game.unity`](Assets/Scenes/Game.unity)
* [`Result.unity`](Assets/Scenes/Result.unity)

Scene 전환은 [`SceneLoader.cs`](Assets/Scripts/Managers/SceneLoader.cs)를 통해 관리합니다.

---

## 📦 Shared Module

Client와 Server에서 사용하는 Packet, Enum, Snapshot 등의 공통 데이터를 별도의 Repository로 관리합니다.

```text
             Block Racing Common
                    │
          ┌─────────┴─────────┐
          ▼                   ▼
       Client               Server
```

Client에서는 `Assets/Scripts/Common`을 Git Submodule로 연결하여 Server와 동일한 네트워크 데이터 구조를 사용합니다.

* [`Common`](Assets/Scripts/Common)

---

## 📁 Project Structure

```text
block-racing-client/
│
├── Assets/
│   ├── Art/
│   ├── Audio/
│   ├── Data/
│   ├── Prefabs/
│   ├── Scenes/
│   │
│   └── Scripts/
│       ├── Common/       # Shared Module
│       ├── Core/         # 공통 Core 기능
│       ├── Data/         # Client 상태 및 데이터
│       ├── Events/       # Event 정의
│       ├── Managers/     # Scene / Audio 등 관리
│       ├── Network/      # TCP / Packet 처리
│       └── Systems/
│           └── GamePlay/ # Player / Lane / Block / Car
│
├── Packages/
├── ProjectSettings/
└── README.md
```

---

## 🛠️ Tech Stack

| Category         | Technology                  |
| ---------------- | --------------------------- |
| Engine           | Unity 2022.3.62f3           |
| Language         | C#                          |
| Network          | TCP Socket                  |
| Architecture     | Server Authoritative Client |
| Shared Data      | Git Submodule               |
| IDE              | Visual Studio 2022          |
| Platform         | Windows                     |

---

## 🔗 Related Repositories

* [Block Racing](https://github.com/rlawodud89/block-racing)
* [Block Racing Server](https://github.com/rlawodud89/block-racing-server)
* [Block Racing Common](https://github.com/rlawodud89/block-racing-common)

---

## 🎯 Development Focus

Client의 핵심 목표는 **Server Authoritative 구조에서 서버 상태를 안정적으로 수신하고 게임 화면으로 반영하는 것**입니다.

주요 구현 내용:

* TCP 기반 실시간 통신
* Packet 기반 Client / Server 통신
* Server Snapshot 기반 State Synchronization
* Event 기반 UI / Game System 연결
* Connection / Disconnect Lifecycle
* Heartbeat 및 Reconnect
* Matchmaking / Room / Game UI 흐름
* Client Game State Rendering
* Client / Server Common Data 공유

Client → Server로 입력을 전달하고, Server → Client로 확정된 게임 상태를 전달하는 구조를 통해 **네트워크 통신과 게임 화면의 역할을 분리**했습니다.

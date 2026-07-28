# TINY DEFENSE

A lightweight 2D tower-defense/RTS hybrid where you spawn units to push down a lane, gather resources, and defend your tower.

Built with Unity as a portfolio project.

## Download

Download the latest playable Windows build from the **Releases** page:

➡️ https://github.com/WilliamHiiTW/TinyDefense/releases/latest

Extract the ZIP file and run `Tiny Defense.exe`.

> **Platform:** Windows (64-bit)

---

## Screenshots / Gameplay

<img width="1907" height="1070" alt="image" src="https://github.com/user-attachments/assets/a546aaae-6276-4594-acb5-df93a80cf16d" />
<img width="1912" height="1072" alt="image" src="https://github.com/user-attachments/assets/722db864-009f-44d3-9585-a14624c2a119" />
<img width="1910" height="1067" alt="image" src="https://github.com/user-attachments/assets/3ead618c-f372-4aef-8d5d-baba04e120ca" />
<img width="1912" height="1070" alt="image" src="https://github.com/user-attachments/assets/e5d332ea-7a59-4b03-9a7b-ded865ae4e58" />

## Features

- Real-time unit spawning with a gold economy
- Multiple unit types with distinct behavior: melee (Warrior, Lancer), ranged (Archer), and support (Monk healer)
- A pawn/worker unit that gathers resources and returns them to base
- Simple win/lose condition based on tower health
- Pooled projectiles for ranged combat (avoids per-shot allocation)

## Tech Stack

- Unity [VERSION — v6000.5.1f1]
- C#
- Universal Render Pipeline (URP)
- TextMesh Pro (UI)

## Project Structure

```
Assets/
├── Scripts/
│   ├── Controller/     # Spawning, animation, and projectile pooling controllers
│   ├── Manager/        # GameManager (game state, economy, win/lose)
│   └── General/
│       ├── Units/      # Unit base class + all unit type implementations
│       ├── Battle/      # Projectile
│       ├── Enums/       # UnitState
│       └── Resources/   # Collectible resource nodes
├── Animator/            # Animator controllers
├── Scenes/
├── Settings/            # URP render pipeline settings
└── TilePalette/
```

## Getting Started

1. Clone the repo.
2. Download the required third-party art assets — see [ATTRIBUTION.md](ATTRIBUTION.md) for the download link and where to place them (the project will show missing sprites without this step).
3. Open the project folder in Unity Hub with Unity [VERSION — v6000.5.1f1].
4. Open the main scene under `Assets/Scenes/` and press Play.

## License

Original source code is licensed under the [MIT License](LICENSE). Third-party art assets are **not** covered by this license — see [ATTRIBUTION.md](ATTRIBUTION.md).

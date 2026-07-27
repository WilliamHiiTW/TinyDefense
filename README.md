# [GAME TITLE]

[One or two sentence pitch — e.g. "A lightweight 2D tower-defense/RTS hybrid where you spawn units to push down a lane, gather resources, and defend your tower."]

Built with Unity as a portfolio project. Currently in active development.

## Screenshots / Gameplay

<!-- Add screenshots or a GIF here, e.g.: -->
<!-- ![Gameplay](docs/screenshot-01.png) -->

*(placeholder — add screenshots/gameplay GIFs here)*

## Features

- Real-time unit spawning with a gold economy
- Multiple unit types with distinct behavior: melee (Warrior, Lancer), ranged (Archer), and support (Monk healer)
- A pawn/worker unit that gathers resources and returns them to base
- Simple win/lose condition based on tower health
- Pooled projectiles for ranged combat (avoids per-shot allocation)

## Tech Stack

- Unity [VERSION — fill in]
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
3. Open the project folder in Unity Hub with Unity [VERSION — fill in].
4. Open the main scene under `Assets/Scenes/` and press Play.

## Known Limitations / Roadmap

*(This project is still in progress — a good place to note what's next, e.g.)*
- [ ] `UnitManager.cs` is currently a stub for future centralized unit tracking
- [ ] Add more unit/enemy variety
- [ ] Polish UI/UX

## License

Original source code is licensed under the [MIT License](LICENSE). Third-party art assets are **not** covered by this license — see [ATTRIBUTION.md](ATTRIBUTION.md).

# Third-Party Assets

This project uses the **Tiny Swords (Free Pack)** art asset pack by [Pixel Frog](https://pixelfrog-assets.itch.io/tiny-swords).

- Publisher: Pixel Frog
- Source: https://pixelfrog-assets.itch.io/tiny-swords
- License: Free for use in personal and commercial projects, but **redistribution, resale, or repackaging of the asset files (even modified) is not permitted**.

Because of that restriction, the asset pack itself is **not included in this repository** (it's excluded via `.gitignore`). If you clone this project and open it in Unity, sprites/animations that depend on this pack will show as missing until you complete the setup step below.

## Setup: restoring the art assets

1. Download the free pack from itch.io: https://pixelfrog-assets.itch.io/tiny-swords
2. Extract it so the folder is placed at:
   ```
   Assets/Tiny Swords (Free Pack)/
   ```
3. Reopen the project in Unity — sprite, animation, and prefab references should resolve automatically.

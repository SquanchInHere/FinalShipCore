![Status: In Development](https://img.shields.io/badge/status-in%20development-yellow)

# FinalShipCore

A modular Unity toolkit for building games with reusable modules, configurable module connections, UI Toolkit menu templates, localization, and asynchronous scene loading.

Import the asset into your project, customize the included templates, and add your own modules.

**Status:** early development. APIs may change.

## Requirements

- Unity **6000.6.6f1**.
- Input System **1.20.0**.
- Localization **1.5.14**, including its dependencies.

## ⚠️ IMPORTANT — INSTALL DEPENDENCIES FIRST
**Install Input System and Localization through Unity Package Manager before importing FinalShipCore.**

## Installation

1. [Download FinalShipCore-0.1.0.unitypackage](https://github.com/SquanchInHere/FinalShipCore/raw/refs/heads/main/FinalShipCore-0.1.0.unitypackage).
2. In Unity, select **Assets → Import Package → Custom Package…**.
3. Select the downloaded file and import all included assets.

The toolkit is imported into **Assets/FinalShipCore**. Its scripts, templates, settings, and scenes are editable.

This distribution uses `.unitypackage`, not installation through a Git URL in Package Manager. Do not keep another installed copy of FinalShipCore alongside it.

## Included systems

- **GameBootstrapper** — module initialization, input configuration, asynchronous preparation, startup, and shutdown.
- **GameModule** — shared `Initialize`, `Begin`, and `Shutdown` lifecycle.
- **ModuleComposer** — Inspector-configured module relationships and signal-to-command routing.
- **ModuleRelationRegistry** — typed `Get`, `GetMany`, and `GetPath` lookups.
- **LoadingPipeline** — sequential asynchronous startup tasks with cancellation.
- **MenuModule** — configurable UI Toolkit menu with localized button labels.
- **SceneFlowModule** — asynchronous scene transitions and loading UI.
- Menu and loading-screen templates, settings, English/Ukrainian translations, and example scenes.

## Run the example

1. Open **Edit → Project Settings → Localization**.
2. For a fresh project, select the included settings asset:

   `Assets/FinalShipCore/Runtime/Modules/Menu/Settings/Localization Settings.asset`

   If your project already uses localization, keep its settings and ensure the required locales and tables are registered.

3. Add these scenes to the enabled scene list in **Build Profiles**:

   - `Assets/FinalShipCore/Runtime/Modules/Menu/Scenes/MainMenu.unity`
   - `Assets/FinalShipCore/Runtime/Modules/Menu/Scenes/TestDestination.unity`

4. Open **MainMenu** and enter Play Mode.
5. Press **New Game** to load **TestDestination**.

The destination is intentionally empty. Check for **Scene transition completed** in the Hierarchy; no gameplay or camera is included in that scene.

No custom auto-installer is included. Active localization settings and the build scene list belong to the host project.

## Customize the templates

Menu resources are located under:

`Assets/FinalShipCore/Runtime/Modules/Menu`

- **Views** and **Styles** — menu layout and appearance.
- **Settings/MainMenuSettings.asset** — example buttons, labels, and actions.
- **Localization** — translation tables and locale assets.

Loading-screen resources are under:

`Assets/FinalShipCore/Runtime/Modules/SceneFlow`

You can edit the supplied assets or create your own settings and templates and assign them to the module components.

Keep `.meta` files when moving or sharing assets.

## Add your own modules

1. Create a class derived from `FinalShipCore.Runtime.Core.GameModule`.
2. Implement `Initialize`, `Begin`, and `Shutdown`.
3. Add the component to an active GameObject.
4. Register it once in the scene's **GameBootstrapper → Modules** array.
5. Configure Composer relationships if communication with other modules is needed.

For a module with its own assembly definition, reference:

- `FinalShipCore.Runtime.Core`
- `FinalShipCore.Runtime.Contracts`
- `FinalShipCore.Runtime.Modules` when using concrete built-in modules.

Keep game-specific modules independent of the core implementation.

## Current limitations

- Settings and Quit currently log requests.
- Continue is disabled in the example.
- Automatic loading progress and wait-for-continue input are unfinished.
- Scene transitions use Single mode; a persistent cross-scene loading service is not implemented.
- Some SceneFlow types retain `FinalShip.Modules.*` namespaces.
- Earlier Unity versions are not verified.

## Validation

The source game was reported working by the author. Static packaging checks passed, but this exact asset still needs clean-project import, compilation, Play Mode, and standalone-build verification.

## License

MIT. See [LICENSE.md](LICENSE.md).

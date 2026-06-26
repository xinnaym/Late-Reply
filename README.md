# Late Reply — Godot 4 C# Prototype

Psychological horror narrative prototype. 10-15 minute vertical slice.

## Project Structure

```
Late Reply/
├── project.godot              # Godot project config
├── Scenes/                    # Scene files (.tscn)
│   ├── Main.tscn              # Entry scene (game manager)
│   ├── Player.tscn            # Player character
│   ├── Bedroom.tscn           # Intro bedroom level
│   ├── PhoneUI.tscn           # Phone message UI
│   ├── ComputerUI.tscn        # Computer interface
│   ├── DialogueUI.tscn        # Dialogue overlay
│   └── VHSEffect.tscn         # VHS post-processing
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs    # Movement, sprint, state
│   │   └── ThirdPersonCamera.cs   # Camera follow + orbit
│   ├── Interaction/
│   │   ├── InteractionSystem.cs   # Raycast interaction
│   │   └── Interactable.cs        # Base interactable component
│   ├── UI/
│   │   ├── PhoneUI.cs             # Phone message interface
│   │   ├── ComputerUI.cs          # Computer screen UI
│   │   └── DialogueUI.cs          # Dialogue overlay
│   ├── Systems/
│   │   ├── EventManager.cs        # Global event bus (autoload)
│   │   ├── SaveSystem.cs          # Save/load stub (autoload)
│   │   ├── PanicAttackEffect.cs   # Panic attack VFX
│   │   └── VHSShaderController.cs # VHS post-processing control
│   ├── Triggers/
│   │   └── TriggerZone.cs         # Trigger volume for events
│   └── Narrative/
│       ├── PhoneMessage.cs        # Phone message data
│       └── ComputerNote.cs        # Computer note data
├── Shaders/
│   └── vhs_postprocess.gdshader   # VHS visual effect
└── Assets/                         # Placeholder assets
```

## Systems

- **PlayerController** — WASD movement, sprint (Shift), third-person camera
- **InteractionSystem** — Raycast-based interaction with `E` key
- **DialogueUI** — Text overlay for narrative moments
- **PhoneUI** — Phone message with reply choices
- **ComputerUI** — Fake desktop with chat, browser, notes, music
- **EventManager** — Signal bus for decoupled communication
- **PanicAttackEffect** — Tunnel vision, shake, distortion
- **VHSShaderController** — Post-processing VHS effect
- **TriggerZone** — Area3D-based event triggers
- **SaveSystem** — Stub for future save/load

## How to Run

1. Open `project.godot` in Godot 4.3+
2. Build C# solution (Build > Build Solution)
3. Press F5 to run

## Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| Shift | Sprint |
| E | Interact |
| Escape | Pause |

## Design Notes

- No combat — exploration and atmosphere only
- No jumpscares — discomfort and confusion
- Placeholder-friendly: replace meshes/materials with final art
- Modular: each system is self-contained with EventManager for communication

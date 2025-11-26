# .NET Version - Installation & Play Instructions

## Prerequisites

### Required Software
- **.NET 8.0 SDK** or later ([Download here](https://dotnet.microsoft.com/download))
- **Visual Studio 2022** (recommended) or any C# IDE
- **Windows** (tested), Linux, or macOS with MonoGame support

### Verify Installation
Open a terminal and run:
```bash
dotnet --version
```
You should see version 8.0 or higher.

---

## Installation

### 1. Clone the Repository
```bash
git clone https://github.com/DJ618/turret_game.git
cd turret_game
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

---

## Running the Game

### Option 1: Command Line
From the repository root:
```bash
cd TurretGame.Game
dotnet run
```

### Option 2: Visual Studio
1. Open `TurretGame.sln` in Visual Studio
2. Set `TurretGame.Game` as the startup project (right-click → Set as Startup Project)
3. Press `F5` or click the "Start" button

### Option 3: Run Compiled Executable
After building:
```bash
cd TurretGame.Game/bin/Debug/net8.0
./TurretGame.Game.exe
```

---

## Game Controls

| Control | Action |
|---------|--------|
| **WASD** | Move player (cyan circle) |
| **Mouse Click** | Place turret during build phase |
| **ESC** | Quit game |

---

## Gameplay Overview

### Objective
Survive waves of enemies by hunting prey while avoiding hunters. Collect resources to upgrade and place turrets.

### Enemies
- **Prey (Green)**: Flee from you - hunt them down for resources
- **Hunter (Red)**: Chase you - avoid them or die

### Resources
- **Gold Coins**: Dropped by killed enemies - walk over to collect
- Use resources to select upgrades between waves

### Upgrades (Between Waves)
- **Extra Turret**: Place additional turrets (+2 max)
- **Faster Projectiles**: Increase turret projectile speed (+10%)
- **Better Resources**: Increase resource value per pickup (+1)
- **Slow Chase Enemies**: Reduce hunter speed (-5%)
- **Slow Fleeing Enemies**: Reduce prey speed (-5%)

### Turrets
- **Placement**: During build phase, click to place turrets (max 15 total)
- **Auto-targeting**: Turrets automatically shoot nearest enemy
- **Strategy**: Position turrets to cover wide areas and choke points

### Progression
- Each wave increases enemy count (Fibonacci sequence for prey)
- Enemy speed increases 1% per wave (capped at 90% of player speed)
- Hunters increase linearly with wave number
- Game over when a hunter catches you

---

## Project Structure

The .NET version uses **Clean Architecture** with dependency injection:

```
TurretGame.Core/          # Domain entities and interfaces
TurretGame.Infrastructure/  # MonoGame rendering and input
TurretGame.Application/     # Game systems and logic
TurretGame.Game/           # Entry point and composition root
```

---

## Configuration

Edit `TurretGame.Game/appsettings.json` to adjust game settings:

```json
{
  "GameSettings": {
    "PlayerSpeed": 300.0,
    "PreySpeed": 140.0,
    "HunterSpeed": 200.0,
    "HuntersPerWave": 2
  }
}
```

Changes are applied in real-time (hot reload supported).

---

## Troubleshooting

### "Project not found" Error
Ensure you're in the correct directory:
```bash
cd TurretGame.Game
dotnet run
```

### Graphics/Display Issues
- Make sure your GPU drivers are up to date
- The game runs in fullscreen by default
- Check MonoGame compatibility with your OS

### Build Errors
Clean and rebuild:
```bash
dotnet clean
dotnet build
```

---

## System Requirements

### Minimum
- **OS**: Windows 10+, Linux (with X11), macOS 10.13+
- **RAM**: 512 MB
- **GPU**: Any with OpenGL 3.0+ support
- **.NET**: 8.0 SDK

### Recommended
- **OS**: Windows 11
- **RAM**: 2 GB
- **GPU**: Dedicated graphics card
- **.NET**: Latest 8.0 SDK

---

## Development

To modify the game:
1. Open `TurretGame.sln` in Visual Studio
2. Make changes to any project
3. Build and run to test

See [CLAUDE.md](CLAUDE.md) for detailed project architecture and design decisions.

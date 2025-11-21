# Turret Game - Project Definition

## Game Overview
A 2D top-down roguelike wave game blending hunting and survival with economic strategy and automated turret systems. Hunt down fleeing enemies while avoiding dangerous pursuers - a dual-threat gameplay loop.

## Core Design Principles
- **System-heavy**: Focus on gameplay systems over content creation
- **Mechanic-focused**: Tight core loop, expandable design
- **Simply addicting**: Tap into psychological hooks that drive replayability

## Psychological Hooks (Design Mandates)
1. Immediate feedback loop - action → result in <1 second
2. Progressive mastery - skill ceiling with visible improvement
3. Variable rewards - unpredictable positive outcomes
4. "One more round" syndrome - sessions under 5 minutes
5. Compound progression - small advantages stack exponentially

## Game Loop
1. **Wave Phase**: Hunt fleeing enemies while avoiding pursuers, turrets provide firepower
2. **Build Phase**: Spend collected resources on turrets/systems/upgrades
3. **Repeat**: Waves escalate in difficulty (more enemies, mixed threat types), economy compounds
4. **Death**: Run ends, meta-progression unlocks persist

## Core Mechanics

### Player Control
- **Movement only** - WASD movement, no direct combat
- Player actively hunts fleeing enemies while dodging pursuers
- Positioning is strategic (herding prey toward turrets, avoiding hunters)
- Success requires balancing offensive hunting with defensive survival

### Perspective
- **Top-down view**
- 360° threat awareness
- Clear turret placement visualization

### Enemy Behavior (Dual-Threat System)
- **Two enemy archetypes with opposite behaviors:**
  - **PREY (majority ~80%)**: FLEE from player and turrets - evasion AI, must be hunted down
  - **HUNTERS (minority ~20%)**: CHASE the player - aggressive pursuit AI, create survival pressure
- Creates dynamic tension: hunt the many while avoiding the few
- Different enemy types have different behaviors within each archetype
  - Prey: fast runners, erratic dodgers, teleporters
  - Hunters: fast rushers, tanky bruisers, trappers
- Player must balance offensive hunting with defensive survival

### Economy System (Dual-Source)
- **Combat drops**: Killing enemies drops resources (incentivizes aggression)
- **Passive generators**: Purchased systems produce resources over time (incentivizes investment)
- Strategic balance between aggressive collection and passive income builds

### Roguelike Elements
- Procedural upgrade choices (random options between waves)
- Synergy discovery (combos create emergent strategies)
- Permadeath with meta-currency
- Run variety through unlockable starting options/classes

## Technical Architecture

### Technology Stack
- **.NET 8.0** (LTS)
- **MonoGame 3.8.4.1** (mgdesktopgl)
- **Microsoft.Extensions.DependencyInjection 10.0.0**
- **Development Environment**: Visual Studio

### Solution Structure (Clean Architecture)
```
TurretGame.Core/          # Entities, interfaces, core game logic (framework-agnostic)
TurretGame.Infrastructure/ # MonoGame implementations, content pipeline, rendering
TurretGame.Application/    # Game systems, managers, state machines, business logic
TurretGame.Game/          # MonoGame entry point, composition root
```

### Project Dependencies
```
TurretGame.Core (no dependencies)
    ↑
    ├── TurretGame.Infrastructure (MonoGame 3.8.4.1)
    └── TurretGame.Application
            ↑
            └── TurretGame.Game (MonoGame 3.8.4.1, DI 10.0.0)
```

### Detailed Project Organization

#### TurretGame.Core (Framework-Agnostic Domain)
**No external dependencies** - Pure C# business logic

```
Core/
├── Entities/
│   ├── Player.cs              # Player state, movement, resource tracking
│   ├── Enemy.cs                # Enemy state, dual-threat AI (Hunter/Prey)
│   └── ResourcePickup.cs       # Collectible resource drops
├── Interfaces/
│   ├── IInputService.cs        # Input abstraction contract
│   └── ICollisionHandler.cs    # Collision response contract
├── State/
│   └── GameState.cs            # Game state enum (Playing, GameOver)
├── Utilities/
│   ├── CollisionDetection.cs  # Circle-to-circle collision math
│   └── Bounds.cs               # Viewport boundary struct
└── sprites/
    └── coins/                  # Source sprite assets (not compiled)
        ├── Coin_One.png
        ├── Coin_Two.png
        ├── Coin_Three.png
        └── Coin_Four.png
```

**Key Principles:**
- No MonoGame or framework types (uses System.Numerics.Vector2)
- Portable to any rendering engine
- Unit testable without graphics context

#### TurretGame.Infrastructure (MonoGame Implementations)
**Dependencies:** MonoGame.Framework.DesktopGL 3.8.4.1

```
Infrastructure/
├── Input/
│   └── MonoGameInputService.cs    # WASD keyboard input implementation
├── Graphics/
│   ├── TextureFactory.cs          # Procedural texture generation
│   └── SpriteAnimator.cs          # Reusable sprite sheet animation
└── Rendering/
    ├── EntityRenderer.cs          # Player/Enemy/Pickup rendering
    └── UIRenderer.cs              # Game over overlay, UI text
```

**Responsibilities:**
- MonoGame-specific implementations of Core interfaces
- Texture creation and management
- SpriteBatch rendering logic
- Coordinate system conversion (System.Numerics → XNA)

#### TurretGame.Application (Business Logic Layer)
**Dependencies:** TurretGame.Core

```
Application/
├── Systems/
│   ├── EntityManager.cs        # Entity collection management
│   ├── CollisionSystem.cs      # Collision detection orchestration
│   ├── SpawnManager.cs         # Enemy spawning logic
│   └── ResourceManager.cs      # Resource count tracking
├── State/
│   └── GameStateManager.cs     # State transitions (Playing ↔ GameOver)
├── GameplayLoop.cs             # Main update loop orchestrator
└── GameCollisionHandler.cs     # Collision response implementation
```

**Responsibilities:**
- Game systems coordination
- Entity lifecycle management
- Collision detection and response
- State management
- No rendering or input code

#### TurretGame.Game (Composition Root)
**Dependencies:** All projects, DI container

```
Game/
├── Content/
│   ├── Content.mgcb            # MonoGame content pipeline
│   ├── CoinSprite.png          # Compiled coin animation
│   └── GameFont.spritefont     # Arial Bold 48pt font
├── Game1.cs                    # MonoGame entry point (~200 lines)
└── Program.cs                  # Application entry point
```

**Game1.cs Responsibilities (Coordination Only):**
- Dependency injection container setup
- Service registration (ConfigureServices)
- MonoGame lifecycle (Initialize, LoadContent, Update, Draw)
- Thin delegation to systems

### Dependency Injection Architecture

**Service Registration (`Game1.cs:79-136`):**
```csharp
ConfigureServices()
├── Core Services
│   └── Player (singleton)
├── Application Layer
│   ├── EntityManager
│   ├── GameStateManager
│   ├── ResourceManager
│   ├── SpawnManager
│   ├── CollisionSystem
│   ├── ICollisionHandler → GameCollisionHandler
│   └── GameplayLoop (orchestrator)
└── Infrastructure Layer
    ├── IInputService → MonoGameInputService
    ├── TextureFactory
    ├── SpriteAnimator (coin animation)
    ├── EntityRenderer
    └── UIRenderer
```

**Benefits:**
- Loose coupling via interfaces
- Easy to swap implementations
- Simplified testing (mock injection)
- Clear dependency graph
- Single Responsibility Principle enforced

### Architecture Flow

**Update Loop:**
```
User Input (Keyboard)
    ↓
MonoGameInputService (Infrastructure)
    ↓
GameplayLoop.Update() (Application)
    ├→ Player.Update() (Core)
    ├→ EntityManager.UpdateEnemies() (Application)
    │   └→ Enemy.Update() (Core)
    ├→ CollisionSystem.CheckCollisions() (Application)
    │   └→ GameCollisionHandler (Application)
    │       ├→ GameStateManager (state change)
    │       ├→ ResourceManager (collect)
    │       └→ EntityManager (spawn pickup)
    └→ EntityManager.RemoveCollectedPickups()
```

**Render Loop:**
```
Game1.Draw()
    ↓
EntityRenderer (Infrastructure)
    ├→ DrawPlayer()
    ├→ DrawEnemy() (for each)
    └→ DrawPickup() (for each, animated)
    ↓
UIRenderer (Infrastructure)
    └→ DrawGameOver() (if game over)
```

### Code Organization Principles

1. **Separation of Concerns**: Each class has one responsibility
2. **Dependency Inversion**: Depend on interfaces, not implementations
3. **Single Responsibility**: Files average 40-60 lines
4. **Framework Independence**: Core logic has zero MonoGame dependencies
5. **Testability**: Business logic unit testable without graphics context
6. **Composition over Inheritance**: Systems composed, not extended

## Development Approach
- **Incremental development**: One small feature at a time
- **Manager-Engineer relationship**: Design and review together, implement systematically
- **Production quality**: Clean architecture, maintainable code, proper separation of concerns

## Sprint 1 Goals (Completed)
1. Player entity with WASD movement ✓
2. Enemy entity with chase AI (HUNTER type) ✓
3. Collision detection system ✓
4. Add flee AI (PREY type) ✓
5. Resource drop on enemy death ✓
6. Animated coin sprite pickups ✓
7. Clean architecture refactoring ✓

## Current Implementation Status
- **Core gameplay loop**: Fully functional
- **Dual-threat AI system**: Implemented (Hunters chase, Prey flee)
- **Resource collection**: Working with animated coin sprites
- **Game over state**: Functional with proper UI
- **Architecture**: Clean architecture with DI fully implemented
- **Code quality**: 16 focused classes, single responsibilities, ~200 lines in Game1.cs

---

## Design Decisions Log

### Decided
- Genre: 2D system-heavy, mechanic-focused
- **Hybrid archetype: Wave hunting/survival + economic strategy** - unique dual-threat system
- **Core dynamic: HUNT fleeing enemies while SURVIVING pursuing enemies**
- Roguelike structure with meta-progression
- Player control: Movement only (no direct combat) - offensive and defensive positioning
- Perspective: Top-down
- Economy: Dual-source (combat drops + passive generators)
- **Enemy AI: Dual-Threat System**
  - **80% PREY** - flee from player/turrets (hunting gameplay)
  - **20% HUNTERS** - chase player (survival gameplay)
- **Architecture: Clean Architecture with Dependency Injection**
  - 4-layer separation (Core, Infrastructure, Application, Game)
  - Framework-agnostic core logic
  - Dependency injection for testability
  - Single Responsibility Principle enforced
- Working title: "Turret Game"

### Major Design Pivots
- **Sprint 1.2 Initial Pivot**: Considered "hunt fleeing enemies only" time-attack gameplay
- **Sprint 1.2 Reversal**: Considered "traditional survival only" with all enemies chasing
- **Sprint 1.2 Final Decision**: Hybrid system with BOTH flee and chase enemies
- Creates unique tension: offensive hunting vs defensive survival simultaneously
- Primary challenge: Balancing aggression (hunt prey) with caution (avoid hunters)
- **Sprint 1.3 Architecture Refactor**: Massive refactoring from God Object to Clean Architecture
  - Game1.cs reduced from 316 lines to ~200 lines
  - Created 16 focused classes across 4 architectural layers
  - Implemented dependency injection for loose coupling
  - Separated concerns: Input, Rendering, Business Logic, Coordination
  - Result: Testable, maintainable, extensible codebase ready for expansion

### To Be Decided
- Exact ratio of prey vs hunters (starting at 80/20)
- Specific turret types and upgrade paths
- Meta-progression unlock structure
- Enemy variety within each archetype
- Wave scaling formulas (how ratios change per wave)
- Art style and visual direction (must distinguish prey from hunters clearly)
- Audio design approach
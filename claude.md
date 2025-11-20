# Turret Game - Project Definition

## Game Overview
A 2D top-down roguelike wave survival game with economic strategy and automated turret defense systems.

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
1. **Wave Phase**: Player dodges enemies while turrets provide firepower
2. **Build Phase**: Spend collected resources on turrets/systems/upgrades
3. **Repeat**: Waves escalate in difficulty, economy compounds
4. **Death**: Run ends, meta-progression unlocks persist

## Core Mechanics

### Player Control
- **Movement only** - WASD dodging, no direct combat
- Player positioning is strategic (kiting, resource collection, turret coverage)
- Survival depends on movement skill + turret placement

### Perspective
- **Top-down view**
- 360° threat awareness
- Clear turret placement visualization

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
- **MonoGame 3.8.1.303** (mgdesktopgl template)
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
    ├── TurretGame.Infrastructure
    └── TurretGame.Application
            ↑
            └── TurretGame.Game
```

## Development Approach
- **Incremental development**: One small feature at a time
- **Manager-Engineer relationship**: Design and review together, implement systematically
- **Production quality**: Clean architecture, maintainable code, proper separation of concerns

## Sprint 1 Goals (Current Focus)
1. Player entity with WASD movement
2. Single enemy type with basic AI (chase player)
3. Collision detection system
4. Resource drop on enemy death
5. Simple resource counter UI display

---

## Design Decisions Log

### Decided
- Genre: 2D system-heavy, mechanic-focused
- Hybrid archetype: Wave survival + economic strategy (Vampire Survivors meets resource management)
- Roguelike structure with meta-progression
- Player control: Movement/dodging only (no direct combat)
- Perspective: Top-down
- Economy: Dual-source (combat drops + passive generators)
- Working title: "Turret Game"

### To Be Decided
- Specific turret types and upgrade paths
- Meta-progression unlock structure
- Enemy variety and behavior patterns
- Wave scaling formulas
- Art style and visual direction
- Audio design approach
# Turret Game - Project Definition

## Game Overview
A 2D top-down roguelike wave hunting game with economic strategy and automated turret systems. Hunt down fleeing enemies as fast as possible in time-attack waves where YOU are the predator.

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
6. Speedrun optimization - time-based leaderboards create competition
7. Perfect runs - reward optimal pathing and hunting strategy

## Game Loop
1. **Wave Phase**: Hunt down fleeing enemies using turrets and strategic positioning
2. **Build Phase**: Spend collected resources on turrets/systems/upgrades to hunt faster
3. **Repeat**: Waves escalate in difficulty (more/faster enemies), economy compounds
4. **Death**: Run ends, but primary challenge is SPEED not survival
5. **Victory**: Complete wave when all enemies eliminated - score based on completion time

## Core Mechanics

### Player Control
- **Movement only** - WASD movement, no direct combat
- Player actively hunts and herds enemies toward turrets
- Positioning is strategic (cutting off escape routes, corralling prey)
- Success depends on movement skill, turret placement, and hunting efficiency

### Perspective
- **Top-down view**
- 360° threat awareness
- Clear turret placement visualization

### Enemy Behavior (Prey Dynamics)
- **Enemies FLEE from player and turrets** - evasion AI, not aggression
- Enemies attempt to escape and survive
- Different enemy types have different evasion strategies (fast runners, erratic dodgers, etc.)
- Player becomes the threat, reversing traditional predator/prey dynamics

### Victory Condition
- Wave complete when ALL enemies are eliminated
- Score based on completion TIME (faster = better)
- Faster kills = better upgrades and more meta-currency
- Death still ends run, but optimal play focuses on speed

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
1. Player entity with WASD movement ✓
2. Single enemy type with basic AI (FLEE from player) - DESIGN PIVOT
3. Collision detection system ✓
4. Resource drop on enemy death
5. Simple resource counter UI display

---

## Design Decisions Log

### Decided
- Genre: 2D system-heavy, mechanic-focused
- **Hybrid archetype: Wave hunting + economic strategy + time attack**
- **Core dynamic: PREDATOR (player) hunts PREY (enemies)**
- Roguelike structure with meta-progression
- Player control: Movement/herding only (no direct combat)
- Perspective: Top-down
- Economy: Dual-source (combat drops + passive generators)
- **Victory condition: Time-based scoring (eliminate all enemies FAST)**
- **Enemy AI: Flee/evade behavior (not chase/attack)**
- Working title: "Turret Game"

### Major Design Pivots
- **Sprint 1.2 Pivot**: Changed from "survive enemy attacks" to "hunt fleeing enemies"
- Inverted predator/prey relationship - player is now the threat
- Primary challenge shifted from survival to speed optimization

### To Be Decided
- Specific turret types and upgrade paths
- Meta-progression unlock structure
- Enemy evasion variety and behavior patterns
- Wave scaling formulas (number/speed of prey)
- Time-based scoring and reward tiers
- Art style and visual direction
- Audio design approach
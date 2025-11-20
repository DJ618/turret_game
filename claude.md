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
2. Enemy entity with chase AI (HUNTER type) ✓
3. Collision detection system ✓
4. Add flee AI (PREY type) - in progress
5. Resource drop on enemy death
6. Simple resource counter UI display

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
- Working title: "Turret Game"

### Major Design Pivots
- **Sprint 1.2 Initial Pivot**: Considered "hunt fleeing enemies only" time-attack gameplay
- **Sprint 1.2 Reversal**: Considered "traditional survival only" with all enemies chasing
- **Sprint 1.2 Final Decision**: Hybrid system with BOTH flee and chase enemies
- Creates unique tension: offensive hunting vs defensive survival simultaneously
- Primary challenge: Balancing aggression (hunt prey) with caution (avoid hunters)

### To Be Decided
- Exact ratio of prey vs hunters (starting at 80/20)
- Specific turret types and upgrade paths
- Meta-progression unlock structure
- Enemy variety within each archetype
- Wave scaling formulas (how ratios change per wave)
- Art style and visual direction (must distinguish prey from hunters clearly)
- Audio design approach
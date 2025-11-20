# TurretGame - MVP Development Roadmap

## MVP Definition
Playable game demonstrating the core addictive loop: hunt fleeing enemies, optimize kill time, collect resources, place turrets, get faster, compete for best times.

**CORE DYNAMIC: Player is PREDATOR, enemies are PREY**

**MVP Success Criteria:**
- 3-5 minute runs feel complete
- Player wants to optimize "one more run" for better time
- Core hunting loop is functional and satisfying
- Time-attack scoring creates replay motivation

## Technical Stack
- **.NET 8.0** (LTS)
- **MonoGame 3.8.1.303** (mgdesktopgl template)
- **Development Environment**: Visual Studio

---

## MAJOR DESIGN PIVOT (Sprint 1.2)

**OLD CONCEPT**: Survive waves of enemies chasing you
**NEW CONCEPT**: Hunt down enemies that flee from you

**What Changed:**
- Enemies now FLEE from player and turrets (evasion AI)
- Player is the PREDATOR, enemies are PREY
- Primary challenge is SPEED not survival
- Victory = eliminate all enemies FAST (time-attack)
- Scoring based on completion time
- Gameplay loop: Hunt → Optimize → Compete for best times

This inverts the traditional predator/prey relationship and shifts focus from defensive survival to aggressive hunting optimization.

---

## Phase 1: Foundation (Weeks 1-2)

### Sprint 1.1: Project Setup & Player Movement
- Solution structure scaffolding
- Player entity (position, velocity)
- WASD movement with bounds checking
- Basic rendering (colored rectangle for player)
- **Deliverable**: Player moves around empty screen

### Sprint 1.2: Enemy Basics
- Enemy entity (position, velocity, health)
- **FLEE AI (move AWAY from player) - DESIGN PIVOT**
- Spawning system (spawn at screen edges)
- Basic collision detection (circle-based)
- **Deliverable**: Enemy flees from player, collision detected

### Sprint 1.3: Combat & Resources
- Enemy death on collision with player (placeholder for turret damage)
- Resource drop entity (pickups)
- Player collection on overlap
- Resource counter UI
- Wave timer display
- **Deliverable**: Hunt enemy → collect resource → counter updates, time tracked

---

## Phase 2: Core Loop (Weeks 3-4)

### Sprint 2.1: Wave System
- Wave state machine (hunt → build → next wave)
- Wave timer (tracks completion time for scoring)
- Enemy spawning (X enemies per wave, increases each wave)
- Wave completion detection (all enemies eliminated)
- **Deliverable**: Hunt all enemies → record time → build phase → next wave

### Sprint 2.2: Turret Placement
- Turret entity (position, range, damage, fire rate)
- Placement UI/system (click to place during build phase)
- Cost system (deduct resources on placement)
- Turret rendering (distinguish from player/enemies)
- **Deliverable**: Place turrets, they appear on field

### Sprint 2.3: Turret Combat
- Target acquisition (find nearest fleeing enemy in range)
- Projectile entity (position, velocity, damage)
- Projectile-enemy collision
- Enemy health/death from turret damage
- **Deliverable**: Turrets help hunt down and eliminate fleeing enemies

---

## Phase 3: Economy & Progression (Weeks 5-6)

### Sprint 3.1: Passive Generators
- Generator entity (production rate, cost)
- Resource generation over time
- Generator placement system
- Visual distinction (generator vs turret)
- **Deliverable**: Place generators, passive income flows

### Sprint 3.2: Upgrade System
- Upgrade options UI (3 random choices between waves)
- Upgrade definitions (turret damage +X%, generator rate +Y%)
- Upgrade application to existing entities
- Synergy tracking (buffs stack)
- **Deliverable**: Choose upgrades, see immediate effects

### Sprint 3.3: Difficulty Scaling
- Wave scaling formula (more enemies, faster flee speed, better evasion)
- Resource drop scaling
- Balance tuning (ensure challenging hunt times increase progressively)
- **Deliverable**: Enemies become harder to hunt, more evasive each wave

---

## Phase 4: Roguelike Structure (Week 7)

### Sprint 4.1: Run Completion & Scoring
- Run summary screen (waves completed, total time, enemies hunted, time rankings)
- Time-based scoring system (faster = better rewards)
- Restart flow (clear entities, reset state)
- **Deliverable**: Complete run → see times/scores → restart for better performance

### Sprint 4.2: Meta-Progression
- Meta-currency rewards (based on time performance and waves completed)
- Persistent unlock system (new turret types, hunting speed bonuses)
- Simple unlock UI (spend meta-currency between runs)
- Save/load system for meta-progression and best times
- **Deliverable**: Each run earns unlocks and records best times

---

## Phase 5: Polish & Juice (Week 8)

### Sprint 5.1: Visual Feedback
- Death animations/effects
- Projectile trails
- Screen shake on impacts
- Resource pickup effects
- **Deliverable**: Game feels satisfying to play

### Sprint 5.2: Audio & UI Polish
- Basic SFX (shoot, hit, death, pickup)
- Background music loop
- Clear UI (resources, wave counter, health)
- Menu system (start, restart, quit)
- **Deliverable**: MVP feels complete

---

## Post-MVP Expansion Ideas (Future)
- More turret types (AOE, slow/trap, guided missiles)
- More enemy evasion types (fast runners, erratic dodgers, teleporters)
- Boss hunts every 5 waves (ultra-evasive prey)
- Map obstacles/terrain (creates escape routes and chokepoints)
- Player abilities (dash for closing distance, temporary speed boost)
- Global leaderboards for best times per wave
- Replay system to study optimal hunting patterns

---

**Estimated Timeline: 8 weeks part-time (2-3 hours/day)**
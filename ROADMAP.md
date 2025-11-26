# TurretGame - MVP Development Roadmap

## MVP Definition
Playable game demonstrating the core addictive loop: hunt fleeing enemies while avoiding pursuers, collect resources, place turrets, get stronger, balance offense and defense.

**CORE DYNAMIC: HUNT the many (prey) while SURVIVING the few (hunters)**

**MVP Success Criteria:**
- 3-5 minute runs feel complete
- Player wants to try "one more run"
- Dual-threat gameplay (hunting + survival) is functional and creates tension

## Technical Stack
- **.NET 8.0** (LTS)
- **MonoGame 3.8.1.303** (mgdesktopgl template)
- **Development Environment**: Visual Studio

---

## DESIGN EVOLUTION (Sprint 1.2)

**ORIGINAL CONCEPT**: Survive waves of enemies chasing you (traditional survival)
**ATTEMPTED PIVOT**: Hunt down enemies that flee from you (pure hunting)
**FINAL DECISION**: Hybrid system with BOTH enemy types

**Dual-Threat System:**
- **~80% PREY enemies**: FLEE from player and turrets - must be hunted down
- **~20% HUNTER enemies**: CHASE the player - must be avoided
- Creates unique tension: offensive hunting vs defensive survival
- Player balances aggression (chase prey) with caution (avoid hunters)
- Gameplay loop: Hunt → Survive → Build → Repeat with more threats

**Why this works:**
- Combines best of both designs: hunting gameplay + survival pressure
- Creates dynamic positioning challenges
- Visual clarity needed to distinguish prey (green?) from hunters (red?)
- More interesting than pure survival or pure hunting alone

---

## Phase 1: Foundation (Weeks 1-2)

### Sprint 1.1: Project Setup & Player Movement
- Solution structure scaffolding
- Player entity (position, velocity)
- WASD movement with bounds checking
- Basic rendering (colored rectangle for player)
- **Deliverable**: Player moves around empty screen

### Sprint 1.2: Enemy Basics & Dual AI
- Enemy entity (position, velocity, health, type)
- **HUNTER AI** (move TOWARD player) - chase/pursuit behavior
- **PREY AI** (move AWAY from player) - flee/evade behavior
- Spawning system (spawn at screen edges, 80% prey / 20% hunters)
- Basic collision detection (circle-based)
- **Deliverable**: Two enemy types with opposite behaviors, collision detected

### Sprint 1.3: Combat & Resources
- Enemy death on collision with player (placeholder for turret damage)
- Resource drop entity (pickups)
- Player collection on overlap
- Resource counter UI
- **Deliverable**: Kill enemy → collect resource → counter updates

---

## Phase 2: Core Loop (Weeks 3-4)

### Sprint 2.1: Wave System
- Wave state machine (preparation → wave → build)
- Wave timer/counter
- Enemy spawning (X enemies per wave, increases each wave, maintains ~80/20 prey/hunter ratio)
- Wave completion detection
- **Deliverable**: Waves spawn with mixed enemy types, escalate, pause for build phase

### Sprint 2.2: Turret Placement
- Turret entity (position, range, damage, fire rate)
- Placement UI/system (click to place during build phase)
- Cost system (deduct resources on placement)
- Turret rendering (distinguish from player/enemies)
- **Deliverable**: Place turrets, they appear on field

### Sprint 2.3: Turret Combat
- Target acquisition (find nearest enemy in range, regardless of type)
- Projectile entity (position, velocity, damage)
- Projectile-enemy collision
- Enemy health/death from turret damage
- **Deliverable**: Turrets shoot and kill both prey and hunter enemies

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
- Wave scaling formula (total enemies, health, speed for both types)
- Prey/Hunter ratio adjustments (e.g., more hunters at higher waves?)
- Resource drop scaling
- Balance tuning (dual threat becomes overwhelming around wave 10-15)
- **Deliverable**: Both hunting and survival challenges escalate appropriately

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
- More turret types (AOE, slow/trap for prey, stun/knockback for hunters, guided missiles)
- More enemy types within each archetype:
  - PREY variants: fast runners, erratic dodgers, teleporters, burrowers
  - HUNTER variants: fast rushers, tanky bruisers, trappers, ambushers
- Boss waves every 5 waves (ultra-evasive prey + elite hunter combo)
- Map obstacles/terrain (creates escape routes for prey, ambush points for hunters)
- Player abilities (dash for closing distance on prey, shield/dodge for avoiding hunters)
- Dynamic prey/hunter ratios (waves could be all-hunters for intense survival)
- Upgrades that specifically benefit against prey vs hunters

---

**Estimated Timeline: 8 weeks part-time (2-3 hours/day)**

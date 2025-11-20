# TurretGame - MVP Development Roadmap

## MVP Definition
Playable game demonstrating the core addictive loop: survive waves, collect resources, place turrets, get stronger, eventually die, want to replay immediately.

**MVP Success Criteria:**
- 3-5 minute runs feel complete
- Player wants to try "one more run"
- Core loop is functional and fun

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
- Simple chase AI (move toward player)
- Spawning system (manual spawn for testing)
- Basic collision detection (circle or AABB)
- **Deliverable**: Enemy chases player, collision detected

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
- Enemy spawning (X enemies per wave, increases each wave)
- Wave completion detection
- **Deliverable**: Waves spawn, escalate, pause for build phase

### Sprint 2.2: Turret Placement
- Turret entity (position, range, damage, fire rate)
- Placement UI/system (click to place during build phase)
- Cost system (deduct resources on placement)
- Turret rendering (distinguish from player/enemies)
- **Deliverable**: Place turrets, they appear on field

### Sprint 2.3: Turret Combat
- Target acquisition (find nearest enemy in range)
- Projectile entity (position, velocity, damage)
- Projectile-enemy collision
- Enemy health/death from turret damage
- **Deliverable**: Turrets shoot and kill enemies

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
- Wave scaling formula (enemies, health, speed)
- Resource drop scaling
- Balance tuning (ensure death around wave 10-15 for MVP)
- **Deliverable**: Game gets harder, eventually kills player

---

## Phase 4: Roguelike Structure (Week 7)

### Sprint 4.1: Permadeath & Run Loop
- Game over state
- Run summary screen (wave reached, enemies killed, resources collected)
- Restart flow (clear entities, reset state)
- **Deliverable**: Death → summary → restart seamlessly

### Sprint 4.2: Meta-Progression
- Meta-currency drop on death (based on performance)
- Persistent unlock system (new turret types, starting bonuses)
- Simple unlock UI (spend meta-currency between runs)
- Save/load system for meta-progression
- **Deliverable**: Each run unlocks something for next run

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
- More turret types (AOE, slow, chain lightning)
- More enemy types (fast, tanky, ranged)
- Boss waves every 5 waves
- Map obstacles/terrain
- Player abilities (dash, temporary shield)
- Leaderboards/stats tracking

---

**Estimated Timeline: 8 weeks part-time (2-3 hours/day)**
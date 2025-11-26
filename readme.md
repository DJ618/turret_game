# Turret Game

A 2D top-down roguelike wave survival game where you **hunt fleeing enemies** while **avoiding pursuing threats**. Build turrets, collect resources, and survive increasingly difficult waves in this dual-threat gameplay experience.

---

## 🎮 Two Versions Available

This game is available in **two parallel implementations** with feature parity:

### 🖥️ .NET Desktop Version
High-performance desktop application built with **MonoGame** and **.NET 8.0**
- Native performance
- Clean Architecture with dependency injection
- Fullscreen gameplay
- Configuration hot-reload support

**[→ .NET Installation & Play Instructions](DOTNET_SETUP.md)**

### 🌐 HTML5 Browser Version
Instant-play web version built with **HTML5 Canvas** and vanilla JavaScript
- No installation required
- Play directly in your browser
- Cross-platform (Windows, Mac, Linux)
- Portable and easy to share

**[→ HTML5 Installation & Play Instructions](HTML_SETUP.md)**

---

## 🎯 Game Overview

### Core Gameplay Loop
1. **Hunt** fleeing prey enemies (green) for resources
2. **Survive** pursuing hunter enemies (red)
3. **Build** defensive turrets during upgrade phases
4. **Upgrade** your capabilities between waves
5. **Repeat** as difficulty escalates

### Key Features
- ✅ **Dual-threat AI system**: Hunt prey while surviving hunters
- ✅ **Wave-based progression**: Fibonacci scaling for prey, linear for hunters
- ✅ **Strategic turret placement**: Max 15 turrets, auto-targeting
- ✅ **Upgrade system**: 5 unique upgrades with selection limits
- ✅ **Progressive difficulty**: 1% speed increase per wave (capped at 90% player speed)
- ✅ **Smooth AI behavior**: Frame-optimized movement with edge detection
- ✅ **Real-time statistics**: Track resources, waves, enemies, and turrets

---

## 🎮 Controls

| Input | Action |
|-------|--------|
| **WASD** | Move player (cyan circle) |
| **Mouse Click** | Place turret during build phase |
| **H** | Toggle instructions (HTML version) |
| **ESC** | Quit game (.NET version) |

---

## 🎨 Gameplay Elements

### Player
- **Cyan circle** - You control this with WASD keys
- Hunt prey, avoid hunters, collect resources

### Enemies
- **Green circles (Prey)** - Flee from you, must be hunted down
- **Red circles (Hunters)** - Chase you, must be avoided
- Progressive AI: smooth movement, edge avoidance, wander behavior

### Turrets
- **Black squares** - Auto-targeting defensive structures
- Max 15 turrets on board
- Place strategically during upgrade phases

### Resources
- **Gold coins** - Animated pickups from killed enemies
- Collect to unlock upgrades between waves

---

## 🔧 Upgrades

Choose **2 random upgrades** between each wave:

| Upgrade | Effect | Limit |
|---------|--------|-------|
| **Extra Turret** | Place +1 additional turret per round | 2 selections max (→ 3 turrets/round) |
| **Faster Projectiles** | +10% projectile velocity | Unlimited |
| **Better Resources** | +1 resource value per pickup | Unlimited |
| **Slow Chase Enemies** | -5% hunter speed | Unlimited |
| **Slow Fleeing Enemies** | -5% prey speed | Unlimited |

---

## 📈 Difficulty Progression

- **Prey count**: Fibonacci sequence (1, 1, 2, 3, 5, 8, 13...)
- **Hunter count**: Linear scaling (wave × 2 per wave)
- **Enemy speed**: +1% per wave, capped at 90% player speed
- **Game over**: When any hunter catches you

---

## 🏗️ Project Structure

### .NET Version
```
TurretGame.Core/          # Domain entities, interfaces (framework-agnostic)
TurretGame.Infrastructure/ # MonoGame rendering, input, graphics
TurretGame.Application/    # Game systems, managers, business logic
TurretGame.Game/          # Entry point, dependency injection
```

### HTML5 Version
```
html/
├── index.html      # Main game page with instructions
├── game.js         # Core game loop, systems, rendering
├── entities.js     # Player, Enemy, Turret, Projectile classes
└── upgrades.js     # Upgrade definitions and management
```

---

## 🚀 Quick Start

### Play Immediately (HTML5)
```bash
# Clone the repo
git clone https://github.com/DJ618/turret_game.git
cd turret_game/html

# Open in browser
start index.html  # Windows
open index.html   # macOS
xdg-open index.html  # Linux
```

### Build & Run (.NET)
```bash
# Clone the repo
git clone https://github.com/DJ618/turret_game.git
cd turret_game

# Build and run
dotnet build
cd TurretGame.Game
dotnet run
```

---

## 📊 Technical Highlights

### Performance Optimizations
- **Frame-based calculations**: Direction updates every 10 frames (both versions)
- **Smooth interpolation**: Lerp-based direction changes prevent jerky movement
- **Edge detection**: Unstuck behavior after 3 seconds near edges
- **Efficient rendering**: Minimal draw calls, optimized collision detection

### Architecture
- **.NET Version**: Clean Architecture, Dependency Injection, Options Pattern
- **HTML5 Version**: Class-based ES6, modular design, canvas optimization

### Game Balance
- **80/20 ratio**: ~80% prey (hunting challenge) + ~20% hunters (survival pressure)
- **Speed cap**: Enemies never exceed 90% player speed (remains fair)
- **Turret limit**: Max 15 prevents spam strategies
- **Upgrade limits**: Extra Turret capped to prevent imbalance

---

## 🛠️ Development

Both versions are actively maintained with feature parity.

### Contributing
1. Fork the repository
2. Create a feature branch
3. Make changes to **both versions** if applicable
4. Test thoroughly
5. Submit a pull request

### Design Philosophy
See [CLAUDE.md](CLAUDE.md) for detailed architecture and design decisions.

---

## 📚 Documentation

- **[.NET Setup Guide](DOTNET_SETUP.md)** - Installation, configuration, troubleshooting
- **[HTML5 Setup Guide](HTML_SETUP.md)** - Browser compatibility, hosting, development
- **[Development Roadmap](ROADMAP.md)** - MVP phases, planned features, timeline
- **[Project Definition](CLAUDE.md)** - Architecture, design principles, decisions

---

## 🎯 Game Design Philosophy

### Core Dynamic
**HUNT the many (prey) while SURVIVING the few (hunters)**

This dual-threat system creates unique tension:
- Offensive positioning required to catch fleeing prey
- Defensive awareness required to avoid pursuing hunters
- Risk/reward decisions: chase prey into danger zones?
- Turret placement strategy: coverage vs. choke points

### Psychological Hooks
1. **Immediate feedback** - Action → result in <1 second
2. **Progressive mastery** - Skill ceiling with visible improvement
3. **Variable rewards** - Random upgrade options create variety
4. **"One more round"** - Sessions designed for 3-5 minute runs
5. **Compound progression** - Upgrades stack exponentially

---

## 🏆 Current Status

**Phase**: Early development (Sprints 1.1-2.3 complete)

**Implemented Features:**
- ✅ Player movement (WASD)
- ✅ Dual-threat AI (hunters chase, prey flee)
- ✅ Collision detection and handling
- ✅ Resource collection system
- ✅ Wave-based spawning (Fibonacci + linear)
- ✅ Turret placement and combat
- ✅ Projectile system
- ✅ Upgrade system (5 upgrades)
- ✅ Progressive difficulty
- ✅ Smooth AI with optimizations
- ✅ Real-time UI (counters, statistics)
- ✅ Game over and restart

**Next Steps:**
See [ROADMAP.md](ROADMAP.md) for upcoming features and timeline.

---

## 📄 License

MIT License - See [LICENSE](LICENSE) file for details

---

## 🔗 Links

- **[Full Roadmap](ROADMAP.md)** - Detailed development plan and future features
- **[Repository](https://github.com/DJ618/turret_game)** - Source code and issues
- **Game Design Document** - [CLAUDE.md](CLAUDE.md)

---

**Made with MonoGame and HTML5 Canvas** | Two versions, one addictive gameplay loop

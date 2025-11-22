# Turret Game - HTML5 Browser Version

This is a browser-based HTML5/JavaScript port of the Turret Game, featuring wave-based roguelike gameplay with a dual-threat enemy system, turret placement, and resource collection.

## How to Play

### Running the Game
Simply open `index.html` in any modern web browser (Chrome, Firefox, Safari, Edge).

You can:
1. Double-click `index.html` to open it in your default browser
2. Right-click `index.html` and choose "Open with" to select a specific browser
3. Drag and drop `index.html` into an open browser window

### Controls
- **W A S D** - Move the player (cyan circle)
- **Mouse Click** - Place turret (during turret placement phase)
- **Continue Button** - Advance to next wave after placing turret
- **Refresh** - Restart game after game over

### Gameplay

#### Wave System
The game progresses through waves with increasing difficulty:
- **Wave 1** starts immediately with 1 prey and 1 hunter
- **Prey count** follows Fibonacci sequence: 1, 1, 2, 3, 5, 8, 13...
- **Hunter count** increases linearly: wave × 1

#### Enemy Types
- **Green Enemy (Prey)**: Flees to the farthest corner from you. Chase them down to eliminate them. Drops resources when killed.
- **Red Enemy (Hunter)**: Chases you relentlessly. Touching them ends the game immediately!

#### Turret System
- After completing wave 1, you can place **1 turret per wave**
- Turrets are **black squares** that auto-shoot white projectiles every 2 seconds
- Turrets target the nearest enemy (hunter or prey)
- Place turrets strategically to cover the map and maximize kills

#### Resource Collection
- Killing enemies (via collision or turret projectiles) spawns **gold coins**
- Collect coins by running over them
- Resources contribute to your final score

#### Scoring
Your score is calculated on game over:
```
Score = (Highest Wave × Resources Collected) + Enemies Killed
```

### Objective
Survive as many waves as possible by:
1. **Hunting** fleeing prey enemies (green) to collect resources
2. **Avoiding** pursuing hunter enemies (red) to stay alive
3. **Placing turrets** strategically to control the battlefield
4. **Maximizing** your score through resource collection and enemy kills

## Technical Details

### Files
- `index.html` - Main HTML page with canvas element, styling, and controls info
- `game.js` - Core game loop, state management, wave system, and rendering
- `entities.js` - Player, Enemy, Turret, Projectile, and ResourcePickup classes

### Features Ported from C#/MonoGame Version

#### Core Gameplay
- Player WASD movement (400 pixels/second) with boundary clamping
- Hunter enemy with chase AI (180 pixels/second, moves toward player)
- Prey enemy with intelligent fleeing AI (320 pixels/second, moves to farthest corner)
- Circle-to-circle collision detection
- Game over state on hunter collision
- Fullscreen canvas that adapts to window size
- 60 FPS game loop using requestAnimationFrame

#### Wave System
- Fibonacci sequence for prey spawning (1, 1, 2, 3, 5, 8, 13...)
- Linear hunter progression (wave × huntersPerWave)
- Wave completion detection (all enemies eliminated)
- Automatic wave transitions

#### Turret Mechanics
- Turret placement system (1 per wave from wave 2+)
- Auto-targeting nearest enemy
- Shooting cooldown (2 seconds)
- Projectile physics with velocity-based movement
- Projectile-enemy collision detection

#### Resource & Scoring
- Resource pickup spawning on enemy death (random offset, clamped to bounds)
- Player-pickup collision detection
- Resource counter UI
- Statistics tracking (enemies killed, resources collected)
- Score calculation: `(level × resources) + enemies killed`

#### UI/UX
- In-game resource and wave counter (top-left)
- Turret placement overlay with mouse preview
- Continue button (only enabled after placing turret)
- Game over screen with score breakdown
- Visual feedback for all game states

#### Configuration
Configurable game settings via `CONFIG` object in `game.js`:
- `playerSpeed`: 400
- `hunterSpeed`: 180
- `preySpeed`: 320
- `huntersPerWave`: 1

### Browser Compatibility
Works in all modern browsers supporting:
- HTML5 Canvas
- ES6 JavaScript (classes, arrow functions, template literals)
- requestAnimationFrame

Tested in Chrome, Firefox, Safari, and Edge.

## Differences from MonoGame Version

### Visual
- Uses HTML5 Canvas instead of MonoGame SpriteBatch
- No animated coin sprites (static gold circles with orange border)
- Simplified text rendering (Canvas 2D text instead of SpriteFont)
- No sprite sheets or texture atlases

### Configuration
- Settings in `CONFIG` object instead of `appsettings.json`
- No hot-reload (requires page refresh to apply changes)

### Architecture
- Single-file systems instead of layered architecture
- No dependency injection container
- Direct object instantiation instead of service resolution

### Behavior Parity
The HTML version matches the .NET version's gameplay exactly:
- Same wave progression (Fibonacci prey, linear hunters)
- Same movement speeds and collision radii
- Same turret placement rules (1 per wave from wave 2+)
- Same shooting cooldown (2 seconds)
- Same score formula
- Same game flow (wave → turret placement → wave → ...)

## Development
Built with vanilla JavaScript - no frameworks or libraries required. All game logic ported directly from the C# codebase to maintain behavior parity with the MonoGame version.

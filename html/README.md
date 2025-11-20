# Turret Game - HTML5 Browser Version

This is a browser-based HTML5/JavaScript port of the Turret Game, featuring the dual-threat enemy system.

## How to Play

### Running the Game
Simply open `index.html` in any modern web browser (Chrome, Firefox, Safari, Edge).

You can:
1. Double-click `index.html` to open it in your default browser
2. Right-click `index.html` and choose "Open with" to select a specific browser
3. Drag and drop `index.html` into an open browser window

### Controls
- **W A S D** - Move the player (cyan circle)
- **ESC** - Close the browser tab to exit

### Gameplay
- **Green Enemy (Prey)**: Flees to the farthest corner from you. Chase it down and touch it to make it disappear. Safe to catch!
- **Red Enemy (Hunter)**: Chases you relentlessly. Avoid it at all costs - touching it ends the game!

### Objective
Catch the fleeing green enemy while avoiding the pursuing red enemy. The dual-threat system creates tension between offensive hunting and defensive survival.

## Technical Details

### Files
- `index.html` - Main HTML page with canvas element and styling
- `game.js` - Core game loop, state management, and rendering
- `entities.js` - Player and Enemy classes with movement logic

### Features Ported from C#/MonoGame Version
- Player WASD movement with boundary clamping
- Hunter enemy with chase AI (moves toward player)
- Prey enemy with intelligent fleeing AI (moves to farthest corner)
- Circle-to-circle collision detection
- Game over state on hunter collision
- Fullscreen canvas that adapts to window size
- 60 FPS game loop using requestAnimationFrame

### Browser Compatibility
Works in all modern browsers supporting:
- HTML5 Canvas
- ES6 JavaScript (classes, arrow functions)
- requestAnimationFrame

Tested in Chrome, Firefox, Safari, and Edge.

## Differences from MonoGame Version
- No fullscreen mode (browser limitation)
- Uses HTML5 Canvas instead of MonoGame rendering
- Simplified visual indicators (no font rendering for game over text, uses canvas text)
- Refresh page to restart (no built-in restart mechanism)

## Development
Built with vanilla JavaScript - no frameworks or libraries required. All game logic ported directly from the C# codebase.

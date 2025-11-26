# HTML5 Version - Installation & Play Instructions

## Prerequisites

### Required Software
- **Modern Web Browser** (Chrome, Firefox, Edge, or Safari)
  - JavaScript enabled
  - HTML5 Canvas support
- **Optional**: Local web server for development

---

## Installation

### Quick Start (No Installation Required)

#### Option 1: Open Directly in Browser
1. Navigate to the `html/` folder
2. Double-click `index.html`
3. Game launches in your default browser

#### Option 2: Clone Repository
```bash
git clone https://github.com/DJ618/turret_game.git
cd turret_game/html
```
Then open `index.html` in your browser.

---

## Running the Game

### Desktop
Simply open `html/index.html` in any modern browser.

### Local Web Server (Recommended for Development)
Using Python 3:
```bash
cd html
python -m http.server 8000
```
Then navigate to `http://localhost:8000` in your browser.

Using Node.js (with `http-server`):
```bash
cd html
npx http-server -p 8000
```

### Live Server (VS Code Extension)
1. Install "Live Server" extension in VS Code
2. Right-click `index.html`
3. Select "Open with Live Server"

---

## Game Controls

| Control | Action |
|---------|--------|
| **WASD** | Move player (cyan circle) |
| **Mouse Click** | Place turret during build phase |
| **H** | Toggle instructions panel |

---

## Gameplay Overview

### Objective
Survive waves of enemies by hunting prey while avoiding hunters. Collect resources to upgrade and place turrets.

### Enemies
- **Prey (Green)**: Flee from you - hunt them down for resources
- **Hunter (Red)**: Chase you - avoid them or die

### Resources
- **Gold Coins**: Dropped by killed enemies - walk over to collect
- Use resources to select upgrades between waves

### Upgrades (Between Waves)
- **Extra Turret**: Place additional turrets (+2 max)
- **Faster Projectiles**: Increase turret projectile speed (+10%)
- **Better Resources**: Increase resource value per pickup (+1)
- **Slow Chase Enemies**: Reduce hunter speed (-5%)
- **Slow Fleeing Enemies**: Reduce prey speed (-5%)

### Turrets
- **Placement**: During build phase, click to place turrets (max 15 total)
- **Auto-targeting**: Turrets automatically shoot nearest enemy
- **Strategy**: Position turrets to cover wide areas and choke points

### Progression
- Each wave increases enemy count (Fibonacci sequence for prey)
- Enemy speed increases 1% per wave (capped at 90% of player speed)
- Hunters increase linearly with wave number
- Game over when a hunter catches you

---

## Project Structure

```
html/
├── index.html      # Main game page
├── game.js         # Core game loop and systems
├── entities.js     # Player, Enemy, Turret, Projectile classes
├── upgrades.js     # Upgrade system definitions
└── styles.css      # Basic styling
```

---

## Configuration

Edit `game.js` to adjust game settings:

```javascript
const CONFIG = {
  playerSpeed: 300,
  preySpeed: 140,
  hunterSpeed: 200,
  huntersPerWave: 2,
  maxTurrets: 15,
  // ... more settings
};
```

Refresh the browser to apply changes.

---

## Browser Compatibility

### Fully Supported
- ✅ **Chrome** 90+ (recommended)
- ✅ **Firefox** 88+
- ✅ **Edge** 90+
- ✅ **Safari** 14+

### Required Features
- HTML5 Canvas API
- ES6 JavaScript (Classes, Arrow Functions, `let`/`const`)
- RequestAnimationFrame API

---

## Troubleshooting

### Game Won't Start
1. Check browser console for errors (F12 → Console tab)
2. Ensure JavaScript is enabled
3. Try a different browser

### Performance Issues
- Close other browser tabs
- Disable browser extensions
- Lower screen resolution
- Use hardware acceleration (check browser settings)

### Graphics Glitches
- Update your browser to the latest version
- Check GPU drivers are up to date
- Try disabling hardware acceleration if issues persist

### File Access Errors (CORS)
If loading from `file://` protocol shows errors:
- Use a local web server instead (see "Local Web Server" section above)
- Or adjust browser security settings (not recommended)

---

## Performance Optimization

The HTML5 version includes several optimizations:
- **Frame-based calculations**: Enemy directions recalculated every 10 frames
- **Smooth movement**: Direction interpolation prevents jerky motion
- **Efficient rendering**: Canvas cleared and redrawn each frame
- **Object pooling**: Minimal garbage collection overhead

Expected performance:
- **60 FPS** on modern hardware
- **30-60 FPS** on older laptops
- Scales automatically with browser window size

---

## Development

### Modifying the Game
1. Open files in any text editor
2. Make changes to `.js` or `.html` files
3. Refresh browser to see changes

### Debugging
- Press `F12` to open browser DevTools
- Use `console.log()` for debugging
- Monitor frame rate in Performance tab

### Testing
Test in multiple browsers to ensure compatibility:
```bash
# Test in different browsers
start chrome html/index.html
start firefox html/index.html
start msedge html/index.html
```

---

## Mobile Support (Experimental)

The HTML5 version is **not optimized for mobile** but may work on tablets:
- Touch controls not implemented
- Performance varies by device
- Landscape orientation recommended

Future enhancements may include:
- Virtual joystick for movement
- Touch-to-place turrets
- Responsive UI scaling

---

## System Requirements

### Minimum
- **Browser**: Any modern browser (2020+)
- **RAM**: 512 MB available
- **Screen**: 1024x768 minimum resolution
- **Internet**: Not required (runs offline)

### Recommended
- **Browser**: Chrome 120+ or Firefox 120+
- **RAM**: 2 GB available
- **Screen**: 1920x1080 or higher
- **GPU**: Hardware acceleration enabled

---

## Features Unique to HTML5 Version

- **Instructions Panel**: Toggle with `H` key (includes SVG graphics)
- **Browser-based**: No installation required
- **Instant Play**: Open and play immediately
- **Cross-platform**: Works on Windows, Mac, Linux
- **Portable**: Can be hosted on any web server

---

## Hosting Online

To deploy the game online:

### GitHub Pages
1. Push `html/` folder to GitHub repository
2. Enable GitHub Pages in repository settings
3. Set source to `main` branch, `/html` folder
4. Access at `https://yourusername.github.io/turret_game/`

### Netlify
```bash
cd html
netlify deploy --prod
```

### Any Static Host
Upload the `html/` folder contents to:
- Vercel, Cloudflare Pages, Firebase Hosting, etc.
- Any web server with static file support

---

## Known Issues

- Fullscreen mode not implemented (runs in browser window)
- Audio not implemented yet
- Save/load not implemented (sessions are temporary)
- Mobile controls not available

See [ROADMAP.md](ROADMAP.md) for planned features.

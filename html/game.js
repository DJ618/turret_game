// Game State
const GameState = {
    PLAYING: 'playing',
    PLACING_TURRET: 'placing_turret',
    GAME_OVER: 'gameover'
};

// Configuration
const CONFIG = {
    playerSpeed: 400,
    hunterSpeed: 180,
    preySpeed: 320,
    huntersPerWave: 1
};

// Game class
class Game {
    constructor() {
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');

        // Set canvas size to window size
        this.resizeCanvas();
        window.addEventListener('resize', () => this.resizeCanvas());

        // Game state
        this.state = GameState.PLAYING;
        this.keys = {};
        this.mouseX = 0;
        this.mouseY = 0;
        this.previousMouseDown = false;

        // Wave management
        this.currentWave = 0;
        this.previousFibonacci = 1;
        this.currentFibonacci = 1;

        // Resource management
        this.resourceCount = 0;
        this.enemiesKilled = 0;

        // Turret placement
        this.turretPlacedThisRound = false;

        // Initialize entities
        this.player = new Player(this.canvas.width / 2, this.canvas.height / 2);
        this.player.speed = CONFIG.playerSpeed;
        this.enemies = [];
        this.turrets = [];
        this.projectiles = [];
        this.pickups = [];

        // Input handling
        this.setupInput();

        // Timing
        this.lastTime = performance.now();

        // Start first wave immediately (wave 1)
        this.startNextWave();

        // Start game loop
        this.gameLoop();
    }

    resizeCanvas() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }

    setupInput() {
        window.addEventListener('keydown', (e) => {
            this.keys[e.key] = true;
        });

        window.addEventListener('keyup', (e) => {
            this.keys[e.key] = false;
        });

        window.addEventListener('mousemove', (e) => {
            const rect = this.canvas.getBoundingClientRect();
            this.mouseX = e.clientX - rect.left;
            this.mouseY = e.clientY - rect.top;
        });

        window.addEventListener('mousedown', (e) => {
            if (e.button === 0) { // Left click
                this.handleMouseClick();
            }
        });
    }

    handleMouseClick() {
        if (this.state === GameState.PLACING_TURRET) {
            // Check if clicking continue button
            if (this.isContinueButtonClicked(this.mouseX, this.mouseY)) {
                if (this.turretPlacedThisRound) {
                    this.continueToNextWave();
                }
            }
            // Otherwise, place turret
            else if (!this.turretPlacedThisRound) {
                this.placeTurret(this.mouseX, this.mouseY);
            }
        }
        else if (this.state === GameState.GAME_OVER) {
            // Check if clicking refresh button
            if (this.isRefreshButtonClicked(this.mouseX, this.mouseY)) {
                location.reload();
            }
        }
    }

    startNextWave() {
        this.currentWave++;

        // Calculate enemy counts
        const preyCount = this.currentFibonacci;
        const hunterCount = this.currentWave * CONFIG.huntersPerWave;

        // Update Fibonacci for next wave
        const nextFib = this.previousFibonacci + this.currentFibonacci;
        this.previousFibonacci = this.currentFibonacci;
        this.currentFibonacci = nextFib;

        // Spawn prey enemies
        for (let i = 0; i < preyCount; i++) {
            const pos = this.getRandomEdgePosition();
            const prey = new Enemy(pos.x, pos.y, EnemyType.PREY, CONFIG.preySpeed);
            this.enemies.push(prey);
        }

        // Spawn hunter enemies (starting from wave 2)
        for (let i = 0; i < hunterCount; i++) {
            const pos = this.getRandomEdgePosition(this.player.x, this.player.y);
            const hunter = new Enemy(pos.x, pos.y, EnemyType.HUNTER, CONFIG.hunterSpeed);
            this.enemies.push(hunter);
        }

        this.state = GameState.PLAYING;
    }

    getRandomEdgePosition(avoidX = null, avoidY = null) {
        const edge = Math.floor(Math.random() * 4); // 0=top, 1=right, 2=bottom, 3=left
        const minDistance = 200; // Minimum distance from player for hunters

        let x, y;
        let attempts = 0;
        do {
            switch (edge) {
                case 0: // Top
                    x = Math.random() * this.canvas.width;
                    y = 0;
                    break;
                case 1: // Right
                    x = this.canvas.width;
                    y = Math.random() * this.canvas.height;
                    break;
                case 2: // Bottom
                    x = Math.random() * this.canvas.width;
                    y = this.canvas.height;
                    break;
                case 3: // Left
                    x = 0;
                    y = Math.random() * this.canvas.height;
                    break;
            }
            attempts++;
        } while (avoidX !== null && avoidY !== null &&
                 this.distance(x, y, avoidX, avoidY) < minDistance &&
                 attempts < 10);

        return { x, y };
    }

    distance(x1, y1, x2, y2) {
        const dx = x2 - x1;
        const dy = y2 - y1;
        return Math.sqrt(dx * dx + dy * dy);
    }

    placeTurret(x, y) {
        if (this.state !== GameState.PLACING_TURRET || this.turretPlacedThisRound) {
            return;
        }

        const turret = new Turret(x, y);
        this.turrets.push(turret);
        this.turretPlacedThisRound = true;
    }

    continueToNextWave() {
        this.startNextWave();
    }

    shouldStartNextWave() {
        // Start next wave if all PREY enemies are dead (hunters don't block progression)
        for (const enemy of this.enemies) {
            if (enemy.type === EnemyType.PREY) {
                return false; // Still have prey alive
            }
        }
        return true; // All prey are dead (hunters don't matter)
    }

    isContinueButtonClicked(x, y) {
        const buttonRect = this.getContinueButtonRect();
        return x >= buttonRect.x && x <= buttonRect.x + buttonRect.width &&
               y >= buttonRect.y && y <= buttonRect.y + buttonRect.height;
    }

    getContinueButtonRect() {
        const buttonWidth = 300;
        const buttonHeight = 80;
        return {
            x: this.canvas.width / 2 - buttonWidth / 2,
            y: this.canvas.height - 200,
            width: buttonWidth,
            height: buttonHeight
        };
    }

    isRefreshButtonClicked(x, y) {
        const buttonRect = this.getRefreshButtonRect();
        return x >= buttonRect.x && x <= buttonRect.x + buttonRect.width &&
               y >= buttonRect.y && y <= buttonRect.y + buttonRect.height;
    }

    getRefreshButtonRect() {
        const buttonWidth = 300;
        const buttonHeight = 80;
        return {
            x: this.canvas.width / 2 - buttonWidth / 2,
            y: this.canvas.height / 2 + 220,
            width: buttonWidth,
            height: buttonHeight
        };
    }

    spawnResourcePickup(x, y) {
        // Random offset for pickup spawn (80-150 pixels)
        const angle = Math.random() * Math.PI * 2;
        const distance = 80 + Math.random() * 70;
        let pickupX = x + Math.cos(angle) * distance;
        let pickupY = y + Math.sin(angle) * distance;

        // Clamp to bounds
        const pickupRadius = 8;
        pickupX = Math.max(pickupRadius, Math.min(this.canvas.width - pickupRadius, pickupX));
        pickupY = Math.max(pickupRadius, Math.min(this.canvas.height - pickupRadius, pickupY));

        const pickup = new ResourcePickup(pickupX, pickupY, 1);
        this.pickups.push(pickup);
    }

    calculateScore() {
        return (this.currentWave * this.resourceCount) + this.enemiesKilled;
    }

    update(deltaTime) {
        if (this.state === GameState.GAME_OVER || this.state === GameState.PLACING_TURRET) {
            return; // Don't update if game is over or placing turret
        }

        // Clean up dead entities first (so wave completion check works correctly)
        this.enemies = this.enemies.filter(e => e.isAlive);
        this.projectiles = this.projectiles.filter(p => p.isActive);
        this.pickups = this.pickups.filter(p => !p.isCollected);

        // Handle wave transitions
        if (this.shouldStartNextWave()) {
            // Wave 1 just completed, now allow turret placement for wave 2+
            if (this.currentWave >= 1) {
                this.state = GameState.PLACING_TURRET;
                this.turretPlacedThisRound = false;
                return;
            }
        }

        // Update player
        this.player.update(
            this.keys,
            deltaTime,
            0,
            this.canvas.width,
            0,
            this.canvas.height
        );

        // Update enemies
        for (const enemy of this.enemies) {
            enemy.update(
                this.player.x,
                this.player.y,
                deltaTime,
                0,
                this.canvas.width,
                0,
                this.canvas.height
            );

            // Check player-enemy collision
            if (enemy.isAlive && circleToCircle(
                this.player.x, this.player.y, this.player.radius,
                enemy.x, enemy.y, enemy.radius
            )) {
                if (enemy.type === EnemyType.HUNTER) {
                    // Collision with hunter = game over
                    this.state = GameState.GAME_OVER;
                } else {
                    // Collision with prey = collect resource
                    enemy.isAlive = false;
                    this.enemiesKilled++;
                    this.spawnResourcePickup(enemy.x, enemy.y);
                }
            }
        }

        // Check enemy-enemy collisions (prevent overlapping beyond half their width)
        for (let i = 0; i < this.enemies.length; i++) {
            for (let j = i + 1; j < this.enemies.length; j++) {
                const enemy1 = this.enemies[i];
                const enemy2 = this.enemies[j];

                const distance = this.distance(enemy1.x, enemy1.y, enemy2.x, enemy2.y);
                const minDistance = (enemy1.radius + enemy2.radius) * 0.5; // Allow overlap of half their combined width

                if (distance < minDistance && distance > 0.001) {
                    // Push enemies apart
                    const dx = enemy2.x - enemy1.x;
                    const dy = enemy2.y - enemy1.y;
                    const dirX = dx / distance;
                    const dirY = dy / distance;
                    const pushAmount = (minDistance - distance) * 0.5;

                    const newPos1X = enemy1.x - dirX * pushAmount;
                    const newPos1Y = enemy1.y - dirY * pushAmount;
                    const newPos2X = enemy2.x + dirX * pushAmount;
                    const newPos2Y = enemy2.y + dirY * pushAmount;

                    // Clamp positions to bounds
                    enemy1.x = Math.max(enemy1.radius, Math.min(this.canvas.width - enemy1.radius, newPos1X));
                    enemy1.y = Math.max(enemy1.radius, Math.min(this.canvas.height - enemy1.radius, newPos1Y));
                    enemy2.x = Math.max(enemy2.radius, Math.min(this.canvas.width - enemy2.radius, newPos2X));
                    enemy2.y = Math.max(enemy2.radius, Math.min(this.canvas.height - enemy2.radius, newPos2Y));
                }
            }
        }

        // Update turrets
        for (const turret of this.turrets) {
            turret.update(deltaTime);

            if (turret.canShoot()) {
                // Find nearest alive enemy
                let nearestEnemy = null;
                let minDistance = Infinity;

                for (const enemy of this.enemies) {
                    if (enemy.isAlive) {
                        const dist = this.distance(turret.x, turret.y, enemy.x, enemy.y);
                        if (dist < minDistance) {
                            minDistance = dist;
                            nearestEnemy = enemy;
                        }
                    }
                }

                if (nearestEnemy) {
                    const projectile = new Projectile(turret.x, turret.y, nearestEnemy.x, nearestEnemy.y);
                    this.projectiles.push(projectile);
                    turret.resetShootCooldown();
                }
            }
        }

        // Update projectiles
        for (const projectile of this.projectiles) {
            projectile.update(deltaTime, 0, this.canvas.width, 0, this.canvas.height);

            // Check projectile-enemy collision
            if (projectile.isActive) {
                for (const enemy of this.enemies) {
                    if (enemy.isAlive && circleToCircle(
                        projectile.x, projectile.y, projectile.radius,
                        enemy.x, enemy.y, enemy.radius
                    )) {
                        enemy.isAlive = false;
                        projectile.isActive = false;
                        this.enemiesKilled++;
                        this.spawnResourcePickup(enemy.x, enemy.y);
                        break;
                    }
                }
            }
        }

        // Check player-pickup collision
        for (const pickup of this.pickups) {
            if (!pickup.isCollected && circleToCircle(
                this.player.x, this.player.y, this.player.radius,
                pickup.x, pickup.y, pickup.radius
            )) {
                pickup.isCollected = true;
                this.resourceCount += pickup.value;
            }
        }
    }

    draw() {
        // Clear canvas
        this.ctx.fillStyle = '#6495ED'; // CornflowerBlue
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Draw player
        this.player.draw(this.ctx);

        // Draw turrets
        for (const turret of this.turrets) {
            turret.draw(this.ctx);
        }

        // Draw enemies
        for (const enemy of this.enemies) {
            enemy.draw(this.ctx);
        }

        // Draw projectiles
        for (const projectile of this.projectiles) {
            projectile.draw(this.ctx);
        }

        // Draw pickups
        for (const pickup of this.pickups) {
            pickup.draw(this.ctx);
        }

        // Draw UI - Resource counter and wave number
        this.ctx.fillStyle = '#FFD700'; // Gold
        this.ctx.font = 'bold 32px Arial';
        this.ctx.textAlign = 'left';
        this.ctx.textBaseline = 'top';
        this.ctx.fillText(`Resources: ${this.resourceCount}`, 20, 20);

        this.ctx.fillStyle = 'white';
        this.ctx.fillText(`Wave: ${this.currentWave}`, 20, 80);

        // Draw turret placement overlay
        if (this.state === GameState.PLACING_TURRET) {
            this.drawTurretPlacement();
        }

        // Draw game over overlay
        if (this.state === GameState.GAME_OVER) {
            this.drawGameOver();
        }
    }

    drawTurretPlacement() {
        // Semi-transparent black overlay
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Title text
        this.ctx.fillStyle = '#00FF00'; // Green
        this.ctx.font = 'bold 48px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(`Wave ${this.currentWave} Complete!`, this.canvas.width / 2, this.canvas.height / 2 - 150);

        // Instruction text
        const instructionText = this.turretPlacedThisRound ? 'Turret placed! Click CONTINUE' : 'Click to place a turret';
        this.ctx.fillStyle = this.turretPlacedThisRound ? '#FFFF00' : 'white'; // Yellow if placed
        this.ctx.font = 'bold 32px Arial';
        this.ctx.fillText(instructionText, this.canvas.width / 2, this.canvas.height / 2 - 70);

        // Draw turret preview at mouse position (only if not placed yet)
        if (!this.turretPlacedThisRound) {
            const turretSize = 30;
            this.ctx.fillStyle = 'rgba(0, 0, 0, 0.6)';
            this.ctx.fillRect(
                this.mouseX - turretSize,
                this.mouseY - turretSize,
                turretSize * 2,
                turretSize * 2
            );
        }

        // Draw continue button
        this.drawContinueButton();
    }

    drawContinueButton() {
        const buttonRect = this.getContinueButtonRect();

        // Button background
        this.ctx.fillStyle = 'rgba(100, 100, 100, 0.8)';
        this.ctx.fillRect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height);

        // Button text
        this.ctx.fillStyle = 'black';
        this.ctx.font = 'bold 36px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText('CONTINUE', buttonRect.x + buttonRect.width / 2, buttonRect.y + buttonRect.height / 2);
    }

    drawGameOver() {
        // Semi-transparent red overlay
        this.ctx.fillStyle = 'rgba(255, 0, 0, 0.7)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Game over text
        this.ctx.fillStyle = 'white';
        this.ctx.font = 'bold 72px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText('GAME OVER', this.canvas.width / 2, this.canvas.height / 2 - 150);

        // Score
        const score = this.calculateScore();
        this.ctx.fillStyle = '#00FFFF'; // Cyan
        this.ctx.font = 'bold 48px Arial';
        this.ctx.fillText(`SCORE: ${score}`, this.canvas.width / 2, this.canvas.height / 2 - 60);

        // Statistics
        this.ctx.fillStyle = '#FFFF00'; // Yellow
        this.ctx.font = 'bold 32px Arial';
        this.ctx.fillText(`Level Reached: ${this.currentWave}`, this.canvas.width / 2, this.canvas.height / 2 + 0);

        this.ctx.fillStyle = '#FFD700'; // Gold
        this.ctx.fillText(`Resources Collected: ${this.resourceCount}`, this.canvas.width / 2, this.canvas.height / 2 + 60);

        this.ctx.fillStyle = 'white';
        this.ctx.fillText(`Enemies Killed: ${this.enemiesKilled}`, this.canvas.width / 2, this.canvas.height / 2 + 120);

        // Draw refresh button
        this.drawRefreshButton();
    }

    drawRefreshButton() {
        const buttonRect = this.getRefreshButtonRect();

        // Button background
        this.ctx.fillStyle = 'rgba(100, 100, 100, 0.8)';
        this.ctx.fillRect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height);

        // Button text
        this.ctx.fillStyle = 'white';
        this.ctx.font = 'bold 36px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText('RESTART', buttonRect.x + buttonRect.width / 2, buttonRect.y + buttonRect.height / 2);
    }

    gameLoop() {
        const currentTime = performance.now();
        const deltaTime = (currentTime - this.lastTime) / 1000; // Convert to seconds
        this.lastTime = currentTime;

        this.update(deltaTime);
        this.draw();

        requestAnimationFrame(() => this.gameLoop());
    }
}

// Start the game when page loads
window.addEventListener('load', () => {
    new Game();
});

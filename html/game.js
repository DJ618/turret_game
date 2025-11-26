// Game State
const GameState = {
    PLAYING: 'playing',
    CHOOSING_UPGRADE: 'choosing_upgrade',
    PLACING_TURRET: 'placing_turret',
    GAME_OVER: 'gameover'
};

// Upgrade Definitions
const Upgrades = {
    EXTRA_TURRET: {
        name: 'Extra Turret',
        description: '+1 turret placement per round',
        apply: (state) => {
            state.extraTurretsPerRound++;
        }
    },
    FASTER_PROJECTILES: {
        name: 'Faster Projectiles',
        description: '+25% projectile velocity',
        apply: (state) => {
            state.projectileVelocityMultiplier *= 1.25;
        }
    },
    BETTER_RESOURCES: {
        name: 'Better Resources',
        description: '+1 resource value per pickup',
        apply: (state) => {
            state.resourceValueBonus++;
        }
    }
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

        // Upgrade state
        this.upgradeState = {
            extraTurretsPerRound: 0,
            projectileVelocityMultiplier: 1.0,
            resourceValueBonus: 0
        };
        this.currentUpgradeOptions = [];

        // Turret placement
        this.turretsPlacedThisRound = 0;
        this.turretsAllowedThisRound = 1;

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
        if (this.state === GameState.CHOOSING_UPGRADE) {
            // Check which upgrade was clicked
            for (let i = 0; i < this.currentUpgradeOptions.length; i++) {
                if (this.isUpgradeButtonClicked(this.mouseX, this.mouseY, i)) {
                    this.selectUpgrade(i);
                    break;
                }
            }
        }
        else if (this.state === GameState.PLACING_TURRET) {
            // Check if clicking continue button
            if (this.isContinueButtonClicked(this.mouseX, this.mouseY)) {
                if (this.canPlaceMoreTurrets()) {
                    // Still can place more, do nothing
                } else {
                    this.continueToNextWave();
                }
            }
            // Otherwise, place turret
            else if (this.canPlaceMoreTurrets()) {
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

    generateUpgradeOptions() {
        // Create array of all upgrade types
        const allUpgrades = Object.values(Upgrades);

        // Shuffle and select 3 random upgrades
        const shuffled = allUpgrades.slice();
        for (let i = shuffled.length - 1; i > 0; i--) {
            const j = Math.floor(Math.random() * (i + 1));
            [shuffled[i], shuffled[j]] = [shuffled[j], shuffled[i]];
        }

        this.currentUpgradeOptions = shuffled.slice(0, Math.min(3, shuffled.length));
    }

    selectUpgrade(optionIndex) {
        if (optionIndex < 0 || optionIndex >= this.currentUpgradeOptions.length) {
            return;
        }

        const selectedUpgrade = this.currentUpgradeOptions[optionIndex];
        selectedUpgrade.apply(this.upgradeState);
        this.currentUpgradeOptions = [];

        // Transition to turret placement
        this.state = GameState.PLACING_TURRET;
        this.turretsPlacedThisRound = 0;
        this.turretsAllowedThisRound = 1 + this.upgradeState.extraTurretsPerRound;
    }

    placeTurret(x, y) {
        if (this.state !== GameState.PLACING_TURRET || !this.canPlaceMoreTurrets()) {
            return;
        }

        const turret = new Turret(x, y);
        this.turrets.push(turret);
        this.turretsPlacedThisRound++;
    }

    canPlaceMoreTurrets() {
        return this.turretsPlacedThisRound < this.turretsAllowedThisRound;
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

    isUpgradeButtonClicked(x, y, optionIndex) {
        const buttonRect = this.getUpgradeButtonRect(optionIndex);
        return x >= buttonRect.x && x <= buttonRect.x + buttonRect.width &&
               y >= buttonRect.y && y <= buttonRect.y + buttonRect.height;
    }

    getUpgradeButtonRect(optionIndex) {
        const buttonWidth = 350;
        const buttonHeight = 120;
        const spacing = 40;
        const totalWidth = buttonWidth * 3 + spacing * 2;
        const startX = this.canvas.width / 2 - totalWidth / 2;
        const startY = this.canvas.height / 2 - 50;

        return {
            x: startX + (buttonWidth + spacing) * optionIndex,
            y: startY,
            width: buttonWidth,
            height: buttonHeight
        };
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
        // Try multiple times to find a position that doesn't overlap with turrets
        const pickupRadius = 8;
        let pickupX = x;
        let pickupY = y;
        let foundValidPosition = false;

        for (let attempt = 0; attempt < 10; attempt++) {
            const angle = Math.random() * Math.PI * 2;
            const distance = 80 + Math.random() * 70;
            pickupX = x + Math.cos(angle) * distance;
            pickupY = y + Math.sin(angle) * distance;

            // Clamp to bounds
            pickupX = Math.max(pickupRadius, Math.min(this.canvas.width - pickupRadius, pickupX));
            pickupY = Math.max(pickupRadius, Math.min(this.canvas.height - pickupRadius, pickupY));

            // Check if position overlaps with any turret
            let overlapsWithTurret = false;
            for (const turret of this.turrets) {
                if (circleToSquare(pickupX, pickupY, pickupRadius, turret.x, turret.y, turret.size)) {
                    overlapsWithTurret = true;
                    break;
                }
            }

            if (!overlapsWithTurret) {
                foundValidPosition = true;
                break;
            }
        }

        // If we couldn't find a valid position after 10 attempts, use the enemy's position
        if (!foundValidPosition) {
            pickupX = x;
            pickupY = y;
        }

        const resourceValue = 1 + this.upgradeState.resourceValueBonus;
        const pickup = new ResourcePickup(pickupX, pickupY, resourceValue);
        this.pickups.push(pickup);
    }

    calculateScore() {
        return (this.currentWave * this.resourceCount) + this.enemiesKilled;
    }

    update(deltaTime) {
        if (this.state === GameState.GAME_OVER || this.state === GameState.PLACING_TURRET || this.state === GameState.CHOOSING_UPGRADE) {
            return; // Don't update if game is over, placing turret, or choosing upgrade
        }

        // Clean up dead entities first (so wave completion check works correctly)
        this.enemies = this.enemies.filter(e => e.isAlive);
        this.projectiles = this.projectiles.filter(p => p.isActive);
        this.pickups = this.pickups.filter(p => !p.isCollected);

        // Handle wave transitions
        if (this.shouldStartNextWave()) {
            // Wave 1 starts immediately without upgrades
            if (this.currentWave === 0) {
                // Wave 1 just completed, transition to upgrade selection for wave 2+
                this.state = GameState.CHOOSING_UPGRADE;
                this.generateUpgradeOptions();
                return;
            }
            // After wave 1, show upgrade selection
            else if (this.currentWave >= 1) {
                this.state = GameState.CHOOSING_UPGRADE;
                this.generateUpgradeOptions();
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

        // Check player-turret collisions (player cannot pass through turrets)
        for (const turret of this.turrets) {
            if (circleToSquare(this.player.x, this.player.y, this.player.radius, turret.x, turret.y, turret.size)) {
                // Push player away from turret
                const closestX = Math.max(turret.x - turret.size, Math.min(this.player.x, turret.x + turret.size));
                const closestY = Math.max(turret.y - turret.size, Math.min(this.player.y, turret.y + turret.size));

                const pushDirX = this.player.x - closestX;
                const pushDirY = this.player.y - closestY;
                const pushLength = Math.sqrt(pushDirX * pushDirX + pushDirY * pushDirY);

                if (pushLength > 0) {
                    const normalizedPushX = pushDirX / pushLength;
                    const normalizedPushY = pushDirY / pushLength;
                    this.player.x = closestX + normalizedPushX * this.player.radius;
                    this.player.y = closestY + normalizedPushY * this.player.radius;
                }
            }
        }

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

        // Check hunter-hunter collisions (prevent overlapping beyond 1/4 their width)
        for (let i = 0; i < this.enemies.length; i++) {
            const enemy1 = this.enemies[i];
            if (!enemy1.isAlive || enemy1.type !== EnemyType.HUNTER) continue;

            for (let j = i + 1; j < this.enemies.length; j++) {
                const enemy2 = this.enemies[j];
                if (!enemy2.isAlive || enemy2.type !== EnemyType.HUNTER) continue;

                const distance = this.distance(enemy1.x, enemy1.y, enemy2.x, enemy2.y);
                const minDistance = (enemy1.radius + enemy2.radius) * 0.75; // Allow 1/4 overlap

                if (distance < minDistance && distance > 0) {
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

        // Check enemy-turret collisions (enemies cannot pass through turrets)
        for (const enemy of this.enemies) {
            if (!enemy.isAlive) continue;

            for (const turret of this.turrets) {
                if (circleToSquare(enemy.x, enemy.y, enemy.radius, turret.x, turret.y, turret.size)) {
                    // Push enemy away from turret
                    const closestX = Math.max(turret.x - turret.size, Math.min(enemy.x, turret.x + turret.size));
                    const closestY = Math.max(turret.y - turret.size, Math.min(enemy.y, turret.y + turret.size));

                    const pushDirX = enemy.x - closestX;
                    const pushDirY = enemy.y - closestY;
                    const pushLength = Math.sqrt(pushDirX * pushDirX + pushDirY * pushDirY);

                    if (pushLength > 0) {
                        const normalizedPushX = pushDirX / pushLength;
                        const normalizedPushY = pushDirY / pushLength;
                        const newX = closestX + normalizedPushX * enemy.radius;
                        const newY = closestY + normalizedPushY * enemy.radius;

                        // Clamp to bounds
                        enemy.x = Math.max(enemy.radius, Math.min(this.canvas.width - enemy.radius, newX));
                        enemy.y = Math.max(enemy.radius, Math.min(this.canvas.height - enemy.radius, newY));
                    }
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
                    // Calculate direction to enemy
                    const dirX = nearestEnemy.x - turret.x;
                    const dirY = nearestEnemy.y - turret.y;
                    const dirLength = Math.sqrt(dirX * dirX + dirY * dirY);

                    if (dirLength > 0) {
                        const normalizedDirX = dirX / dirLength;
                        const normalizedDirY = dirY / dirLength;

                        // Spawn projectile outside the turret's bounding box
                        // Use diagonal distance (size * sqrt(2)) plus a small buffer
                        const spawnDistance = turret.size * 1.5; // 1.414 (sqrt(2)) rounded up for safety
                        const spawnX = turret.x + normalizedDirX * spawnDistance;
                        const spawnY = turret.y + normalizedDirY * spawnDistance;

                        const projectile = new Projectile(spawnX, spawnY, nearestEnemy.x, nearestEnemy.y);
                        // Apply velocity upgrade
                        projectile.speed *= this.upgradeState.projectileVelocityMultiplier;
                        projectile.velocityX *= this.upgradeState.projectileVelocityMultiplier;
                        projectile.velocityY *= this.upgradeState.projectileVelocityMultiplier;
                        this.projectiles.push(projectile);
                        turret.resetShootCooldown();
                    }
                }
            }
        }

        // Update projectiles
        for (const projectile of this.projectiles) {
            projectile.update(deltaTime, 0, this.canvas.width, 0, this.canvas.height);

            // Check projectile-turret collision (projectiles are destroyed by turrets)
            if (projectile.isActive) {
                let hitTurret = false;
                for (const turret of this.turrets) {
                    if (circleToSquare(projectile.x, projectile.y, projectile.radius, turret.x, turret.y, turret.size)) {
                        projectile.isActive = false;
                        hitTurret = true;
                        break;
                    }
                }
                if (hitTurret) continue;
            }

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

        // Draw upgrade selection overlay
        if (this.state === GameState.CHOOSING_UPGRADE) {
            this.drawUpgradeSelection();
        }

        // Draw turret placement overlay
        if (this.state === GameState.PLACING_TURRET) {
            this.drawTurretPlacement();
        }

        // Draw game over overlay
        if (this.state === GameState.GAME_OVER) {
            this.drawGameOver();
        }
    }

    drawUpgradeSelection() {
        // Semi-transparent black overlay
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.8)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Title text
        this.ctx.fillStyle = '#00FF00'; // Green
        this.ctx.font = 'bold 56px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(`Wave ${this.currentWave} Complete!`, this.canvas.width / 2, this.canvas.height / 2 - 200);

        // Instruction text
        this.ctx.fillStyle = '#FFFF00'; // Yellow
        this.ctx.font = 'bold 36px Arial';
        this.ctx.fillText('Choose an Upgrade', this.canvas.width / 2, this.canvas.height / 2 - 130);

        // Draw upgrade options
        for (let i = 0; i < this.currentUpgradeOptions.length; i++) {
            this.drawUpgradeButton(i);
        }
    }

    drawUpgradeButton(optionIndex) {
        const upgrade = this.currentUpgradeOptions[optionIndex];
        const buttonRect = this.getUpgradeButtonRect(optionIndex);

        // Check if mouse is hovering
        const isHovering = this.mouseX >= buttonRect.x && this.mouseX <= buttonRect.x + buttonRect.width &&
                          this.mouseY >= buttonRect.y && this.mouseY <= buttonRect.y + buttonRect.height;

        // Button background (brighter if hovering)
        this.ctx.fillStyle = isHovering ? 'rgba(80, 80, 150, 0.9)' : 'rgba(50, 50, 100, 0.9)';
        this.ctx.fillRect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height);

        // Button border
        this.ctx.strokeStyle = isHovering ? '#FFFFFF' : '#AAAAAA';
        this.ctx.lineWidth = 3;
        this.ctx.strokeRect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height);

        // Upgrade name
        this.ctx.fillStyle = '#FFD700'; // Gold
        this.ctx.font = 'bold 28px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(upgrade.name, buttonRect.x + buttonRect.width / 2, buttonRect.y + 40);

        // Upgrade description
        this.ctx.fillStyle = 'white';
        this.ctx.font = '22px Arial';
        this.ctx.fillText(upgrade.description, buttonRect.x + buttonRect.width / 2, buttonRect.y + 80);
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
        this.ctx.fillText('Place Your Turrets', this.canvas.width / 2, this.canvas.height / 2 - 150);

        // Instruction text
        const placedText = `Turrets Placed: ${this.turretsPlacedThisRound} / ${this.turretsAllowedThisRound}`;
        this.ctx.fillStyle = 'white';
        this.ctx.font = 'bold 32px Arial';
        this.ctx.fillText(placedText, this.canvas.width / 2, this.canvas.height / 2 - 90);

        const instructionText = this.canPlaceMoreTurrets() ? 'Click to place a turret' : 'Click CONTINUE to start next wave';
        this.ctx.fillStyle = this.canPlaceMoreTurrets() ? 'white' : '#FFFF00'; // Yellow if done
        this.ctx.font = 'bold 28px Arial';
        this.ctx.fillText(instructionText, this.canvas.width / 2, this.canvas.height / 2 - 40);

        // Draw turret preview at mouse position (only if can still place)
        if (this.canPlaceMoreTurrets()) {
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

// Game State
const GameState = {
    PLAYING: 'playing',
    GAME_OVER: 'gameover'
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

        // Initialize entities
        this.player = new Player(this.canvas.width / 2, this.canvas.height / 2);
        this.enemies = [];

        // Spawn 2 enemies: 1 Hunter (chaser) and 1 Prey (fleer)
        this.spawnEnemies();

        // Input handling
        this.setupInput();

        // Timing
        this.lastTime = performance.now();

        // Start game loop
        this.gameLoop();
    }

    resizeCanvas() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }

    spawnEnemies() {
        // Spawn Hunter at 50% player speed (100)
        const hunterPos = this.getRandomEdgePosition();
        const hunter = new Enemy(hunterPos.x, hunterPos.y, EnemyType.HUNTER, 100);
        this.enemies.push(hunter);

        // Spawn Prey at 80% player speed (160)
        const preyPos = this.getRandomEdgePosition();
        const prey = new Enemy(preyPos.x, preyPos.y, EnemyType.PREY, 160);
        this.enemies.push(prey);
    }

    getRandomEdgePosition() {
        const edge = Math.floor(Math.random() * 4); // 0=top, 1=right, 2=bottom, 3=left

        switch (edge) {
            case 0: // Top
                return { x: Math.random() * this.canvas.width, y: 0 };
            case 1: // Right
                return { x: this.canvas.width, y: Math.random() * this.canvas.height };
            case 2: // Bottom
                return { x: Math.random() * this.canvas.width, y: this.canvas.height };
            case 3: // Left
                return { x: 0, y: Math.random() * this.canvas.height };
            default:
                return { x: this.canvas.width / 2, y: 0 };
        }
    }

    setupInput() {
        window.addEventListener('keydown', (e) => {
            this.keys[e.key] = true;
        });

        window.addEventListener('keyup', (e) => {
            this.keys[e.key] = false;
        });
    }

    update(deltaTime) {
        if (this.state === GameState.GAME_OVER) {
            return; // Don't update if game is over
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

            // Check collision
            if (enemy.isAlive && circleToCircle(
                this.player.x, this.player.y, this.player.radius,
                enemy.x, enemy.y, enemy.radius
            )) {
                if (enemy.type === EnemyType.HUNTER) {
                    // Collision with hunter = game over
                    this.state = GameState.GAME_OVER;
                } else {
                    // Collision with prey = prey disappears
                    enemy.isAlive = false;
                }
            }
        }
    }

    draw() {
        // Clear canvas
        this.ctx.fillStyle = '#6495ED'; // CornflowerBlue
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Draw player
        this.player.draw(this.ctx);

        // Draw enemies
        for (const enemy of this.enemies) {
            enemy.draw(this.ctx);
        }

        // Draw game over overlay
        if (this.state === GameState.GAME_OVER) {
            // Semi-transparent red overlay
            this.ctx.fillStyle = 'rgba(255, 0, 0, 0.7)';
            this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

            // Game over text
            this.ctx.fillStyle = 'white';
            this.ctx.font = 'bold 72px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.textBaseline = 'middle';
            this.ctx.fillText('GAME OVER', this.canvas.width / 2, this.canvas.height / 2);

            this.ctx.font = '24px Arial';
            this.ctx.fillText('Refresh to restart', this.canvas.width / 2, this.canvas.height / 2 + 60);
        }
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

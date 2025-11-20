// Enemy Types
const EnemyType = {
    HUNTER: 'hunter',  // Chases the player
    PREY: 'prey'       // Flees from the player
};

// Player class
class Player {
    constructor(x, y) {
        this.x = x;
        this.y = y;
        this.speed = 200; // pixels per second
        this.radius = 20;
    }

    update(keys, deltaTime, minX, maxX, minY, maxY) {
        let dx = 0;
        let dy = 0;

        if (keys['w'] || keys['W']) dy -= 1;
        if (keys['s'] || keys['S']) dy += 1;
        if (keys['a'] || keys['A']) dx -= 1;
        if (keys['d'] || keys['D']) dx += 1;

        // Normalize diagonal movement
        const length = Math.sqrt(dx * dx + dy * dy);
        if (length > 0) {
            dx /= length;
            dy /= length;
        }

        // Apply movement
        this.x += dx * this.speed * deltaTime;
        this.y += dy * this.speed * deltaTime;

        // Clamp to bounds (accounting for radius)
        this.x = Math.max(minX + this.radius, Math.min(maxX - this.radius, this.x));
        this.y = Math.max(minY + this.radius, Math.min(maxY - this.radius, this.y));
    }

    draw(ctx) {
        ctx.fillStyle = '#00FFFF'; // Cyan
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
    }
}

// Enemy class
class Enemy {
    constructor(x, y, type, speed) {
        this.x = x;
        this.y = y;
        this.type = type;
        this.speed = speed;
        this.radius = 15;
        this.health = 100;
        this.isAlive = true;
    }

    update(playerX, playerY, deltaTime, minX, maxX, minY, maxY) {
        if (!this.isAlive) return;

        let dirX, dirY;

        if (this.type === EnemyType.HUNTER) {
            // Hunters chase - move toward player
            dirX = playerX - this.x;
            dirY = playerY - this.y;
        } else {
            // Prey flee - move toward the farthest corner from player to maximize distance
            const corner = this.getFarthestCorner(playerX, playerY, minX, maxX, minY, maxY);
            dirX = corner.x - this.x;
            dirY = corner.y - this.y;
        }

        // Normalize direction
        const length = Math.sqrt(dirX * dirX + dirY * dirY);
        if (length > 0) {
            dirX /= length;
            dirY /= length;
        }

        // Apply movement
        this.x += dirX * this.speed * deltaTime;
        this.y += dirY * this.speed * deltaTime;

        // Clamp to bounds (accounting for radius)
        this.x = Math.max(minX + this.radius, Math.min(maxX - this.radius, this.x));
        this.y = Math.max(minY + this.radius, Math.min(maxY - this.radius, this.y));
    }

    getFarthestCorner(playerX, playerY, minX, maxX, minY, maxY) {
        const corners = [
            { x: minX + this.radius, y: minY + this.radius },     // Top-left
            { x: maxX - this.radius, y: minY + this.radius },     // Top-right
            { x: minX + this.radius, y: maxY - this.radius },     // Bottom-left
            { x: maxX - this.radius, y: maxY - this.radius }      // Bottom-right
        ];

        let farthestCorner = corners[0];
        let maxDistance = this.distance(playerX, playerY, corners[0].x, corners[0].y);

        for (let i = 1; i < corners.length; i++) {
            const dist = this.distance(playerX, playerY, corners[i].x, corners[i].y);
            if (dist > maxDistance) {
                maxDistance = dist;
                farthestCorner = corners[i];
            }
        }

        return farthestCorner;
    }

    distance(x1, y1, x2, y2) {
        const dx = x2 - x1;
        const dy = y2 - y1;
        return Math.sqrt(dx * dx + dy * dy);
    }

    draw(ctx) {
        if (!this.isAlive) return;

        // Set color based on type
        ctx.fillStyle = this.type === EnemyType.HUNTER ? '#FF0000' : '#00FF00'; // Red for hunter, Green for prey

        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
    }
}

// Collision Detection
function circleToCircle(x1, y1, r1, x2, y2, r2) {
    const dx = x2 - x1;
    const dy = y2 - y1;
    const distance = Math.sqrt(dx * dx + dy * dy);
    return distance < (r1 + r2);
}

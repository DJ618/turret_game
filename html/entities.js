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
        this.speed = 400; // pixels per second (matches .NET default)
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

// Turret class
class Turret {
    constructor(x, y) {
        this.x = x;
        this.y = y;
        this.size = 30; // Half-width of square (1.5x player radius)
        this.shootCooldown = 2.0; // Seconds between shots
        this.timeSinceLastShot = 0;
    }

    update(deltaTime) {
        this.timeSinceLastShot += deltaTime;
    }

    canShoot() {
        return this.timeSinceLastShot >= this.shootCooldown;
    }

    resetShootCooldown() {
        this.timeSinceLastShot = 0;
    }

    draw(ctx) {
        ctx.fillStyle = '#000000'; // Black square
        ctx.fillRect(
            this.x - this.size,
            this.y - this.size,
            this.size * 2,
            this.size * 2
        );
    }
}

// Projectile class
class Projectile {
    constructor(startX, startY, targetX, targetY) {
        this.x = startX;
        this.y = startY;
        this.radius = 5;
        this.speed = 500; // pixels per second
        this.isActive = true;

        // Calculate velocity toward target
        let dirX = targetX - startX;
        let dirY = targetY - startY;
        const length = Math.sqrt(dirX * dirX + dirY * dirY);

        if (length > 0) {
            dirX /= length;
            dirY /= length;
        }

        this.velocityX = dirX * this.speed;
        this.velocityY = dirY * this.speed;
    }

    update(deltaTime, minX, maxX, minY, maxY) {
        if (!this.isActive) return;

        // Move projectile
        this.x += this.velocityX * deltaTime;
        this.y += this.velocityY * deltaTime;

        // Deactivate if out of bounds
        if (this.x < minX || this.x > maxX || this.y < minY || this.y > maxY) {
            this.isActive = false;
        }
    }

    draw(ctx) {
        if (!this.isActive) return;

        ctx.fillStyle = '#FFFFFF'; // White
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
    }
}

// ResourcePickup class (coin)
class ResourcePickup {
    constructor(x, y, value = 1) {
        this.x = x;
        this.y = y;
        this.radius = 8;
        this.value = value;
        this.isCollected = false;
    }

    draw(ctx) {
        if (this.isCollected) return;

        // Draw gold coin
        ctx.fillStyle = '#FFD700'; // Gold
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();

        // Add border
        ctx.strokeStyle = '#FFA500'; // Orange
        ctx.lineWidth = 2;
        ctx.stroke();
    }
}

// Collision Detection
function circleToCircle(x1, y1, r1, x2, y2, r2) {
    const dx = x2 - x1;
    const dy = y2 - y1;
    const distance = Math.sqrt(dx * dx + dy * dy);
    return distance < (r1 + r2);
}

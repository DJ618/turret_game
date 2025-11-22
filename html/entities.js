// Enemy Types
const EnemyType = {
    HUNTER: 'hunter',  // Chases the player
    PREY: 'prey'       // Flees from the player
};

// Flee Strategies
const FleeStrategy = {
    DIRECT_AWAY: 'direct_away',        // Flee directly away from player
    RANDOM_ANGLE: 'random_angle',      // Flee away with random angular offset
    PREFERRED_CORNER: 'preferred_corner' // Flee toward a specific corner
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

        // Constants for prey behavior
        this.FLEE_DISTANCE_THRESHOLD = 250; // Start fleeing when player is within 250 pixels
        this.WANDER_SPEED_MULTIPLIER = 0.3; // Move slower when wandering

        // Assign random flee strategy for prey
        if (type === EnemyType.PREY) {
            const strategyChoice = Math.floor(Math.random() * 3);
            this.fleeStrategy = Object.values(FleeStrategy)[strategyChoice];
            this.fleeAngleOffset = Math.random() * Math.PI / 2 - Math.PI / 4; // -45 to +45 degrees
            this.preferredCornerIndex = Math.floor(Math.random() * 4);

            // Initialize wander behavior with random direction
            this.wanderDirection = this.getRandomDirection();
            this.wanderChangeInterval = 1 + Math.random() * 2; // Change direction every 1-3 seconds
            this.wanderTimer = 0;
        }
    }

    getRandomDirection() {
        const angle = Math.random() * 2 * Math.PI;
        return {
            x: Math.cos(angle),
            y: Math.sin(angle)
        };
    }

    update(playerX, playerY, deltaTime, minX, maxX, minY, maxY) {
        if (!this.isAlive) return;

        let dirX, dirY;

        if (this.type === EnemyType.HUNTER) {
            // Hunters chase - move toward player
            dirX = playerX - this.x;
            dirY = playerY - this.y;
        } else {
            // Prey behavior depends on distance to player
            const distanceToPlayer = this.distance(this.x, this.y, playerX, playerY);

            if (distanceToPlayer < this.FLEE_DISTANCE_THRESHOLD) {
                // Player is close - flee actively
                const fleeDir = this.getFleeDirection(playerX, playerY, minX, maxX, minY, maxY);
                dirX = fleeDir.x;
                dirY = fleeDir.y;
            } else {
                // Player is far - wander slowly, biased away from edges
                this.wanderTimer += deltaTime;
                if (this.wanderTimer >= this.wanderChangeInterval) {
                    this.wanderTimer = 0;
                    this.wanderDirection = this.getWanderDirection(minX, maxX, minY, maxY);
                    this.wanderChangeInterval = 1 + Math.random() * 2;
                }
                dirX = this.wanderDirection.x;
                dirY = this.wanderDirection.y;
            }
        }

        // Normalize direction
        const length = Math.sqrt(dirX * dirX + dirY * dirY);
        if (length > 0) {
            dirX /= length;
            dirY /= length;
        }

        // Adjust speed based on behavior (wander slower)
        let effectiveSpeed = this.speed;
        if (this.type === EnemyType.PREY) {
            const distanceToPlayer = this.distance(this.x, this.y, playerX, playerY);
            if (distanceToPlayer >= this.FLEE_DISTANCE_THRESHOLD) {
                effectiveSpeed *= this.WANDER_SPEED_MULTIPLIER;
            }
        }

        // Apply movement
        this.x += dirX * effectiveSpeed * deltaTime;
        this.y += dirY * effectiveSpeed * deltaTime;

        // Clamp to bounds (accounting for radius)
        this.x = Math.max(minX + this.radius, Math.min(maxX - this.radius, this.x));
        this.y = Math.max(minY + this.radius, Math.min(maxY - this.radius, this.y));
    }

    getFleeDirection(playerX, playerY, minX, maxX, minY, maxY) {
        switch (this.fleeStrategy) {
            case FleeStrategy.DIRECT_AWAY:
                // Flee directly away from player
                return {
                    x: this.x - playerX,
                    y: this.y - playerY
                };

            case FleeStrategy.RANDOM_ANGLE: {
                // Flee away from player with angular offset
                let awayX = this.x - playerX;
                let awayY = this.y - playerY;
                const awayLength = Math.sqrt(awayX * awayX + awayY * awayY);

                if (awayLength > 0) {
                    awayX /= awayLength;
                    awayY /= awayLength;

                    // Rotate by the offset angle
                    const cos = Math.cos(this.fleeAngleOffset);
                    const sin = Math.sin(this.fleeAngleOffset);
                    return {
                        x: awayX * cos - awayY * sin,
                        y: awayX * sin + awayY * cos
                    };
                }
                return { x: 0, y: 0 };
            }

            case FleeStrategy.PREFERRED_CORNER: {
                // Flee toward specific corner
                const corner = this.getCorner(this.preferredCornerIndex, minX, maxX, minY, maxY);
                return {
                    x: corner.x - this.x,
                    y: corner.y - this.y
                };
            }

            default:
                return {
                    x: this.x - playerX,
                    y: this.y - playerY
                };
        }
    }

    getCorner(cornerIndex, minX, maxX, minY, maxY) {
        switch (cornerIndex) {
            case 0: return { x: minX + this.radius, y: minY + this.radius };     // Top-left
            case 1: return { x: maxX - this.radius, y: minY + this.radius };     // Top-right
            case 2: return { x: minX + this.radius, y: maxY - this.radius };     // Bottom-left
            case 3: return { x: maxX - this.radius, y: maxY - this.radius };     // Bottom-right
            default: return { x: (minX + maxX) / 2, y: (minY + maxY) / 2 };
        }
    }

    getWanderDirection(minX, maxX, minY, maxY) {
        // Calculate center of bounds
        const centerX = (minX + maxX) / 2;
        const centerY = (minY + maxY) / 2;

        // Calculate distance to each edge
        const edgeThreshold = 200; // Start biasing toward center when within 200 pixels of edge
        const distToLeft = this.x - minX;
        const distToRight = maxX - this.x;
        const distToTop = this.y - minY;
        const distToBottom = maxY - this.y;

        // Determine if we're near any edge
        const nearEdge = distToLeft < edgeThreshold || distToRight < edgeThreshold ||
                        distToTop < edgeThreshold || distToBottom < edgeThreshold;

        if (nearEdge) {
            // Bias toward center
            let towardCenterX = centerX - this.x;
            let towardCenterY = centerY - this.y;
            const centerLength = Math.sqrt(towardCenterX * towardCenterX + towardCenterY * towardCenterY);

            if (centerLength > 0) {
                towardCenterX /= centerLength;
                towardCenterY /= centerLength;
            }

            const randomDir = this.getRandomDirection();

            // Blend 70% toward center, 30% random for natural movement
            const blendedX = towardCenterX * 0.7 + randomDir.x * 0.3;
            const blendedY = towardCenterY * 0.7 + randomDir.y * 0.3;
            const blendedLength = Math.sqrt(blendedX * blendedX + blendedY * blendedY);

            if (blendedLength > 0) {
                return {
                    x: blendedX / blendedLength,
                    y: blendedY / blendedLength
                };
            }
        }

        // Not near edge, wander randomly
        return this.getRandomDirection();
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

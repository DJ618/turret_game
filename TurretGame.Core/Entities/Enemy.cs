using System;
using System.Numerics;

namespace TurretGame.Core.Entities;

public enum EnemyType
{
    Hunter,  // Chases the player
    Prey     // Flees from the player
}

public enum FleeStrategy
{
    DirectAway,      // Flee directly away from player
    RandomAngle,     // Flee away with random angular offset
    PreferredCorner  // Flee toward a specific corner
}

public class Enemy
{
    private static readonly Random _random = new Random();
    private const float FLEE_DISTANCE_THRESHOLD = 250f; // Start fleeing when player is within 250 pixels
    private const float WANDER_SPEED_MULTIPLIER = 0.3f; // Move slower when wandering
    private const float DIRECTION_SMOOTHING = 0.15f; // How quickly to turn (0.1 = smooth, 1.0 = instant)
    private const int DIRECTION_UPDATE_INTERVAL = 10; // Recalculate direction every 10 frames
    private const float EDGE_DISTANCE_THRESHOLD = 50f; // Distance from edge to be considered "at edge"
    private const float EDGE_STUCK_TIME = 3f; // Time in seconds before forcing movement away from edge

    private Vector2 _wanderDirection;
    private Vector2 _currentFleeDirection; // Smoothed flee direction
    private Vector2 _cachedDirection; // Cached direction vector
    private float _wanderTimer;
    private float _wanderChangeInterval;
    private int _frameCounter; // Counter for direction updates
    private float _edgeStuckTimer; // Timer for how long enemy has been near edge
    private bool _isNearEdge; // Whether enemy is currently near an edge

    public Vector2 Position { get; private set; }
    public float Speed { get; set; } = 160f;
    public float Health { get; set; } = 100f;
    public float Radius { get; set; } = 15f;
    public bool IsAlive { get; set; } = true;
    public EnemyType Type { get; set; }
    public FleeStrategy FleeStrategy { get; set; }
    public float FleeAngleOffset { get; private set; }
    public int PreferredCornerIndex { get; private set; }

    public Enemy(Vector2 startPosition, EnemyType type, float speed)
    {
        Position = startPosition;
        Type = type;
        Speed = speed;
        _frameCounter = 0;
        _cachedDirection = Vector2.Zero;
        _edgeStuckTimer = 0f;
        _isNearEdge = false;

        // Assign random flee strategy for prey
        if (type == EnemyType.Prey)
        {
            int strategyChoice = _random.Next(3);
            FleeStrategy = (FleeStrategy)strategyChoice;
            FleeAngleOffset = (float)(_random.NextDouble() * Math.PI / 2 - Math.PI / 4); // -45 to +45 degrees
            PreferredCornerIndex = _random.Next(4);

            // Initialize wander behavior with random direction
            // (bounds-aware direction will be set on first update)
            _wanderDirection = GetRandomDirection();
            _currentFleeDirection = GetRandomDirection(); // Initialize flee direction
            _wanderChangeInterval = 1f + (float)_random.NextDouble() * 2f; // Change direction every 1-3 seconds
            _wanderTimer = 0f;
        }
    }

    public void SetPosition(Vector2 newPosition)
    {
        Position = newPosition;
    }

    public void Update(Vector2 targetPosition, float deltaTime, float minX, float maxX, float minY, float maxY)
    {
        if (!IsAlive) return;

        // Increment frame counter
        _frameCounter++;

        // Only recalculate direction every 10 frames
        if (_frameCounter % DIRECTION_UPDATE_INTERVAL == 0)
        {
            Vector2 direction;

            if (Type == EnemyType.Hunter)
            {
                // Hunters chase - move toward player
                direction = targetPosition - Position;
            }
            else
            {
                // Prey behavior depends on distance to player
                float distanceToPlayer = Vector2.Distance(Position, targetPosition);

                // Check if near edge
                bool wasNearEdge = _isNearEdge;
                _isNearEdge = IsNearEdge(minX, maxX, minY, maxY);

                // Track time spent near edge
                if (_isNearEdge)
                {
                    _edgeStuckTimer += deltaTime;
                }
                else
                {
                    _edgeStuckTimer = 0f;
                }

                // If stuck at edge for too long, force movement away from edge
                if (_edgeStuckTimer >= EDGE_STUCK_TIME)
                {
                    direction = GetDirectionAwayFromEdge(minX, maxX, minY, maxY);
                    _edgeStuckTimer = 0f; // Reset timer after forcing movement
                }
                else if (distanceToPlayer < FLEE_DISTANCE_THRESHOLD)
                {
                    // Player is close - flee actively with smooth direction changes
                    Vector2 desiredFleeDirection = GetFleeDirection(targetPosition, minX, maxX, minY, maxY);

                    // Normalize desired direction
                    if (desiredFleeDirection.Length() > 0)
                    {
                        desiredFleeDirection = Vector2.Normalize(desiredFleeDirection);
                    }

                    // Smoothly interpolate current direction toward desired direction
                    // This prevents jerky circular motion
                    _currentFleeDirection = Vector2.Lerp(_currentFleeDirection, desiredFleeDirection, DIRECTION_SMOOTHING);

                    // Normalize the result
                    if (_currentFleeDirection.Length() > 0)
                    {
                        _currentFleeDirection = Vector2.Normalize(_currentFleeDirection);
                    }

                    direction = _currentFleeDirection;
                }
                else
                {
                    // Player is far - wander slowly, biased away from edges
                    _wanderTimer += deltaTime;
                    if (_wanderTimer >= _wanderChangeInterval)
                    {
                        _wanderTimer = 0f;
                        _wanderDirection = GetWanderDirection(minX, maxX, minY, maxY);
                        _wanderChangeInterval = 1f + (float)_random.NextDouble() * 2f;
                    }
                    direction = _wanderDirection;
                }
            }

            // Cache the calculated direction
            _cachedDirection = direction;
        }

        // Normalize and move if we have a valid cached direction
        if (_cachedDirection.Length() > 0)
        {
            Vector2 normalizedDirection = Vector2.Normalize(_cachedDirection);

            // Adjust speed based on behavior (wander slower)
            float effectiveSpeed = Speed;
            if (Type == EnemyType.Prey && Vector2.Distance(Position, targetPosition) >= FLEE_DISTANCE_THRESHOLD)
            {
                effectiveSpeed *= WANDER_SPEED_MULTIPLIER;
            }

            Position += normalizedDirection * effectiveSpeed * deltaTime;
        }

        // Clamp to bounds (accounting for radius)
        Position = new Vector2(
            Math.Clamp(Position.X, minX + Radius, maxX - Radius),
            Math.Clamp(Position.Y, minY + Radius, maxY - Radius)
        );
    }

    private Vector2 GetFleeDirection(Vector2 playerPosition, float minX, float maxX, float minY, float maxY)
    {
        switch (FleeStrategy)
        {
            case FleeStrategy.DirectAway:
                // Flee directly away from player
                return Position - playerPosition;

            case FleeStrategy.RandomAngle:
                // Flee away from player with angular offset
                Vector2 awayFromPlayer = Position - playerPosition;
                if (awayFromPlayer.Length() > 0)
                {
                    awayFromPlayer = Vector2.Normalize(awayFromPlayer);
                    // Rotate by the offset angle
                    float cos = (float)Math.Cos(FleeAngleOffset);
                    float sin = (float)Math.Sin(FleeAngleOffset);
                    return new Vector2(
                        awayFromPlayer.X * cos - awayFromPlayer.Y * sin,
                        awayFromPlayer.X * sin + awayFromPlayer.Y * cos
                    );
                }
                return Vector2.Zero;

            case FleeStrategy.PreferredCorner:
                // Flee toward specific corner
                Vector2 corner = GetCorner(PreferredCornerIndex, minX, maxX, minY, maxY);
                return corner - Position;

            default:
                return Position - playerPosition;
        }
    }

    private Vector2 GetCorner(int cornerIndex, float minX, float maxX, float minY, float maxY)
    {
        return cornerIndex switch
        {
            0 => new Vector2(minX + Radius, minY + Radius),     // Top-left
            1 => new Vector2(maxX - Radius, minY + Radius),     // Top-right
            2 => new Vector2(minX + Radius, maxY - Radius),     // Bottom-left
            3 => new Vector2(maxX - Radius, maxY - Radius),     // Bottom-right
            _ => new Vector2((minX + maxX) / 2, (minY + maxY) / 2)
        };
    }

    private Vector2 GetRandomDirection()
    {
        // Generate random direction vector
        float angle = (float)(_random.NextDouble() * 2 * Math.PI);
        return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
    }

    private Vector2 GetWanderDirection(float minX, float maxX, float minY, float maxY)
    {
        // Calculate center of bounds
        Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);

        // Calculate distance to each edge
        float edgeThreshold = 200f; // Start biasing toward center when within 200 pixels of edge
        float distToLeft = Position.X - minX;
        float distToRight = maxX - Position.X;
        float distToTop = Position.Y - minY;
        float distToBottom = maxY - Position.Y;

        // Determine if we're near any edge
        bool nearEdge = distToLeft < edgeThreshold || distToRight < edgeThreshold ||
                       distToTop < edgeThreshold || distToBottom < edgeThreshold;

        if (nearEdge)
        {
            // Bias toward center
            Vector2 towardCenter = Vector2.Normalize(center - Position);
            Vector2 randomDir = GetRandomDirection();

            // Blend 70% toward center, 30% random for natural movement
            Vector2 blendedDirection = towardCenter * 0.7f + randomDir * 0.3f;
            return Vector2.Normalize(blendedDirection);
        }
        else
        {
            // Not near edge, wander randomly
            return GetRandomDirection();
        }
    }

    private bool IsNearEdge(float minX, float maxX, float minY, float maxY)
    {
        // Check if enemy is within threshold distance from any edge
        float distToLeft = Position.X - minX;
        float distToRight = maxX - Position.X;
        float distToTop = Position.Y - minY;
        float distToBottom = maxY - Position.Y;

        return distToLeft < EDGE_DISTANCE_THRESHOLD ||
               distToRight < EDGE_DISTANCE_THRESHOLD ||
               distToTop < EDGE_DISTANCE_THRESHOLD ||
               distToBottom < EDGE_DISTANCE_THRESHOLD;
    }

    private Vector2 GetDirectionAwayFromEdge(float minX, float maxX, float minY, float maxY)
    {
        // Calculate center of bounds
        Vector2 center = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);

        // Direction toward center
        Vector2 towardCenter = Vector2.Normalize(center - Position);

        // Add random angle variation (-45 to +45 degrees) for natural movement
        float randomAngle = (float)(_random.NextDouble() * Math.PI / 2 - Math.PI / 4);
        float cos = (float)Math.Cos(randomAngle);
        float sin = (float)Math.Sin(randomAngle);

        Vector2 rotatedDirection = new Vector2(
            towardCenter.X * cos - towardCenter.Y * sin,
            towardCenter.X * sin + towardCenter.Y * cos
        );

        return rotatedDirection;
    }
}

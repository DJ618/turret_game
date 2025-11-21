using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;
using SysVector2 = System.Numerics.Vector2;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace TurretGame.Game;

public enum GameState
{
    Playing,
    GameOver
}

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private List<Enemy> _enemies;
    private List<ResourcePickup> _resourcePickups;
    private Texture2D _playerTexture;
    private Texture2D _hunterTexture;
    private Texture2D _preyTexture;
    private Texture2D _coinSpriteSheet;
    private Random _random;
    private GameState _gameState;
    private Texture2D _gameOverTexture;
    private SpriteFont _gameFont;

    // Coin animation
    private int _coinFrameCount = 10;
    private int _coinCurrentFrame = 0;
    private float _coinFrameTime = 0f;
    private float _coinFrameDuration = 0.08f; // 80ms per frame
    private int _coinFrameWidth;
    private int _coinFrameHeight;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _random = new Random();
        _enemies = new List<Enemy>();
        _resourcePickups = new List<ResourcePickup>();
        _gameState = GameState.Playing;
    }

    protected override void Initialize()
    {
        // Set window to fullscreen desktop resolution
        var displayMode = GraphicsDevice.Adapter.CurrentDisplayMode;
        _graphics.PreferredBackBufferWidth = displayMode.Width;
        _graphics.PreferredBackBufferHeight = displayMode.Height;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create player at screen center
        var screenCenter = new SysVector2(
            GraphicsDevice.Viewport.Width / 2f,
            GraphicsDevice.Viewport.Height / 2f
        );
        _player = new Player(screenCenter);

        // Create textures
        _playerTexture = CreateCircleTexture(GraphicsDevice, (int)_player.Radius, Color.Cyan);
        _hunterTexture = CreateCircleTexture(GraphicsDevice, 15, Color.Red);
        _preyTexture = CreateCircleTexture(GraphicsDevice, 15, Color.Green);

        // Load coin sprite sheet
        _coinSpriteSheet = Content.Load<Texture2D>("CoinSprite");
        _coinFrameWidth = _coinSpriteSheet.Width / _coinFrameCount;
        _coinFrameHeight = _coinSpriteSheet.Height;

        // Load font
        _gameFont = Content.Load<SpriteFont>("GameFont");

        // Create game over overlay texture (semi-transparent red)
        _gameOverTexture = new Texture2D(GraphicsDevice, 1, 1);
        _gameOverTexture.SetData(new[] { new Color(255, 0, 0, 180) });

        // Spawn 2 enemies: 1 Hunter (chaser) and 1 Prey (fleer)
        // Hunter at 50% player speed (100f), Prey at 80% player speed (160f)
        var hunterPosition = GetRandomEdgePosition();
        var hunter = new Enemy(hunterPosition, EnemyType.Hunter, 100f);
        _enemies.Add(hunter);

        var preyPosition = GetRandomEdgePosition();
        var prey = new Enemy(preyPosition, EnemyType.Prey, 160f);
        _enemies.Add(prey);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Don't update game if it's over
        if (_gameState == GameState.GameOver)
        {
            base.Update(gameTime);
            return;
        }

        // Read WASD input
        var keyboardState = Keyboard.GetState();
        var movementDirection = SysVector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
            movementDirection.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S))
            movementDirection.Y += 1;
        if (keyboardState.IsKeyDown(Keys.A))
            movementDirection.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D))
            movementDirection.X += 1;

        // Update player with bounds clamping
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _player.Update(
            movementDirection,
            deltaTime,
            0,
            GraphicsDevice.Viewport.Width,
            0,
            GraphicsDevice.Viewport.Height
        );

        // Update all enemies (hunters chase, prey flee)
        foreach (var enemy in _enemies)
        {
            enemy.Update(
                _player.Position,
                deltaTime,
                0,
                GraphicsDevice.Viewport.Width,
                0,
                GraphicsDevice.Viewport.Height
            );

            // Check collision between player and this enemy
            if (enemy.IsAlive && CollisionDetection.CircleToCircle(
                _player.Position, _player.Radius,
                enemy.Position, enemy.Radius))
            {
                if (enemy.Type == EnemyType.Hunter)
                {
                    // Collision with hunter = game over
                    _gameState = GameState.GameOver;
                }
                else
                {
                    // Collision with prey = prey disappears and drops resource
                    enemy.IsAlive = false;

                    // Spawn resource pickup at enemy's position
                    var pickup = new ResourcePickup(enemy.Position, 1);
                    _resourcePickups.Add(pickup);
                }
            }
        }

        // Check collision between player and resource pickups
        foreach (var pickup in _resourcePickups)
        {
            if (!pickup.IsCollected && CollisionDetection.CircleToCircle(
                _player.Position, _player.Radius,
                pickup.Position, pickup.Radius))
            {
                // Collect the resource
                pickup.IsCollected = true;
                _player.CollectResource(pickup.Value);
            }
        }

        // Remove collected pickups
        _resourcePickups.RemoveAll(p => p.IsCollected);

        // Update coin animation
        _coinFrameTime += deltaTime;
        if (_coinFrameTime >= _coinFrameDuration)
        {
            _coinFrameTime -= _coinFrameDuration;
            _coinCurrentFrame = (_coinCurrentFrame + 1) % _coinFrameCount;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw player
        var playerPos = new XnaVector2(_player.Position.X, _player.Position.Y);
        var playerOrigin = new XnaVector2(_player.Radius, _player.Radius);
        _spriteBatch.Draw(_playerTexture, playerPos, null, Color.White, 0f, playerOrigin, 1f, SpriteEffects.None, 0f);

        // Draw all enemies if alive (Hunters=red, Prey=green)
        foreach (var enemy in _enemies)
        {
            if (enemy.IsAlive)
            {
                var enemyPos = new XnaVector2(enemy.Position.X, enemy.Position.Y);
                var enemyOrigin = new XnaVector2(enemy.Radius, enemy.Radius);
                var texture = enemy.Type == EnemyType.Hunter ? _hunterTexture : _preyTexture;
                _spriteBatch.Draw(texture, enemyPos, null, Color.White, 0f, enemyOrigin, 1f, SpriteEffects.None, 0f);
            }
        }

        // Draw all resource pickups with animated coin sprite
        foreach (var pickup in _resourcePickups)
        {
            if (!pickup.IsCollected)
            {
                var pickupPos = new XnaVector2(pickup.Position.X, pickup.Position.Y);

                // Calculate source rectangle for current animation frame
                var sourceRect = new Rectangle(
                    _coinCurrentFrame * _coinFrameWidth,
                    0,
                    _coinFrameWidth,
                    _coinFrameHeight
                );

                // Center the sprite on the pickup position
                var origin = new XnaVector2(_coinFrameWidth / 2f, _coinFrameHeight / 2f);

                _spriteBatch.Draw(_coinSpriteSheet, pickupPos, sourceRect, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        // Draw game over overlay if game is over
        if (_gameState == GameState.GameOver)
        {
            // Draw semi-transparent red overlay covering entire screen
            var screenRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.Draw(_gameOverTexture, screenRect, Color.White);

            // Draw "GAME OVER" text centered on screen
            string gameOverText = "GAME OVER";
            var textSize = _gameFont.MeasureString(gameOverText);
            var textPosition = new XnaVector2(
                GraphicsDevice.Viewport.Width / 2f - textSize.X / 2f,
                GraphicsDevice.Viewport.Height / 2f - textSize.Y / 2f
            );
            _spriteBatch.DrawString(_gameFont, gameOverText, textPosition, Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color)
    {
        int diameter = radius * 2;
        var texture = new Texture2D(graphicsDevice, diameter, diameter);
        var colorData = new Color[diameter * diameter];

        float radiusSquared = radius * radius;

        for (int y = 0; y < diameter; y++)
        {
            for (int x = 0; x < diameter; x++)
            {
                int index = y * diameter + x;
                float dx = x - radius;
                float dy = y - radius;
                float distanceSquared = dx * dx + dy * dy;

                if (distanceSquared <= radiusSquared)
                {
                    colorData[index] = color;
                }
                else
                {
                    colorData[index] = Color.Transparent;
                }
            }
        }

        texture.SetData(colorData);
        return texture;
    }

    private SysVector2 GetRandomEdgePosition()
    {
        int screenWidth = GraphicsDevice.Viewport.Width;
        int screenHeight = GraphicsDevice.Viewport.Height;
        int edge = _random.Next(4); // 0=top, 1=right, 2=bottom, 3=left

        return edge switch
        {
            0 => new SysVector2(_random.Next(0, screenWidth), 0), // Top edge
            1 => new SysVector2(screenWidth, _random.Next(0, screenHeight)), // Right edge
            2 => new SysVector2(_random.Next(0, screenWidth), screenHeight), // Bottom edge
            3 => new SysVector2(0, _random.Next(0, screenHeight)), // Left edge
            _ => new SysVector2(screenWidth / 2f, 0) // Default to top center
        };
    }
}

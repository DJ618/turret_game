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
    private Texture2D _playerTexture;
    private Texture2D _hunterTexture;
    private Texture2D _preyTexture;
    private Random _random;
    private GameState _gameState;
    private Texture2D _gameOverTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _random = new Random();
        _enemies = new List<Enemy>();
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
                    // Collision with prey = prey disappears
                    enemy.IsAlive = false;
                }
            }
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

        // Draw game over overlay if game is over
        if (_gameState == GameState.GameOver)
        {
            // Draw semi-transparent red overlay covering entire screen
            var screenRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            _spriteBatch.Draw(_gameOverTexture, screenRect, Color.White);

            // Draw "GAME OVER" using large circles (simple visual indicator)
            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            // Draw large text indicator circles spelling "G O"
            for (int i = -3; i <= 3; i++)
            {
                var pos = new XnaVector2(centerX + i * 60, centerY);
                _spriteBatch.Draw(_playerTexture, pos, null, Color.White, 0f,
                    new XnaVector2(_player.Radius, _player.Radius), 2f, SpriteEffects.None, 0f);
            }
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

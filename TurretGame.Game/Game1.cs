using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TurretGame.Core.Entities;
using TurretGame.Core.Utilities;
using SysVector2 = System.Numerics.Vector2;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace TurretGame.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Enemy _enemy;
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;
    private Random _random;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _random = new Random();
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

        // Spawn enemy at random edge
        var enemyPosition = GetRandomEdgePosition();
        _enemy = new Enemy(enemyPosition);
        _enemyTexture = CreateCircleTexture(GraphicsDevice, (int)_enemy.Radius, Color.Red);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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

        // Update enemy to chase player
        _enemy.Update(_player.Position, deltaTime);

        // Check collision between player and enemy
        if (_enemy.IsAlive && CollisionDetection.CircleToCircle(
            _player.Position, _player.Radius,
            _enemy.Position, _enemy.Radius))
        {
            _enemy.IsAlive = false;
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

        // Draw enemy if alive
        if (_enemy.IsAlive)
        {
            var enemyPos = new XnaVector2(_enemy.Position.X, _enemy.Position.Y);
            var enemyOrigin = new XnaVector2(_enemy.Radius, _enemy.Radius);
            _spriteBatch.Draw(_enemyTexture, enemyPos, null, Color.White, 0f, enemyOrigin, 1f, SpriteEffects.None, 0f);
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

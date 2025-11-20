using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TurretGame.Core.Entities;
using SysVector2 = System.Numerics.Vector2;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace TurretGame.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Texture2D _circleTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
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

        // Create circle texture for player rendering
        _circleTexture = CreateCircleTexture(GraphicsDevice, (int)_player.Radius, Color.Cyan);
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw player as a colored circle
        var playerPos = new XnaVector2(_player.Position.X, _player.Position.Y);
        var origin = new XnaVector2(_player.Radius, _player.Radius);
        _spriteBatch.Draw(_circleTexture, playerPos, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);

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
}

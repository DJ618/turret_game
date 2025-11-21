using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TurretGame.Application;
using TurretGame.Application.State;
using TurretGame.Application.Systems;
using TurretGame.Core.Entities;
using TurretGame.Core.Interfaces;
using TurretGame.Core.State;
using TurretGame.Core.Utilities;
using TurretGame.Infrastructure.Graphics;
using TurretGame.Infrastructure.Input;
using TurretGame.Infrastructure.Rendering;
using SysVector2 = System.Numerics.Vector2;

namespace TurretGame.Game;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private readonly GraphicsDeviceManager _graphics;
    private ServiceProvider _serviceProvider;

    // Services injected from DI container
    private GameplayLoop _gameplayLoop;
    private EntityRenderer _entityRenderer;
    private UIRenderer _uiRenderer;
    private SpriteAnimator _coinAnimator;
    private SpawnManager _spawnManager;
    private StatisticsManager _statisticsManager;

    private SpriteBatch _spriteBatch;
    private Bounds _viewportBounds;

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

        // Calculate viewport bounds
        _viewportBounds = new Bounds(
            0,
            GraphicsDevice.Viewport.Width,
            0,
            GraphicsDevice.Viewport.Height
        );

        // Build dependency injection container
        _serviceProvider = ConfigureServices();

        // Resolve main services
        _gameplayLoop = _serviceProvider.GetRequiredService<GameplayLoop>();
        _entityRenderer = _serviceProvider.GetRequiredService<EntityRenderer>();
        _uiRenderer = _serviceProvider.GetRequiredService<UIRenderer>();
        _coinAnimator = _serviceProvider.GetRequiredService<SpriteAnimator>();
        _spawnManager = _serviceProvider.GetRequiredService<SpawnManager>();
        _statisticsManager = _serviceProvider.GetRequiredService<StatisticsManager>();

        // First wave will be spawned automatically by WaveManager
    }

    private ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Core services
        var screenCenter = new SysVector2(
            GraphicsDevice.Viewport.Width / 2f,
            GraphicsDevice.Viewport.Height / 2f
        );
        var player = new Player(screenCenter);
        services.AddSingleton(player);

        // Application layer services
        services.AddSingleton<EntityManager>();
        services.AddSingleton<GameStateManager>();
        services.AddSingleton<ResourceManager>();
        services.AddSingleton<StatisticsManager>();
        services.AddSingleton<SpawnManager>();
        services.AddSingleton<WaveManager>();
        services.AddSingleton<ICollisionHandler, GameCollisionHandler>();
        services.AddSingleton<CollisionSystem>();
        services.AddSingleton<GameplayLoop>();

        // Infrastructure layer services
        services.AddSingleton<IInputService, MonoGameInputService>();

        // Graphics services
        var textureFactory = new TextureFactory(GraphicsDevice);
        services.AddSingleton(textureFactory);

        var playerTexture = textureFactory.CreateCircleTexture((int)player.Radius, Color.Cyan);
        var hunterTexture = textureFactory.CreateCircleTexture(15, Color.Red);
        var preyTexture = textureFactory.CreateCircleTexture(15, Color.Green);

        var coinSpriteSheet = Content.Load<Texture2D>("CoinSprite");
        var coinAnimator = new SpriteAnimator(coinSpriteSheet, frameCount: 10, frameDuration: 0.08f);
        services.AddSingleton(coinAnimator);

        services.AddSingleton(new EntityRenderer(
            _spriteBatch,
            playerTexture,
            hunterTexture,
            preyTexture,
            coinAnimator
        ));

        // UI services
        var gameFont = Content.Load<SpriteFont>("GameFont");
        var gameOverTexture = new Texture2D(GraphicsDevice, 1, 1);
        gameOverTexture.SetData(new[] { new Color(255, 0, 0, 180) });

        services.AddSingleton(new UIRenderer(
            _spriteBatch,
            GraphicsDevice,
            gameOverTexture,
            gameFont
        ));

        return services.BuildServiceProvider();
    }

    protected override void Update(GameTime gameTime)
    {
        if (_gameplayLoop.ShouldExit())
        {
            Exit();
            return;
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Update gameplay logic
        _gameplayLoop.Update(deltaTime, _viewportBounds);

        // Update coin animation
        _coinAnimator.Update(deltaTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw player
        _entityRenderer.DrawPlayer(_gameplayLoop.Player);

        // Draw enemies
        foreach (var enemy in _gameplayLoop.EntityManager.Enemies)
        {
            _entityRenderer.DrawEnemy(enemy);
        }

        // Draw resource pickups
        foreach (var pickup in _gameplayLoop.EntityManager.Pickups)
        {
            _entityRenderer.DrawPickup(pickup);
        }

        // Draw resource counter and wave number
        _uiRenderer.DrawResourceCounter(
            _gameplayLoop.ResourceManager.ResourceCount,
            _gameplayLoop.WaveManager.CurrentWave
        );

        // Draw UI overlays
        if (_gameplayLoop.GameStateManager.IsGameOver())
        {
            _uiRenderer.DrawGameOver(
                _gameplayLoop.WaveManager.CurrentWave,
                _gameplayLoop.ResourceManager.ResourceCount,
                _statisticsManager.EnemiesKilled
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _serviceProvider?.Dispose();
        }
        base.Dispose(disposing);
    }
}

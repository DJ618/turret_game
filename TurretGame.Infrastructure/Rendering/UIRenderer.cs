using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TurretGame.Core.State;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace TurretGame.Infrastructure.Rendering;

public class UIRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Texture2D _gameOverTexture;
    private readonly SpriteFont _font;

    public UIRenderer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D gameOverTexture, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDevice;
        _gameOverTexture = gameOverTexture;
        _font = font;
    }

    public void DrawGameOver(int levelReached, int resourcesCollected, int enemiesKilled)
    {
        // Draw semi-transparent red overlay
        var screenRect = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_gameOverTexture, screenRect, Color.White);

        // Draw "GAME OVER" text centered
        string gameOverText = "GAME OVER";
        var gameOverSize = _font.MeasureString(gameOverText);
        var gameOverPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - gameOverSize.X / 2f,
            _graphicsDevice.Viewport.Height / 2f - gameOverSize.Y - 100f
        );
        _spriteBatch.DrawString(_font, gameOverText, gameOverPosition, Color.White);

        // Draw statistics below the game over text
        float startY = gameOverPosition.Y + gameOverSize.Y + 40f;
        float lineSpacing = 60f;

        string levelText = $"Level Reached: {levelReached}";
        var levelSize = _font.MeasureString(levelText);
        var levelPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - levelSize.X / 2f,
            startY
        );
        _spriteBatch.DrawString(_font, levelText, levelPosition, Color.Yellow);

        string resourceText = $"Resources Collected: {resourcesCollected}";
        var resourceSize = _font.MeasureString(resourceText);
        var resourcePosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - resourceSize.X / 2f,
            startY + lineSpacing
        );
        _spriteBatch.DrawString(_font, resourceText, resourcePosition, Color.Gold);

        string killsText = $"Enemies Killed: {enemiesKilled}";
        var killsSize = _font.MeasureString(killsText);
        var killsPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - killsSize.X / 2f,
            startY + lineSpacing * 2
        );
        _spriteBatch.DrawString(_font, killsText, killsPosition, Color.White);
    }

    public void DrawResourceCounter(int resourceCount, int currentWave)
    {
        // Draw resource count in top-left corner
        string resourceText = $"Resources: {resourceCount}";
        var resourcePosition = new XnaVector2(20, 20);
        _spriteBatch.DrawString(_font, resourceText, resourcePosition, Color.Gold);

        // Draw wave count below resource counter
        string waveText = $"Wave: {currentWave}";
        var wavePosition = new XnaVector2(20, 80);
        _spriteBatch.DrawString(_font, waveText, wavePosition, Color.White);
    }
}

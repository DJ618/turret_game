using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TurretGame.Core.State;
using TurretGame.Core.Upgrades;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;
using SysVector2 = System.Numerics.Vector2;

namespace TurretGame.Infrastructure.Rendering;

public class UIRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Texture2D _gameOverTexture;
    private readonly SpriteFont _font;
    private Texture2D _buttonTexture;

    public UIRenderer(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Texture2D gameOverTexture, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _graphicsDevice = graphicsDevice;
        _gameOverTexture = gameOverTexture;
        _font = font;

        // Create button texture
        _buttonTexture = new Texture2D(graphicsDevice, 1, 1);
        _buttonTexture.SetData(new[] { new Color(100, 100, 100, 200) });
    }

    public void DrawGameOver(int levelReached, int resourcesCollected, int enemiesKilled, int score)
    {
        // Draw semi-transparent red overlay
        var screenRect = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_gameOverTexture, screenRect, Color.White);

        // Draw "GAME OVER" text centered
        string gameOverText = "GAME OVER";
        var gameOverSize = _font.MeasureString(gameOverText);
        var gameOverPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - gameOverSize.X / 2f,
            _graphicsDevice.Viewport.Height / 2f - gameOverSize.Y - 150f
        );
        _spriteBatch.DrawString(_font, gameOverText, gameOverPosition, Color.White);

        // Draw score prominently below game over text
        float startY = gameOverPosition.Y + gameOverSize.Y + 40f;
        float lineSpacing = 60f;

        string scoreText = $"SCORE: {score}";
        var scoreSize = _font.MeasureString(scoreText);
        var scorePosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - scoreSize.X / 2f,
            startY
        );
        _spriteBatch.DrawString(_font, scoreText, scorePosition, Color.Cyan);

        // Draw statistics below the score
        string levelText = $"Level Reached: {levelReached}";
        var levelSize = _font.MeasureString(levelText);
        var levelPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - levelSize.X / 2f,
            startY + lineSpacing
        );
        _spriteBatch.DrawString(_font, levelText, levelPosition, Color.Yellow);

        string resourceText = $"Resources Collected: {resourcesCollected}";
        var resourceSize = _font.MeasureString(resourceText);
        var resourcePosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - resourceSize.X / 2f,
            startY + lineSpacing * 2
        );
        _spriteBatch.DrawString(_font, resourceText, resourcePosition, Color.Gold);

        string killsText = $"Enemies Killed: {enemiesKilled}";
        var killsSize = _font.MeasureString(killsText);
        var killsPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - killsSize.X / 2f,
            startY + lineSpacing * 3
        );
        _spriteBatch.DrawString(_font, killsText, killsPosition, Color.White);

        // Draw restart button
        DrawRestartButton();
    }

    private void DrawRestartButton()
    {
        var buttonRect = GetRestartButtonRect();
        _spriteBatch.Draw(_buttonTexture, buttonRect, Color.White);

        string buttonText = "RESTART";
        var textSize = _font.MeasureString(buttonText);
        var textPosition = new XnaVector2(
            buttonRect.X + buttonRect.Width / 2f - textSize.X / 2f,
            buttonRect.Y + buttonRect.Height / 2f - textSize.Y / 2f
        );
        _spriteBatch.DrawString(_font, buttonText, textPosition, Color.Black);
    }

    public bool IsRestartButtonClicked(SysVector2 mousePosition)
    {
        var buttonRect = GetRestartButtonRect();
        return buttonRect.Contains((int)mousePosition.X, (int)mousePosition.Y);
    }

    private Rectangle GetRestartButtonRect()
    {
        int buttonWidth = 300;
        int buttonHeight = 80;
        return new Rectangle(
            _graphicsDevice.Viewport.Width / 2 - buttonWidth / 2,
            _graphicsDevice.Viewport.Height / 2 + 220,
            buttonWidth,
            buttonHeight
        );
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

    public void DrawTurretPlacement(int currentWave, SysVector2 mousePosition, int turretsPlaced, int turretsAllowed)
    {
        // Draw semi-transparent overlay
        var screenRect = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_gameOverTexture, screenRect, new Color(0, 0, 0, 180));

        // Draw title text
        string titleText = $"Upgrade Applied!";
        var titleSize = _font.MeasureString(titleText);
        var titlePosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - titleSize.X / 2f,
            _graphicsDevice.Viewport.Height / 2f - titleSize.Y - 150f
        );
        _spriteBatch.DrawString(_font, titleText, titlePosition, Color.Green);

        // Draw turret placement progress
        string progressText = $"Turrets placed: {turretsPlaced}/{turretsAllowed}";
        var progressSize = _font.MeasureString(progressText);
        var progressPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - progressSize.X / 2f,
            titlePosition.Y + titleSize.Y + 20f
        );
        _spriteBatch.DrawString(_font, progressText, progressPosition, Color.Cyan);

        // Draw instruction text based on turret placement status
        bool allTurretsPlaced = turretsPlaced >= turretsAllowed;
        string instructionText = allTurretsPlaced ? "All turrets placed! Click CONTINUE" : "Click to place a turret";
        var instructionSize = _font.MeasureString(instructionText);
        var instructionPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - instructionSize.X / 2f,
            progressPosition.Y + progressSize.Y + 20f
        );
        Color instructionColor = allTurretsPlaced ? Color.Yellow : Color.White;
        _spriteBatch.DrawString(_font, instructionText, instructionPosition, instructionColor);

        // Draw turret preview at mouse position (only if not all placed yet)
        if (!allTurretsPlaced)
        {
            float turretSize = 30f;
            var turretRect = new Rectangle(
                (int)(mousePosition.X - turretSize),
                (int)(mousePosition.Y - turretSize),
                (int)(turretSize * 2),
                (int)(turretSize * 2)
            );
            _spriteBatch.Draw(_buttonTexture, turretRect, new Color(0, 0, 0, 150));
        }

        // Draw continue button
        DrawContinueButton();
    }

    private void DrawContinueButton()
    {
        var buttonRect = GetContinueButtonRect();
        _spriteBatch.Draw(_buttonTexture, buttonRect, Color.White);

        string buttonText = "CONTINUE";
        var textSize = _font.MeasureString(buttonText);
        var textPosition = new XnaVector2(
            buttonRect.X + buttonRect.Width / 2f - textSize.X / 2f,
            buttonRect.Y + buttonRect.Height / 2f - textSize.Y / 2f
        );
        _spriteBatch.DrawString(_font, buttonText, textPosition, Color.Black);
    }

    public bool IsContinueButtonClicked(SysVector2 mousePosition)
    {
        var buttonRect = GetContinueButtonRect();
        return buttonRect.Contains((int)mousePosition.X, (int)mousePosition.Y);
    }

    private Rectangle GetContinueButtonRect()
    {
        int buttonWidth = 300;
        int buttonHeight = 80;
        return new Rectangle(
            _graphicsDevice.Viewport.Width / 2 - buttonWidth / 2,
            _graphicsDevice.Viewport.Height - 200,
            buttonWidth,
            buttonHeight
        );
    }

    public void DrawUpgradeSelection(int currentWave, IReadOnlyList<IUpgrade> upgradeOptions)
    {
        // Draw semi-transparent overlay
        var screenRect = new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
        _spriteBatch.Draw(_gameOverTexture, screenRect, new Color(0, 0, 50, 220));

        // Draw title text
        string titleText = $"Wave {currentWave} Complete!";
        var titleSize = _font.MeasureString(titleText);
        var titlePosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - titleSize.X / 2f,
            100f
        );
        _spriteBatch.DrawString(_font, titleText, titlePosition, Color.Green);

        // Draw instruction text
        string instructionText = "Choose an upgrade:";
        var instructionSize = _font.MeasureString(instructionText);
        var instructionPosition = new XnaVector2(
            _graphicsDevice.Viewport.Width / 2f - instructionSize.X / 2f,
            titlePosition.Y + titleSize.Y + 30f
        );
        _spriteBatch.DrawString(_font, instructionText, instructionPosition, Color.White);

        // Draw upgrade option buttons
        int buttonWidth = 450;
        int buttonHeight = 150;
        int buttonSpacing = 40;
        int totalHeight = (buttonHeight * upgradeOptions.Count) + (buttonSpacing * (upgradeOptions.Count - 1));
        int startY = (_graphicsDevice.Viewport.Height - totalHeight) / 2 + 50;

        for (int i = 0; i < upgradeOptions.Count; i++)
        {
            var upgrade = upgradeOptions[i];
            var buttonRect = GetUpgradeButtonRect(i, buttonWidth, buttonHeight, buttonSpacing, startY);

            // Draw button background
            _spriteBatch.Draw(_buttonTexture, buttonRect, Color.DarkBlue);

            // Draw upgrade name
            string nameText = $"{i + 1}. {upgrade.Name}";
            var nameSize = _font.MeasureString(nameText);
            var namePosition = new XnaVector2(
                buttonRect.X + buttonRect.Width / 2f - nameSize.X / 2f,
                buttonRect.Y + 20f
            );
            _spriteBatch.DrawString(_font, nameText, namePosition, Color.Yellow);

            // Draw upgrade description
            string descText = upgrade.Description;
            var descSize = _font.MeasureString(descText);
            var descPosition = new XnaVector2(
                buttonRect.X + buttonRect.Width / 2f - descSize.X / 2f,
                buttonRect.Y + buttonRect.Height / 2f + 10f
            );
            _spriteBatch.DrawString(_font, descText, descPosition, Color.White);
        }
    }

    public int GetClickedUpgradeOption(SysVector2 mousePosition, int upgradeCount)
    {
        int buttonWidth = 450;
        int buttonHeight = 150;
        int buttonSpacing = 40;
        int totalHeight = (buttonHeight * upgradeCount) + (buttonSpacing * (upgradeCount - 1));
        int startY = (_graphicsDevice.Viewport.Height - totalHeight) / 2 + 50;

        for (int i = 0; i < upgradeCount; i++)
        {
            var buttonRect = GetUpgradeButtonRect(i, buttonWidth, buttonHeight, buttonSpacing, startY);
            if (buttonRect.Contains((int)mousePosition.X, (int)mousePosition.Y))
            {
                return i;
            }
        }
        return -1;
    }

    private Rectangle GetUpgradeButtonRect(int index, int buttonWidth, int buttonHeight, int buttonSpacing, int startY)
    {
        int x = _graphicsDevice.Viewport.Width / 2 - buttonWidth / 2;
        int y = startY + (index * (buttonHeight + buttonSpacing));
        return new Rectangle(x, y, buttonWidth, buttonHeight);
    }
}

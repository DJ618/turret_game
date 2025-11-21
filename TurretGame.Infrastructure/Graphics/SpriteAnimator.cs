using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TurretGame.Infrastructure.Graphics;

public class SpriteAnimator
{
    private readonly Texture2D _spriteSheet;
    private readonly int _frameCount;
    private readonly float _frameDuration;
    private readonly int _frameWidth;
    private readonly int _frameHeight;

    private int _currentFrame;
    private float _frameTime;

    public SpriteAnimator(Texture2D spriteSheet, int frameCount, float frameDuration)
    {
        _spriteSheet = spriteSheet;
        _frameCount = frameCount;
        _frameDuration = frameDuration;
        _frameWidth = spriteSheet.Width / frameCount;
        _frameHeight = spriteSheet.Height;
        _currentFrame = 0;
        _frameTime = 0f;
    }

    public void Update(float deltaTime)
    {
        _frameTime += deltaTime;

        if (_frameTime >= _frameDuration)
        {
            _frameTime -= _frameDuration;
            _currentFrame = (_currentFrame + 1) % _frameCount;
        }
    }

    public Rectangle GetCurrentFrameRectangle()
    {
        return new Rectangle(
            _currentFrame * _frameWidth,
            0,
            _frameWidth,
            _frameHeight
        );
    }

    public Vector2 GetOrigin()
    {
        return new Vector2(_frameWidth / 2f, _frameHeight / 2f);
    }

    public Texture2D Texture => _spriteSheet;
}

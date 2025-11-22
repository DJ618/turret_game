using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TurretGame.Core.Entities;
using TurretGame.Infrastructure.Graphics;
using SysVector2 = System.Numerics.Vector2;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace TurretGame.Infrastructure.Rendering;

public class EntityRenderer
{
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _playerTexture;
    private readonly Texture2D _hunterTexture;
    private readonly Texture2D _preyTexture;
    private readonly Texture2D _turretTexture;
    private readonly Texture2D _projectileTexture;
    private readonly SpriteAnimator _coinAnimator;

    public EntityRenderer(
        SpriteBatch spriteBatch,
        Texture2D playerTexture,
        Texture2D hunterTexture,
        Texture2D preyTexture,
        Texture2D turretTexture,
        Texture2D projectileTexture,
        SpriteAnimator coinAnimator)
    {
        _spriteBatch = spriteBatch;
        _playerTexture = playerTexture;
        _hunterTexture = hunterTexture;
        _preyTexture = preyTexture;
        _turretTexture = turretTexture;
        _projectileTexture = projectileTexture;
        _coinAnimator = coinAnimator;
    }

    public void DrawPlayer(Player player)
    {
        var position = ToXnaVector2(player.Position);
        var origin = new XnaVector2(player.Radius, player.Radius);
        _spriteBatch.Draw(_playerTexture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    public void DrawEnemy(Enemy enemy)
    {
        if (!enemy.IsAlive) return;

        var position = ToXnaVector2(enemy.Position);
        var origin = new XnaVector2(enemy.Radius, enemy.Radius);
        var texture = enemy.Type == EnemyType.Hunter ? _hunterTexture : _preyTexture;
        _spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    public void DrawPickup(ResourcePickup pickup)
    {
        if (pickup.IsCollected) return;

        var position = ToXnaVector2(pickup.Position);
        var sourceRect = _coinAnimator.GetCurrentFrameRectangle();
        var origin = _coinAnimator.GetOrigin();

        _spriteBatch.Draw(_coinAnimator.Texture, position, sourceRect, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    public void DrawTurret(Turret turret)
    {
        var position = ToXnaVector2(turret.Position);
        // Draw as a square - the texture is 1x1 pixel, so we scale it to the size
        var destinationRect = new Rectangle(
            (int)(position.X - turret.Size),
            (int)(position.Y - turret.Size),
            (int)(turret.Size * 2),
            (int)(turret.Size * 2)
        );
        _spriteBatch.Draw(_turretTexture, destinationRect, Color.Black);
    }

    public void DrawProjectile(Projectile projectile)
    {
        if (!projectile.IsActive) return;

        var position = ToXnaVector2(projectile.Position);
        var origin = new XnaVector2(projectile.Radius, projectile.Radius);
        _spriteBatch.Draw(_projectileTexture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
    }

    private static XnaVector2 ToXnaVector2(SysVector2 vector)
    {
        return new XnaVector2(vector.X, vector.Y);
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TurretGame.Core.Interfaces;
using SysVector2 = System.Numerics.Vector2;

namespace TurretGame.Infrastructure.Input;

public class MonoGameInputService : IInputService
{
    public SysVector2 GetMovementDirection()
    {
        var keyboardState = Keyboard.GetState();
        var direction = SysVector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
            direction.Y -= 1;
        if (keyboardState.IsKeyDown(Keys.S))
            direction.Y += 1;
        if (keyboardState.IsKeyDown(Keys.A))
            direction.X -= 1;
        if (keyboardState.IsKeyDown(Keys.D))
            direction.X += 1;

        return direction;
    }

    public bool IsExitRequested()
    {
        var keyboardState = Keyboard.GetState();
        var gamePadState = GamePad.GetState(PlayerIndex.One);

        return keyboardState.IsKeyDown(Keys.Escape) ||
               gamePadState.Buttons.Back == ButtonState.Pressed;
    }
}

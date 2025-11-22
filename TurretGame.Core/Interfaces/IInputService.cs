using System.Numerics;

namespace TurretGame.Core.Interfaces;

public interface IInputService
{
    Vector2 GetMovementDirection();
    bool IsExitRequested();
    Vector2 GetMousePosition();
    bool IsLeftMouseButtonClicked();
}

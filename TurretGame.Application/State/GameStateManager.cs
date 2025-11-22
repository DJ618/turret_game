using TurretGame.Core.State;

namespace TurretGame.Application.State;

public class GameStateManager
{
    public GameState CurrentState { get; private set; }

    public GameStateManager()
    {
        CurrentState = GameState.Playing;
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
    }

    public bool IsPlaying() => CurrentState == GameState.Playing;
    public bool IsPlacingTurret() => CurrentState == GameState.PlacingTurret;
    public bool IsGameOver() => CurrentState == GameState.GameOver;

    public void Reset()
    {
        CurrentState = GameState.Playing;
    }
}

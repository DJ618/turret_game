namespace TurretGame.Core.Utilities;

public struct Bounds
{
    public float MinX { get; }
    public float MaxX { get; }
    public float MinY { get; }
    public float MaxY { get; }

    public Bounds(float minX, float maxX, float minY, float maxY)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
    }

    public float Width => MaxX - MinX;
    public float Height => MaxY - MinY;
}

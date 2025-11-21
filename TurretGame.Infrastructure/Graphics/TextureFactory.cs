using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TurretGame.Infrastructure.Graphics;

public class TextureFactory
{
    private readonly GraphicsDevice _graphicsDevice;

    public TextureFactory(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public Texture2D CreateCircleTexture(int radius, Color color)
    {
        int diameter = radius * 2;
        var texture = new Texture2D(_graphicsDevice, diameter, diameter);
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

using Microsoft.Xna.Framework;

namespace Serene.Common.Interfaces;

public interface ICameraService
{
    // Public outputs
    public Matrix View { get; set; }
    public Matrix Projection { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }

    // Orbit params
    public float Yaw { get; set; }                   // links/rechts
    public float Pitch { get; set; }              // hoch/runter (negativ = nach unten schauen)
    public float Distance { get; set; }

    public void Resize(float aspectRatio);

    public void Update(GameTime gameTime);
}
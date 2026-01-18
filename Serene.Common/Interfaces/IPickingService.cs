using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SereneHorizons.Core.Services;

public interface IPickingService
{
    static abstract Ray GetMouseRay(GraphicsDevice gd, Matrix view, Matrix projection);

    static abstract Vector3? RayPlaneIntersection(Ray ray, float planeY);
}
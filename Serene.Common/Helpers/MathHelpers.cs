using System;
using Microsoft.Xna.Framework;
using Serene.Common.Interfaces;

namespace Serene.Common.Helpers
{
    public static class MathHelpers
    {
        /// <summary>
        /// Erzeugt einen zufälligen Punkt innerhalb eines Radius um ein Zentrum.
        /// </summary>
        /// <param name="center">Das Zentrum des Bereichs</param>
        /// <param name="radius">Der Radius des Bereichs</param>
        /// <param name="randomService">Optional: Ein eigener Zufallsgenerator (wenn null, wird der integrierte verwendet)</param>
        /// <returns>Ein zufälliger Punkt innerhalb des angegebenen Radius</returns>
        public static Vector3 GetRandomPointInRadius(Vector3 center, float radius, IRandomService randomService)
        {
            // Methode 1: Gleichmäßige Verteilung im Volumen einer Kugel
            // Zufälliger Radius (kubische Wurzel für gleichmäßige Volumenverteilung)
            float randomRadius = radius * (float)Math.Pow(randomService.NextDouble(), 1.0/3.0);
            
            // Zufällige Richtung (gleichmäßig auf der Oberfläche einer Kugel)
            float theta = (float)(randomService.NextDouble() * 2.0 * Math.PI); // Azimuth [0, 2π]
            float phi = (float)(Math.Acos(2.0 * randomService.NextDouble() - 1.0)); // Inclination [0, π]
            
            float x = randomRadius * (float)(Math.Sin(phi) * Math.Cos(theta));
            float y = randomRadius * (float)(Math.Sin(phi) * Math.Sin(theta));
            float z = randomRadius * (float)(Math.Cos(phi));
            
            return center + new Vector3(x, y, z);
        }

        /// <summary>
        /// Erzeugt einen zufälligen Punkt innerhalb eines horizontalen Kreises um ein Zentrum.
        /// </summary>
        /// <param name="center">Das Zentrum des Bereichs</param>
        /// <param name="radius">Der Radius des Bereichs</param>
        /// <param name="randomService">Optional: Ein eigener Zufallsgenerator (wenn null, wird der integrierte verwendet)</param>
        /// <returns>Ein zufälliger Punkt innerhalb des angegebenen Radius (nur X und Z werden verändert)</returns>
        public static Vector3 GetRandomPointInCircle(Vector3 center, float radius, IRandomService randomService)
        {
            // Methode 2: Gleichmäßige Verteilung in einem Kreis (für 2D)
            // Zufälliger Radius (Quadratwurzel für gleichmäßige Flächenverteilung)
            float randomRadius = radius * (float)Math.Sqrt(randomService.NextDouble());
            
            // Zufälliger Winkel
            float angle = (float)(randomService.NextDouble() * 2.0 * Math.PI);
            
            float x = randomRadius * (float)Math.Cos(angle);
            float z = randomRadius * (float)Math.Sin(angle);
            
            // Y-Wert bleibt unverändert
            return new Vector3(center.X + x, center.Y, center.Z + z);
        }

        /// <summary>
        /// Erzeugt zufällige Punkte in einem Ring um ein Zentrum
        /// </summary>
        /// <param name="center">Das Zentrum des Rings</param>
        /// <param name="innerRadius">Der innere Radius des Rings</param>
        /// <param name="outerRadius">Der äußere Radius des Rings</param>
        /// <param name="randomService">Optional: Ein eigener Zufallsgenerator (wenn null, wird der integrierte verwendet)</param>
        /// <returns>Ein zufälliger Punkt innerhalb des angegebenen Rings (nur X und Z werden verändert)</returns>
        public static Vector3 GetRandomPointInRing(Vector3 center, float innerRadius, float outerRadius, IRandomService randomService)
        {
            if (innerRadius > outerRadius)
            {
                throw new ArgumentException("Der innere Radius muss kleiner als der äußere Radius sein.");
            }
            
            // Zufälliger Radius (Quadratwurzel für gleichmäßige Flächenverteilung)
            float randomRadius = (float)Math.Sqrt(randomService.NextDouble()  * (outerRadius * outerRadius - innerRadius * innerRadius) + innerRadius * innerRadius);
            
            // Zufälliger Winkel
            float angle = (float)(randomService.NextDouble()  * 2.0 * Math.PI);
            
            float x = randomRadius * (float)Math.Cos(angle);
            float z = randomRadius * (float)Math.Sin(angle);
            
            // Y-Wert bleibt unverändert
            return new Vector3(center.X + x, center.Y, center.Z + z);
        }
    }
}
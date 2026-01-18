using System;
using Microsoft.Xna.Framework;
using SereneUI.Shared.DataStructures;

namespace SereneUI.Utilities;

public static class LayoutUtility
{
    public static Rectangle Deflate(this Rectangle rect, Thickness t)
        => new Rectangle(
            rect.X + t.Left,
            rect.Y + t.Top,
            Math.Max(0, rect.Width  - t.Horizontal),
            Math.Max(0, rect.Height - t.Vertical));

    public static Rectangle Inflate(this Rectangle rect, Thickness t)
        => new Rectangle(
            rect.X - t.Left,
            rect.Y - t.Top,
            Math.Max(0, rect.Width  + t.Horizontal),
            Math.Max(0, rect.Height + t.Vertical));

    public static Point Deflate(this Point p, Thickness t)
        => new(
            Math.Max(0, p.X - t.Horizontal),
            Math.Max(0, p.Y - t.Vertical));

    public static Point Inflate(this Point p, Thickness t)
        => new(
            Math.Max(0, p.X + t.Horizontal),
            Math.Max(0, p.Y + t.Vertical));
}
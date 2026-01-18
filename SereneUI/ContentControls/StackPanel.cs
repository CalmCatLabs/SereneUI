using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serene.Common.Extensions;
using SereneUI.Base;
using SereneUI.Shared.DataStructures;
using SereneUI.Shared.Enums;
using SereneUI.Utilities;

namespace SereneUI.ContentControls;

public class StackPanel : ItemsControlBase
{
    public Orientation Orientation { get; set; } =  Orientation.Vertical;
    
    public StackPanel()
    {
        IsVisible = true;
        IsEnabled = true;

        Padding = new Thickness(0, 0, 0, 0);
        Margin  = new Thickness(0, 0, 0, 0);

        // Diese Alignments gelten für *Panel selbst* (vom Parent zu beachten),
        // nicht zur Positionierung des Content.
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment   = VerticalAlignment.Top;
    }
    
    protected override void OnMeasure(in Point availableSize)
    {
        // Innenraum, den Children "sehen"
        var innerAvailable = availableSize.Deflate(Padding);

        int contentW = 0;
        int contentH = 0;

        foreach (var child in Children)
        {
            // Child bekommt verfügbaren Raum ohne seine Margin
            var childAvailable = innerAvailable.Deflate(child.Margin);

            // WICHTIG: Falls deine Elemente ein Measure haben, hier aufrufen:
            child.Measure(childAvailable);

            // Wenn du (noch) kein DesiredSize hast, ist child.Size ok,
            // aber dann MUSS child.Size vorher korrekt gesetzt werden (z.B. TextBlock misst Font).
            int cw = child.Size.X;
            int ch = child.Size.Y;

            int totalW = cw + child.Margin.Horizontal;
            int totalH = ch + child.Margin.Vertical;

            if (Orientation == Orientation.Horizontal)
            {
                contentW += totalW;
                contentH = Math.Max(contentH, totalH);
            }
            else
            {
                contentW = Math.Max(contentW, totalW);
                contentH += totalH;
            }
        }

        // Panelgröße ist Content + Padding
        Size = new Point(
            contentW + Padding.Horizontal,
            contentH + Padding.Vertical
        );
    }

   
protected override void OnArrange(in Rectangle finalRect)
{
    if (Children.Count < 1) return;

    // Innenraum des Panels
    var innerRect = finalRect.Deflate(Padding);

    int cursorX = innerRect.X;
    int cursorY = innerRect.Y;

    foreach (var child in Children)
    {
        if (!child.IsVisible) continue;

        int cw = child.Size.X;
        int ch = child.Size.Y;

        // "Slot" = Bereich, in dem das Kind ausgerichtet werden darf (ohne Margin)
        // Quer zur Stack-Richtung ist der Slot so groß wie der Innenraum.
        // In Stack-Richtung ist er so groß wie das Kind (damit es nicht alles überlappt).
        Rectangle slot;

        if (Orientation == Orientation.Horizontal)
        {
            int slotX = cursorX + child.Margin.Left;
            int slotY = innerRect.Y + child.Margin.Top;

            int slotW = Math.Max(0, cw); // Stack-Richtung: nur Kindbreite
            int slotH = Math.Max(0, innerRect.Height - child.Margin.Vertical); // Quer: voller Innenraum

            slot = new Rectangle(slotX, slotY, slotW, slotH);
        }
        else // Vertical
        {
            int slotX = innerRect.X + child.Margin.Left;
            int slotY = cursorY + child.Margin.Top;

            int slotW = Math.Max(0, innerRect.Width - child.Margin.Horizontal); // Quer: voller Innenraum
            int slotH = Math.Max(0, ch); // Stack-Richtung: nur Kindhöhe

            slot = new Rectangle(slotX, slotY, slotW, slotH);
        }

        // Alignment innerhalb des Slots
        int x = slot.X;
        int y = slot.Y;

        // Horizontal alignment (wirkt immer quer/innerhalb slotW)
        switch (child.HorizontalAlignment)
        {
            case HorizontalAlignment.Left:
                x = slot.X;
                break;
            case HorizontalAlignment.Center:
                x = slot.X + (slot.Width - cw) / 2;
                break;
            case HorizontalAlignment.Right:
                x = slot.Right - cw;
                break;
            case HorizontalAlignment.Stretch:
                x = slot.X;
                cw = Math.Max(0, slot.Width);
                break;
        }

        // Vertical alignment (wirkt immer quer/innerhalb slotH)
        switch (child.VerticalAlignment)
        {
            case VerticalAlignment.Top:
                y = slot.Y;
                break;
            case VerticalAlignment.Center:
                y = slot.Y + (slot.Height - ch) / 2;
                break;
            case VerticalAlignment.Bottom:
                y = slot.Bottom - ch;
                break;
            case VerticalAlignment.Stretch:
                y = slot.Y;
                ch = Math.Max(0, slot.Height);
                break;
        }

        child.Arrange(new Rectangle(x, y, Math.Max(0, cw), Math.Max(0, ch)));

        // Cursor entlang der Stack-Richtung fortschieben (inkl. Margin!)
        if (Orientation == Orientation.Horizontal)
        {
            cursorX += child.Margin.Left + cw + child.Margin.Right;
        }
        else
        {
            cursorY += child.Margin.Top + ch + child.Margin.Bottom;
        }
    }
}

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        Children.ForEach(child => child.Draw(spriteBatch));
    }
}
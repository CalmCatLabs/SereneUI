namespace SereneUI.Shared.DataStructures;

public readonly record struct Thickness(int Left, int Top, int Right, int Bottom)
{
    public int Horizontal => Left + Right;
    public int Vertical => Top + Bottom;
    
    public static readonly Thickness Zero = new(0, 0, 0, 0);
}
namespace SereneUI.Shared.DataStructures;

/// <summary>
/// Structure for handeling Thickness, for e.g. borders, margins, paddings.
/// </summary>
/// <param name="Left">Value for left width.</param>
/// <param name="Top">Value for top height.</param>
/// <param name="Right">Value for right width.</param>
/// <param name="Bottom">Value for bottom height.</param>
public readonly record struct Thickness(int Left, int Top, int Right, int Bottom)
{
    /// <summary>
    /// Returns the horizontal sum.
    /// </summary>
    public int Horizontal => Left + Right;
    
    /// <summary>
    /// Returns the vertical sum.
    /// </summary>
    public int Vertical => Top + Bottom;
    
    /// <summary>
    /// Returns a Thickness object with every value 0. 
    /// </summary>
    public static readonly Thickness Zero = new(0, 0, 0, 0);
}
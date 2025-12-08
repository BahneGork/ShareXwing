namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// Represents a floating window that can be displayed on top of other windows.
/// </summary>
public interface IFloatingWindow
{
    /// <summary>
    /// Sets the opacity of the floating window.
    /// </summary>
    /// <param name="opacity">Opacity value between 0.1 and 1.0.</param>
    void SetOpacity(double opacity);

    /// <summary>
    /// Sets whether the window is locked (click-through enabled).
    /// </summary>
    /// <param name="locked">True to enable click-through, false to allow interaction.</param>
    void SetLocked(bool locked);

    /// <summary>
    /// Closes the floating window.
    /// </summary>
    void Close();

    /// <summary>
    /// Gets the current opacity of the window.
    /// </summary>
    double CurrentOpacity { get; }

    /// <summary>
    /// Gets whether the window is currently locked.
    /// </summary>
    bool IsLocked { get; }
}

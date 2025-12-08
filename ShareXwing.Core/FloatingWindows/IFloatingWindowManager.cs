using System.Collections.Generic;

namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// Manages all floating windows in the application.
/// </summary>
public interface IFloatingWindowManager
{
    /// <summary>
    /// Registers a floating window with the manager.
    /// </summary>
    /// <param name="window">The window to register.</param>
    void RegisterWindow(IFloatingWindow window);

    /// <summary>
    /// Unregisters a floating window from the manager.
    /// </summary>
    /// <param name="window">The window to unregister.</param>
    void UnregisterWindow(IFloatingWindow window);

    /// <summary>
    /// Closes all registered floating windows.
    /// </summary>
    void CloseAll();

    /// <summary>
    /// Gets the count of active floating windows.
    /// </summary>
    int ActiveCount { get; }

    /// <summary>
    /// Gets all active floating windows.
    /// </summary>
    IEnumerable<IFloatingWindow> GetActiveWindows();
}

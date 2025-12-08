using System.Collections.Generic;
using System.Linq;

namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// Default implementation of <see cref="IFloatingWindowManager"/>.
/// </summary>
public class FloatingWindowManager : IFloatingWindowManager
{
    private readonly List<IFloatingWindow> activeWindows = new();
    private readonly object lockObject = new();

    /// <inheritdoc/>
    public int ActiveCount
    {
        get
        {
            lock (lockObject)
            {
                return activeWindows.Count;
            }
        }
    }

    /// <inheritdoc/>
    public void RegisterWindow(IFloatingWindow window)
    {
        if (window == null)
        {
            throw new System.ArgumentNullException(nameof(window));
        }

        lock (lockObject)
        {
            if (!activeWindows.Contains(window))
            {
                activeWindows.Add(window);
            }
        }
    }

    /// <inheritdoc/>
    public void UnregisterWindow(IFloatingWindow window)
    {
        if (window == null)
        {
            throw new System.ArgumentNullException(nameof(window));
        }

        lock (lockObject)
        {
            activeWindows.Remove(window);
        }
    }

    /// <inheritdoc/>
    public void CloseAll()
    {
        List<IFloatingWindow> windowsCopy;

        lock (lockObject)
        {
            windowsCopy = new List<IFloatingWindow>(activeWindows);
            activeWindows.Clear();
        }

        foreach (var window in windowsCopy)
        {
            window.Close();
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IFloatingWindow> GetActiveWindows()
    {
        lock (lockObject)
        {
            return new List<IFloatingWindow>(activeWindows);
        }
    }
}

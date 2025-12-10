using System;

namespace ShareXwing.Core.Win32;

/// <summary>
/// Helper class for Win32 window operations.
/// </summary>
public static class WindowHelper
{
    /// <summary>
    /// Sets a window to always-on-top.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <param name="topMost">True to make topmost, false to remove topmost.</param>
    /// <returns>True if successful.</returns>
    public static bool SetTopMost(IntPtr handle, bool topMost)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle", nameof(handle));
        }

        IntPtr hWndInsertAfter = topMost ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST;

        return NativeMethods.SetWindowPos(
            handle,
            hWndInsertAfter,
            0,
            0,
            0,
            0,
            NativeMethods.SWP.NOMOVE | NativeMethods.SWP.NOSIZE | NativeMethods.SWP.NOACTIVATE);
    }

    /// <summary>
    /// Sets window opacity.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <param name="opacity">Opacity value between 0.0 and 1.0.</param>
    /// <returns>True if successful.</returns>
    public static bool SetOpacity(IntPtr handle, double opacity)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle", nameof(handle));
        }

        if (opacity < 0.0 || opacity > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity), "Opacity must be between 0.0 and 1.0");
        }

        // Ensure window has WS_EX_LAYERED style
        IntPtr exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_EXSTYLE);
        if ((exStyle.ToInt32() & NativeMethods.WS_EX_LAYERED) == 0)
        {
            NativeMethods.SetWindowLong(
                handle,
                NativeMethods.GWL_EXSTYLE,
                new IntPtr(exStyle.ToInt32() | NativeMethods.WS_EX_LAYERED));
        }

        // Set alpha value (0-255)
        byte alpha = (byte)(opacity * 255);
        return NativeMethods.SetLayeredWindowAttributes(handle, 0, alpha, NativeMethods.LWA_ALPHA);
    }

    /// <summary>
    /// Sets window click-through (transparent to mouse input).
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <param name="clickThrough">True to enable click-through, false to disable.</param>
    /// <returns>True if successful.</returns>
    public static bool SetClickThrough(IntPtr handle, bool clickThrough)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle", nameof(handle));
        }

        IntPtr exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_EXSTYLE);
        int newStyle;

        if (clickThrough)
        {
            // Add WS_EX_TRANSPARENT flag
            newStyle = exStyle.ToInt32() | NativeMethods.WS_EX_TRANSPARENT;
        }
        else
        {
            // Remove WS_EX_TRANSPARENT flag
            newStyle = exStyle.ToInt32() & ~NativeMethods.WS_EX_TRANSPARENT;
        }

        IntPtr result = NativeMethods.SetWindowLong(handle, NativeMethods.GWL_EXSTYLE, new IntPtr(newStyle));
        return result != IntPtr.Zero;
    }

    /// <summary>
    /// Gets whether a window has click-through enabled.
    /// </summary>
    /// <param name="handle">Window handle.</param>
    /// <returns>True if click-through is enabled.</returns>
    public static bool GetClickThrough(IntPtr handle)
    {
        if (handle == IntPtr.Zero)
        {
            throw new ArgumentException("Invalid window handle", nameof(handle));
        }

        IntPtr exStyle = NativeMethods.GetWindowLong(handle, NativeMethods.GWL_EXSTYLE);
        return (exStyle.ToInt32() & NativeMethods.WS_EX_TRANSPARENT) != 0;
    }
}

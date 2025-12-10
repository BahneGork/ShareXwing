using System;
using System.Runtime.InteropServices;

namespace ShareXwing.Core.Win32;

/// <summary>
/// P/Invoke declarations for Win32 API functions.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Places the window above all non-topmost windows.
    /// </summary>
    public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    /// <summary>
    /// Places the window above all non-topmost windows (removes always-on-top).
    /// </summary>
    public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

    /// <summary>
    /// SetWindowPos flags.
    /// </summary>
    [Flags]
    public enum SWP : uint
    {
        NOSIZE = 0x0001,
        NOMOVE = 0x0002,
        NOZORDER = 0x0004,
        NOREDRAW = 0x0008,
        NOACTIVATE = 0x0010,
        FRAMECHANGED = 0x0020,
        SHOWWINDOW = 0x0040,
        HIDEWINDOW = 0x0080,
        NOCOPYBITS = 0x0100,
        NOOWNERZORDER = 0x0200,
        NOSENDCHANGING = 0x0400,
    }

    /// <summary>
    /// GetWindowLong/SetWindowLong index for extended window styles.
    /// </summary>
    public const int GWL_EXSTYLE = -20;

    /// <summary>
    /// Extended window style: Layered window (required for opacity).
    /// </summary>
    public const int WS_EX_LAYERED = 0x00080000;

    /// <summary>
    /// Extended window style: Transparent to mouse input (click-through).
    /// </summary>
    public const int WS_EX_TRANSPARENT = 0x00000020;

    /// <summary>
    /// LayeredWindowAttributes flag: Use alpha channel.
    /// </summary>
    public const int LWA_ALPHA = 0x00000002;

    /// <summary>
    /// Changes the size, position, and Z order of a window.
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        SWP uFlags);

    /// <summary>
    /// Retrieves information about the specified window (32-bit).
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    /// <summary>
    /// Retrieves information about the specified window (64-bit).
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    public static extern IntPtr GetWindowLong64(IntPtr hWnd, int nIndex);

    /// <summary>
    /// Changes an attribute of the specified window (32-bit).
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    public static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    /// <summary>
    /// Changes an attribute of the specified window (64-bit).
    /// </summary>
    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    public static extern IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    /// <summary>
    /// Sets the opacity and transparency color key of a layered window.
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetLayeredWindowAttributes(
        IntPtr hwnd,
        uint crKey,
        byte bAlpha,
        uint dwFlags);

    /// <summary>
    /// Platform-independent GetWindowLong wrapper.
    /// </summary>
    public static IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 8)
        {
            return GetWindowLong64(hWnd, nIndex);
        }
        else
        {
            return new IntPtr(GetWindowLong32(hWnd, nIndex));
        }
    }

    /// <summary>
    /// Platform-independent SetWindowLong wrapper.
    /// </summary>
    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        if (IntPtr.Size == 8)
        {
            return SetWindowLong64(hWnd, nIndex, dwNewLong);
        }
        else
        {
            return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }
    }
}

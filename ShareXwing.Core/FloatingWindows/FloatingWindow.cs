using System;
using System.Drawing;
using System.Windows.Forms;
using ShareXwing.Core.Win32;

namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// A floating window that displays an image always-on-top with opacity and lock controls.
/// </summary>
public partial class FloatingWindow : Form, IFloatingWindow
{
    private const double MinOpacityValue = 0.1;
    private const double MaxOpacityValue = 1.0;
    private const double DefaultOpacityValue = 1.0;

    private double currentOpacity = DefaultOpacityValue;
    private bool isLocked;
    private readonly Image displayImage;
    private Point dragStartPoint;
    private bool isDragging;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatingWindow"/> class.
    /// </summary>
    /// <param name="image">The image to display in the floating window.</param>
    public FloatingWindow(Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        displayImage = image;
        InitializeComponent();
        InitializeWindow();
    }

    /// <inheritdoc/>
    public double CurrentOpacity
    {
        get => currentOpacity;
        private set
        {
            currentOpacity = Math.Clamp(value, MinOpacityValue, MaxOpacityValue);
            UpdateOpacity();
        }
    }

    /// <inheritdoc/>
    public bool IsLocked
    {
        get => isLocked;
        private set
        {
            isLocked = value;
            UpdateLockState();
        }
    }

    /// <inheritdoc/>
    public void SetOpacity(double opacity)
    {
        CurrentOpacity = opacity;
    }

    /// <inheritdoc/>
    public void SetLocked(bool locked)
    {
        IsLocked = locked;
    }

    /// <summary>
    /// Initializes the window properties and appearance.
    /// </summary>
    private void InitializeComponent()
    {
        SuspendLayout();

        // Window properties
        FormBorderStyle = FormBorderStyle.SizableToolWindow;
        Text = "Floating Screenshot";
        StartPosition = FormStartPosition.CenterScreen;
        ShowInTaskbar = false;
        BackColor = Color.Black;
        BackgroundImage = displayImage;
        BackgroundImageLayout = ImageLayout.Zoom;

        // Size to image with reasonable constraints
        int width = Math.Min(Math.Max(displayImage.Width, 200), Screen.PrimaryScreen.WorkingArea.Width - 100);
        int height = Math.Min(Math.Max(displayImage.Height, 150), Screen.PrimaryScreen.WorkingArea.Height - 100);
        ClientSize = new Size(width, height);

        // Mouse events for dragging
        MouseDown += FloatingWindow_MouseDown;
        MouseMove += FloatingWindow_MouseMove;
        MouseUp += FloatingWindow_MouseUp;

        ResumeLayout(false);
    }

    /// <summary>
    /// Initializes the window after creation.
    /// </summary>
    private void InitializeWindow()
    {
        // Calculate aspect ratio for resize
        CalculateAspectRatio();

        // Create context menu
        CreateContextMenu();

        // Set always-on-top
        WindowHelper.SetTopMost(Handle, true);

        // Set initial opacity
        UpdateOpacity();
    }

    /// <summary>
    /// Updates the window opacity using Win32 API.
    /// </summary>
    private void UpdateOpacity()
    {
        if (Handle != IntPtr.Zero && IsHandleCreated)
        {
            WindowHelper.SetOpacity(Handle, currentOpacity);
        }
    }

    /// <summary>
    /// Updates the lock state (click-through).
    /// </summary>
    private void UpdateLockState()
    {
        if (Handle != IntPtr.Zero && IsHandleCreated)
        {
            WindowHelper.SetClickThrough(Handle, isLocked);
        }

        UpdateLockMenuCheck();
    }

    /// <summary>
    /// Handles mouse down event for dragging.
    /// </summary>
    private void FloatingWindow_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!isLocked && e.Button == MouseButtons.Left)
        {
            isDragging = true;
            dragStartPoint = e.Location;
            Cursor = Cursors.SizeAll;
        }
    }

    /// <summary>
    /// Handles mouse move event for dragging.
    /// </summary>
    private void FloatingWindow_MouseMove(object? sender, MouseEventArgs e)
    {
        if (isDragging && !isLocked)
        {
            Point screenPoint = PointToScreen(e.Location);
            Location = new Point(
                screenPoint.X - dragStartPoint.X,
                screenPoint.Y - dragStartPoint.Y);
        }
    }

    /// <summary>
    /// Handles mouse up event to stop dragging.
    /// </summary>
    private void FloatingWindow_MouseUp(object? sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            isDragging = false;
            Cursor = Cursors.Default;
        }
    }

    /// <summary>
    /// Overrides OnHandleCreated to apply window styles.
    /// </summary>
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // Ensure always-on-top and opacity are applied
        WindowHelper.SetTopMost(Handle, true);
        UpdateOpacity();

        if (isLocked)
        {
            UpdateLockState();
        }
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            displayImage?.Dispose();
        }

        base.Dispose(disposing);
    }
}

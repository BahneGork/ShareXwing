using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// Resize functionality for FloatingWindow with aspect ratio preservation.
/// </summary>
public partial class FloatingWindow
{
    private double aspectRatio;
    private bool preserveAspectRatio = true;

    /// <summary>
    /// Gets or sets whether to preserve aspect ratio when resizing.
    /// </summary>
    public bool PreserveAspectRatio
    {
        get => preserveAspectRatio;
        set => preserveAspectRatio = value;
    }

    /// <summary>
    /// Calculates and stores the aspect ratio.
    /// </summary>
    private void CalculateAspectRatio()
    {
        if (displayImage != null && displayImage.Height > 0)
        {
            aspectRatio = (double)displayImage.Width / displayImage.Height;
        }
        else
        {
            aspectRatio = 1.0;
        }
    }

    /// <summary>
    /// Overrides OnResizeEnd to preserve aspect ratio.
    /// </summary>
    protected override void OnResizeEnd(EventArgs e)
    {
        base.OnResizeEnd(e);

        if (preserveAspectRatio && !isLocked)
        {
            AdjustSizeToAspectRatio();
        }
    }

    /// <summary>
    /// Adjusts the window size to maintain aspect ratio.
    /// </summary>
    private void AdjustSizeToAspectRatio()
    {
        if (aspectRatio <= 0)
        {
            CalculateAspectRatio();
        }

        int currentWidth = ClientSize.Width;
        int currentHeight = ClientSize.Height;

        // Calculate what the height should be for the current width
        int targetHeight = (int)(currentWidth / aspectRatio);

        // If the difference is significant, adjust
        if (Math.Abs(targetHeight - currentHeight) > 5)
        {
            ClientSize = new Size(currentWidth, targetHeight);
        }
    }

    /// <summary>
    /// Overrides OnSizeChanged to update during resize.
    /// </summary>
    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);

        // Update display
        Invalidate();
    }
}

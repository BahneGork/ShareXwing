using System;

namespace ShareXwing.Core.Capture
{
    /// <summary>
    /// Capture mode options
    /// </summary>
    public enum CaptureMode
    {
        None,
        Region,
        Window,
        Fullscreen,
        ActiveMonitor,
        LastRegion,
        ScrollingCapture
    }

    /// <summary>
    /// Interface for All-in-One Capture selector overlay
    /// </summary>
    public interface ICaptureSelector : IDisposable
    {
        /// <summary>
        /// Shows the capture selector at the cursor position
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the capture selector
        /// </summary>
        void Hide();

        /// <summary>
        /// Gets whether the selector is currently visible
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Occurs when a capture mode is selected
        /// </summary>
        event EventHandler<CaptureMode>? CaptureModeSelected;

        /// <summary>
        /// Occurs when the selector is cancelled (ESC or click outside)
        /// </summary>
        event EventHandler? Cancelled;
    }
}

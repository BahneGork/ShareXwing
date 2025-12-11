using System;
using System.Drawing;

namespace ShareXwing.Core.QuickAccess
{
    /// <summary>
    /// Interface for managing Quick Access Overlay instances
    /// </summary>
    public interface IQuickAccessManager
    {
        /// <summary>
        /// Shows the Quick Access Overlay with the specified image
        /// </summary>
        /// <param name="image">The captured image to display</param>
        /// <param name="location">Screen location to display the overlay (use cursor position if default)</param>
        void ShowOverlay(Image image, Point? location = null);

        /// <summary>
        /// Hides the current Quick Access Overlay if visible
        /// </summary>
        void HideOverlay();

        /// <summary>
        /// Gets whether an overlay is currently visible
        /// </summary>
        bool IsOverlayVisible { get; }

        /// <summary>
        /// Gets or sets the auto-dismiss timeout in milliseconds (0 = no auto-dismiss)
        /// </summary>
        int AutoDismissTimeout { get; set; }

        /// <summary>
        /// Occurs when the user clicks the Copy button
        /// </summary>
        event EventHandler<Image>? CopyRequested;

        /// <summary>
        /// Occurs when the user clicks the Save button
        /// </summary>
        event EventHandler<Image>? SaveRequested;

        /// <summary>
        /// Occurs when the user clicks the Pin button
        /// </summary>
        event EventHandler<Image>? PinRequested;

        /// <summary>
        /// Occurs when the user clicks the Upload button
        /// </summary>
        event EventHandler<Image>? UploadRequested;

        /// <summary>
        /// Occurs when the user clicks the Annotate button
        /// </summary>
        event EventHandler<Image>? AnnotateRequested;

        /// <summary>
        /// Occurs when the overlay is closed or dismissed
        /// </summary>
        event EventHandler? OverlayClosed;
    }
}

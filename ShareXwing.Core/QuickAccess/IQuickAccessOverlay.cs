using System;
using System.Drawing;

namespace ShareXwing.Core.QuickAccess
{
    /// <summary>
    /// Interface for Quick Access Overlay - post-capture action panel
    /// </summary>
    public interface IQuickAccessOverlay : IDisposable
    {
        /// <summary>
        /// Shows the overlay at the specified location with the given image
        /// </summary>
        /// <param name="image">The captured image to preview</param>
        /// <param name="location">Screen location to display the overlay</param>
        void Show(Image image, Point location);

        /// <summary>
        /// Hides the overlay
        /// </summary>
        void Hide();

        /// <summary>
        /// Gets or sets the auto-dismiss timeout in milliseconds (0 = no auto-dismiss)
        /// </summary>
        int AutoDismissTimeout { get; set; }

        /// <summary>
        /// Occurs when the user clicks the Copy button
        /// </summary>
        event EventHandler? CopyClicked;

        /// <summary>
        /// Occurs when the user clicks the Save button
        /// </summary>
        event EventHandler? SaveClicked;

        /// <summary>
        /// Occurs when the user clicks the Pin button
        /// </summary>
        event EventHandler? PinClicked;

        /// <summary>
        /// Occurs when the user clicks the Upload button
        /// </summary>
        event EventHandler? UploadClicked;

        /// <summary>
        /// Occurs when the user clicks the Annotate button
        /// </summary>
        event EventHandler? AnnotateClicked;

        /// <summary>
        /// Occurs when the user clicks the Close button or the overlay is dismissed
        /// </summary>
        event EventHandler? Closed;

        /// <summary>
        /// Gets the captured image being displayed
        /// </summary>
        Image? CurrentImage { get; }
    }
}

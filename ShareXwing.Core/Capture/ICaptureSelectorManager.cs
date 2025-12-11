using System;

namespace ShareXwing.Core.Capture
{
    /// <summary>
    /// Interface for managing the All-in-One Capture selector
    /// </summary>
    public interface ICaptureSelectorManager
    {
        /// <summary>
        /// Shows the capture selector
        /// </summary>
        void ShowSelector();

        /// <summary>
        /// Hides the capture selector
        /// </summary>
        void HideSelector();

        /// <summary>
        /// Gets whether the selector is currently visible
        /// </summary>
        bool IsSelectorVisible { get; }

        /// <summary>
        /// Occurs when a capture mode is selected
        /// </summary>
        event EventHandler<CaptureMode>? CaptureModeSelected;

        /// <summary>
        /// Occurs when the selector is cancelled
        /// </summary>
        event EventHandler? SelectorCancelled;
    }
}

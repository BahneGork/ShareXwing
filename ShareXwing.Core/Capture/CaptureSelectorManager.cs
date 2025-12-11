using System;

namespace ShareXwing.Core.Capture
{
    /// <summary>
    /// Manages the All-in-One Capture selector lifecycle
    /// </summary>
    public class CaptureSelectorManager : ICaptureSelectorManager
    {
        private ICaptureSelector? currentSelector;

        /// <inheritdoc/>
        public bool IsSelectorVisible => currentSelector != null && currentSelector.IsVisible;

        /// <inheritdoc/>
        public event EventHandler<CaptureMode>? CaptureModeSelected;

        /// <inheritdoc/>
        public event EventHandler? SelectorCancelled;

        /// <inheritdoc/>
        public void ShowSelector()
        {
            // Hide existing selector if any
            HideSelector();

            // Create new selector
            currentSelector = new CaptureSelector();

            // Wire up events
            currentSelector.CaptureModeSelected += CurrentSelector_CaptureModeSelected;
            currentSelector.Cancelled += CurrentSelector_Cancelled;

            // Show selector
            currentSelector.Show();
        }

        /// <inheritdoc/>
        public void HideSelector()
        {
            if (currentSelector != null)
            {
                UnwireSelectorEvents(currentSelector);
                currentSelector.Hide();
                currentSelector.Dispose();
                currentSelector = null;
            }
        }

        private void UnwireSelectorEvents(ICaptureSelector selector)
        {
            selector.CaptureModeSelected -= CurrentSelector_CaptureModeSelected;
            selector.Cancelled -= CurrentSelector_Cancelled;
        }

        private void CurrentSelector_CaptureModeSelected(object? sender, CaptureMode mode)
        {
            CaptureModeSelected?.Invoke(this, mode);

            if (currentSelector != null)
            {
                UnwireSelectorEvents(currentSelector);
                currentSelector.Dispose();
                currentSelector = null;
            }
        }

        private void CurrentSelector_Cancelled(object? sender, EventArgs e)
        {
            SelectorCancelled?.Invoke(this, EventArgs.Empty);

            if (currentSelector != null)
            {
                UnwireSelectorEvents(currentSelector);
                currentSelector.Dispose();
                currentSelector = null;
            }
        }
    }
}

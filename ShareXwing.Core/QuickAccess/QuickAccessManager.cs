using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareXwing.Core.QuickAccess
{
    /// <summary>
    /// Manages Quick Access Overlay instances and coordinates user actions
    /// </summary>
    public class QuickAccessManager : IQuickAccessManager
    {
        private const int DefaultAutoDismissTimeout = 5000; // 5 seconds

        private IQuickAccessOverlay? currentOverlay;
        private int autoDismissTimeout = DefaultAutoDismissTimeout;

        /// <inheritdoc/>
        public bool IsOverlayVisible => currentOverlay != null;

        /// <inheritdoc/>
        public int AutoDismissTimeout
        {
            get => autoDismissTimeout;
            set
            {
                autoDismissTimeout = value;
                if (currentOverlay != null)
                {
                    currentOverlay.AutoDismissTimeout = value;
                }
            }
        }

        /// <inheritdoc/>
        public event EventHandler<Image>? CopyRequested;

        /// <inheritdoc/>
        public event EventHandler<Image>? SaveRequested;

        /// <inheritdoc/>
        public event EventHandler<Image>? PinRequested;

        /// <inheritdoc/>
        public event EventHandler<Image>? UploadRequested;

        /// <inheritdoc/>
        public event EventHandler<Image>? AnnotateRequested;

        /// <inheritdoc/>
        public event EventHandler? OverlayClosed;

        /// <inheritdoc/>
        public void ShowOverlay(Image image, Point? location = null)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            // Hide existing overlay if any
            HideOverlay();

            // Determine location (use cursor position if not specified)
            Point displayLocation = location ?? Cursor.Position;

            // Create new overlay
            currentOverlay = new QuickAccessOverlay
            {
                AutoDismissTimeout = autoDismissTimeout
            };

            // Wire up events
            currentOverlay.CopyClicked += CurrentOverlay_CopyClicked;
            currentOverlay.SaveClicked += CurrentOverlay_SaveClicked;
            currentOverlay.PinClicked += CurrentOverlay_PinClicked;
            currentOverlay.UploadClicked += CurrentOverlay_UploadClicked;
            currentOverlay.AnnotateClicked += CurrentOverlay_AnnotateClicked;
            currentOverlay.Closed += CurrentOverlay_Closed;

            // Show overlay
            currentOverlay.Show(image, displayLocation);
        }

        /// <inheritdoc/>
        public void HideOverlay()
        {
            if (currentOverlay != null)
            {
                UnwireOverlayEvents(currentOverlay);
                currentOverlay.Hide();
                currentOverlay.Dispose();
                currentOverlay = null;
            }
        }

        private void UnwireOverlayEvents(IQuickAccessOverlay overlay)
        {
            overlay.CopyClicked -= CurrentOverlay_CopyClicked;
            overlay.SaveClicked -= CurrentOverlay_SaveClicked;
            overlay.PinClicked -= CurrentOverlay_PinClicked;
            overlay.UploadClicked -= CurrentOverlay_UploadClicked;
            overlay.AnnotateClicked -= CurrentOverlay_AnnotateClicked;
            overlay.Closed -= CurrentOverlay_Closed;
        }

        private void CurrentOverlay_CopyClicked(object? sender, EventArgs e)
        {
            if (currentOverlay?.CurrentImage != null)
            {
                CopyRequested?.Invoke(this, currentOverlay.CurrentImage);
            }
        }

        private void CurrentOverlay_SaveClicked(object? sender, EventArgs e)
        {
            if (currentOverlay?.CurrentImage != null)
            {
                SaveRequested?.Invoke(this, currentOverlay.CurrentImage);
            }
        }

        private void CurrentOverlay_PinClicked(object? sender, EventArgs e)
        {
            if (currentOverlay?.CurrentImage != null)
            {
                PinRequested?.Invoke(this, currentOverlay.CurrentImage);
            }
        }

        private void CurrentOverlay_UploadClicked(object? sender, EventArgs e)
        {
            if (currentOverlay?.CurrentImage != null)
            {
                UploadRequested?.Invoke(this, currentOverlay.CurrentImage);
            }
        }

        private void CurrentOverlay_AnnotateClicked(object? sender, EventArgs e)
        {
            if (currentOverlay?.CurrentImage != null)
            {
                AnnotateRequested?.Invoke(this, currentOverlay.CurrentImage);
            }
        }

        private void CurrentOverlay_Closed(object? sender, EventArgs e)
        {
            OverlayClosed?.Invoke(this, EventArgs.Empty);
            if (currentOverlay != null)
            {
                UnwireOverlayEvents(currentOverlay);
                currentOverlay.Dispose();
                currentOverlay = null;
            }
        }
    }
}

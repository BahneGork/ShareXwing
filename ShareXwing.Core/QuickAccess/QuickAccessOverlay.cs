using System;
using System.Drawing;
using System.Windows.Forms;
using ShareXwing.Core.Win32;

namespace ShareXwing.Core.QuickAccess
{
    /// <summary>
    /// Quick Access Overlay - Post-capture action panel with thumbnail preview
    /// </summary>
    public partial class QuickAccessOverlay : Form, IQuickAccessOverlay
    {
        private const int DefaultThumbnailWidth = 300;
        private const int DefaultThumbnailHeight = 200;
        private const int DefaultAutoDismissTimeout = 5000; // 5 seconds
        private const int ButtonHeight = 40;
        private const int ButtonSpacing = 8;
        private const int Padding = 12;

        private Image? currentImage;
        private PictureBox? thumbnailBox;
        private Panel? actionPanel;
        private Timer? autoDismissTimer;
        private int autoDismissTimeout = DefaultAutoDismissTimeout;

        /// <summary>
        /// Initializes a new instance of the QuickAccessOverlay class
        /// </summary>
        public QuickAccessOverlay()
        {
            InitializeComponent();
            InitializeOverlay();
        }

        /// <inheritdoc/>
        public int AutoDismissTimeout
        {
            get => autoDismissTimeout;
            set
            {
                autoDismissTimeout = value;
                if (autoDismissTimer != null)
                {
                    autoDismissTimer.Interval = value > 0 ? value : 1;
                    autoDismissTimer.Enabled = value > 0;
                }
            }
        }

        /// <inheritdoc/>
        public Image? CurrentImage => currentImage;

        /// <inheritdoc/>
        public event EventHandler? CopyClicked;

        /// <inheritdoc/>
        public event EventHandler? SaveClicked;

        /// <inheritdoc/>
        public event EventHandler? PinClicked;

        /// <inheritdoc/>
        public event EventHandler? UploadClicked;

        /// <inheritdoc/>
        public event EventHandler? AnnotateClicked;

        /// <inheritdoc/>
        public event EventHandler? Closed;

        /// <inheritdoc/>
        public void Show(Image image, Point location)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            currentImage = image;

            // Update thumbnail
            if (thumbnailBox != null)
            {
                thumbnailBox.Image = CreateThumbnail(image);
            }

            // Position overlay
            PositionOverlay(location);

            // Show and activate
            base.Show();
            WindowHelper.SetTopMost(Handle, true);

            // Start auto-dismiss timer
            ResetAutoDismissTimer();
        }

        /// <inheritdoc/>
        public new void Hide()
        {
            StopAutoDismissTimer();
            base.Hide();
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes the overlay window components
        /// </summary>
        private void InitializeComponent()
        {
            // Form properties
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(240, 240, 240);
            Opacity = 0.95;
            Size = new Size(DefaultThumbnailWidth + (Padding * 2), DefaultThumbnailHeight + ButtonHeight + (Padding * 3) + ButtonSpacing);

            // Thumbnail picture box
            thumbnailBox = new PictureBox
            {
                Location = new Point(Padding, Padding),
                Size = new Size(DefaultThumbnailWidth, DefaultThumbnailHeight),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(thumbnailBox);

            // Action panel
            actionPanel = new Panel
            {
                Location = new Point(Padding, thumbnailBox.Bottom + ButtonSpacing),
                Size = new Size(DefaultThumbnailWidth, ButtonHeight),
                BackColor = Color.Transparent
            };
            Controls.Add(actionPanel);

            // Create action buttons
            CreateActionButtons();

            // Auto-dismiss timer
            autoDismissTimer = new Timer
            {
                Interval = DefaultAutoDismissTimeout
            };
            autoDismissTimer.Tick += AutoDismissTimer_Tick;

            // Mouse events for hover detection
            MouseEnter += QuickAccessOverlay_MouseEnter;
            MouseLeave += QuickAccessOverlay_MouseLeave;
            thumbnailBox.MouseEnter += QuickAccessOverlay_MouseEnter;
            thumbnailBox.MouseLeave += QuickAccessOverlay_MouseLeave;
            actionPanel.MouseEnter += QuickAccessOverlay_MouseEnter;
            actionPanel.MouseLeave += QuickAccessOverlay_MouseLeave;

            // Deactivate event for click-outside-to-dismiss
            Deactivate += QuickAccessOverlay_Deactivate;
        }

        /// <summary>
        /// Initializes the overlay after components are created
        /// </summary>
        private void InitializeOverlay()
        {
            // Additional initialization if needed
        }

        /// <summary>
        /// Creates the action buttons
        /// </summary>
        private void CreateActionButtons()
        {
            if (actionPanel == null) return;

            int buttonWidth = 50;
            int x = 0;

            // Copy button
            var copyButton = CreateButton("Copy", x);
            copyButton.Click += (s, e) => CopyClicked?.Invoke(this, EventArgs.Empty);
            actionPanel.Controls.Add(copyButton);
            x += buttonWidth + ButtonSpacing;

            // Save button
            var saveButton = CreateButton("Save", x);
            saveButton.Click += (s, e) => SaveClicked?.Invoke(this, EventArgs.Empty);
            actionPanel.Controls.Add(saveButton);
            x += buttonWidth + ButtonSpacing;

            // Pin button
            var pinButton = CreateButton("Pin", x);
            pinButton.Click += (s, e) => PinClicked?.Invoke(this, EventArgs.Empty);
            actionPanel.Controls.Add(pinButton);
            x += buttonWidth + ButtonSpacing;

            // Upload button
            var uploadButton = CreateButton("Upload", x);
            uploadButton.Click += (s, e) => UploadClicked?.Invoke(this, EventArgs.Empty);
            actionPanel.Controls.Add(uploadButton);
            x += buttonWidth + ButtonSpacing;

            // Annotate button
            var annotateButton = CreateButton("Edit", x);
            annotateButton.Click += (s, e) => AnnotateClicked?.Invoke(this, EventArgs.Empty);
            actionPanel.Controls.Add(annotateButton);
            x += buttonWidth + ButtonSpacing;

            // Close button
            var closeButton = CreateButton("✕", x);
            closeButton.Click += (s, e) => Hide();
            actionPanel.Controls.Add(closeButton);
        }

        /// <summary>
        /// Creates a button with consistent styling
        /// </summary>
        private Button CreateButton(string text, int x)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, 0),
                Size = new Size(50, ButtonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
        }

        /// <summary>
        /// Creates a thumbnail of the given image
        /// </summary>
        private Image CreateThumbnail(Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            // Calculate aspect-ratio-preserving size
            double imageAspect = (double)image.Width / image.Height;
            double thumbAspect = (double)DefaultThumbnailWidth / DefaultThumbnailHeight;

            int thumbWidth, thumbHeight;
            if (imageAspect > thumbAspect)
            {
                // Image is wider than thumbnail
                thumbWidth = DefaultThumbnailWidth;
                thumbHeight = (int)(DefaultThumbnailWidth / imageAspect);
            }
            else
            {
                // Image is taller than thumbnail
                thumbHeight = DefaultThumbnailHeight;
                thumbWidth = (int)(DefaultThumbnailHeight * imageAspect);
            }

            var thumbnail = new Bitmap(thumbWidth, thumbHeight);
            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, thumbWidth, thumbHeight);
            }

            return thumbnail;
        }

        /// <summary>
        /// Positions the overlay at the specified location with screen edge detection
        /// </summary>
        private void PositionOverlay(Point location)
        {
            // Get screen bounds
            Screen screen = Screen.FromPoint(location);
            Rectangle screenBounds = screen.WorkingArea;

            // Calculate position (default: bottom-right of cursor)
            int x = location.X + 20;
            int y = location.Y + 20;

            // Check right edge
            if (x + Width > screenBounds.Right)
            {
                x = location.X - Width - 20;
            }

            // Check bottom edge
            if (y + Height > screenBounds.Bottom)
            {
                y = location.Y - Height - 20;
            }

            // Check left edge
            if (x < screenBounds.Left)
            {
                x = screenBounds.Left + 10;
            }

            // Check top edge
            if (y < screenBounds.Top)
            {
                y = screenBounds.Top + 10;
            }

            Location = new Point(x, y);
        }

        /// <summary>
        /// Resets the auto-dismiss timer
        /// </summary>
        private void ResetAutoDismissTimer()
        {
            if (autoDismissTimer == null || autoDismissTimeout <= 0) return;

            autoDismissTimer.Stop();
            autoDismissTimer.Start();
        }

        /// <summary>
        /// Stops the auto-dismiss timer
        /// </summary>
        private void StopAutoDismissTimer()
        {
            autoDismissTimer?.Stop();
        }

        private void AutoDismissTimer_Tick(object? sender, EventArgs e)
        {
            Hide();
        }

        private void QuickAccessOverlay_MouseEnter(object? sender, EventArgs e)
        {
            // Stop auto-dismiss when hovering
            StopAutoDismissTimer();
        }

        private void QuickAccessOverlay_MouseLeave(object? sender, EventArgs e)
        {
            // Restart auto-dismiss when leaving
            ResetAutoDismissTimer();
        }

        private void QuickAccessOverlay_Deactivate(object? sender, EventArgs e)
        {
            // Hide when clicking outside (user clicked somewhere else)
            Hide();
        }

        /// <summary>
        /// Disposes resources used by the overlay
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                autoDismissTimer?.Dispose();
                thumbnailBox?.Image?.Dispose();
                currentImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

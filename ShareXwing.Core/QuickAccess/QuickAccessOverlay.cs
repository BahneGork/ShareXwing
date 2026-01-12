using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ShareXwing.Core.Win32;

namespace ShareXwing.Core.QuickAccess
{
    /// <summary>
    /// CleanShot X-style Quick Access Overlay - Minimal post-capture card with hover buttons
    /// </summary>
    public partial class QuickAccessOverlay : Form, IQuickAccessOverlay
    {
        private const int CardWidth = 280;
        private const int CardHeight = 200;
        private const int CardPadding = 12;
        private const int CornerRadius = 16;
        private const int ButtonSize = 36;
        private const int PillButtonWidth = 100;
        private const int PillButtonHeight = 36;
        private const int PillSpacing = 8;
        private const int DefaultAutoDismissTimeout = 8000; // 8 seconds

        private Image? currentImage;
        private Image? thumbnailImage;
        private Timer? autoDismissTimer;
        private int autoDismissTimeout = DefaultAutoDismissTimeout;
        private bool isHovering = false;

        // Buttons (created on hover)
        private Button? btnPin;
        private Button? btnClose;
        private Button? btnUpload;
        private Button? btnEdit;
        private Button? btnCopy;
        private Button? btnSave;

        /// <summary>
        /// Initializes a new instance of the QuickAccessOverlay class
        /// </summary>
        public QuickAccessOverlay()
        {
            InitializeComponent();
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
            thumbnailImage = CreateThumbnail(image);

            // Position in bottom-right corner of screen
            PositionInCorner();

            // Show and activate
            base.Show();
            WindowHelper.SetTopMost(Handle, true);

            // Enable drag-and-drop
            AllowDrop = false; // We're dragging FROM this window, not TO it
            MouseDown += Thumbnail_MouseDown;

            // Start auto-dismiss timer
            ResetAutoDismissTimer();

            Invalidate();
        }

        /// <inheritdoc/>
        public new void Hide()
        {
            StopAutoDismissTimer();
            base.Hide();
            Closed?.Invoke(this, EventArgs.Empty);

            // Cleanup
            thumbnailImage?.Dispose();
            thumbnailImage = null;
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
            BackColor = Color.White;
            Size = new Size(CardWidth, CardHeight);
            DoubleBuffered = true;

            // Rounded corners region
            Region = CreateRoundedRegion();

            // Auto-dismiss timer
            autoDismissTimer = new Timer
            {
                Interval = DefaultAutoDismissTimeout,
                Enabled = false
            };
            autoDismissTimer.Tick += AutoDismissTimer_Tick;

            // Mouse events for hover detection
            MouseEnter += Overlay_MouseEnter;
            MouseLeave += Overlay_MouseLeave;

            // Paint event for custom drawing
            Paint += Overlay_Paint;

            // Create buttons (hidden by default)
            CreateButtons();
        }

        /// <summary>
        /// Creates all buttons (corner + center pills)
        /// </summary>
        private void CreateButtons()
        {
            // Top-left: Pin
            btnPin = CreateCornerButton("📌", CardPadding, CardPadding);
            btnPin.Click += (s, e) => PinClicked?.Invoke(this, EventArgs.Empty);

            // Top-right: Close
            btnClose = CreateCornerButton("✕", CardWidth - ButtonSize - CardPadding, CardPadding);
            btnClose.Click += (s, e) => Hide();

            // Bottom-left: Upload
            btnUpload = CreateCornerButton("☁", CardPadding, CardHeight - ButtonSize - CardPadding);
            btnUpload.Click += (s, e) => UploadClicked?.Invoke(this, EventArgs.Empty);

            // Bottom-right: Edit
            btnEdit = CreateCornerButton("✏", CardWidth - ButtonSize - CardPadding, CardHeight - ButtonSize - CardPadding);
            btnEdit.Click += (s, e) => AnnotateClicked?.Invoke(this, EventArgs.Empty);

            // Center: Copy (top)
            int centerX = (CardWidth - PillButtonWidth) / 2;
            int centerY = (CardHeight - (PillButtonHeight * 2 + PillSpacing)) / 2;
            btnCopy = CreatePillButton("Copy", centerX, centerY);
            btnCopy.Click += (s, e) => CopyClicked?.Invoke(this, EventArgs.Empty);

            // Center: Save (bottom)
            btnSave = CreatePillButton("Save", centerX, centerY + PillButtonHeight + PillSpacing);
            btnSave.Click += (s, e) => SaveClicked?.Invoke(this, EventArgs.Empty);

            // All buttons start hidden
            HideAllButtons();
        }

        /// <summary>
        /// Creates a circular corner button
        /// </summary>
        private Button CreateCornerButton(string text, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(ButtonSize, ButtonSize),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14),
                Cursor = Cursors.Hand,
                Visible = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);

            // Make it circular
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, ButtonSize, ButtonSize);
            btn.Region = new Region(path);

            Controls.Add(btn);
            btn.BringToFront();

            return btn;
        }

        /// <summary>
        /// Creates a pill-shaped center button
        /// </summary>
        private Button CreatePillButton(string text, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(PillButtonWidth, PillButtonHeight),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(60, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Visible = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 240, 240);

            // Make it pill-shaped (rounded ends)
            GraphicsPath path = new GraphicsPath();
            int radius = PillButtonHeight / 2;
            path.AddArc(0, 0, radius * 2, radius * 2, 90, 180);
            path.AddArc(PillButtonWidth - radius * 2, 0, radius * 2, radius * 2, 270, 180);
            path.CloseFigure();
            btn.Region = new Region(path);

            Controls.Add(btn);
            btn.BringToFront();

            return btn;
        }

        /// <summary>
        /// Custom paint for rounded card with thumbnail
        /// </summary>
        private void Overlay_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Draw thumbnail
            if (thumbnailImage != null)
            {
                // If hovering, draw darkened version
                if (isHovering)
                {
                    // Draw thumbnail
                    g.DrawImage(thumbnailImage, 0, 0, CardWidth, CardHeight);

                    // Darken overlay
                    using (SolidBrush darkBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                    {
                        g.FillRectangle(darkBrush, 0, 0, CardWidth, CardHeight);
                    }
                }
                else
                {
                    // Normal thumbnail
                    g.DrawImage(thumbnailImage, 0, 0, CardWidth, CardHeight);
                }
            }

            // Draw shadow effect (optional - would need to be drawn BEFORE the form region)
        }

        /// <summary>
        /// Creates rounded rectangle region for the form
        /// </summary>
        private Region CreateRoundedRegion()
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, CornerRadius * 2, CornerRadius * 2, 180, 90);
            path.AddArc(CardWidth - CornerRadius * 2, 0, CornerRadius * 2, CornerRadius * 2, 270, 90);
            path.AddArc(CardWidth - CornerRadius * 2, CardHeight - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 0, 90);
            path.AddArc(0, CardHeight - CornerRadius * 2, CornerRadius * 2, CornerRadius * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }

        /// <summary>
        /// Creates thumbnail from full image
        /// </summary>
        private Image CreateThumbnail(Image image)
        {
            Bitmap thumbnail = new Bitmap(CardWidth, CardHeight);
            using (Graphics g = Graphics.FromImage(thumbnail))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Calculate aspect ratio scaling
                float imgAspect = (float)image.Width / image.Height;
                float cardAspect = (float)CardWidth / CardHeight;

                Rectangle destRect;
                if (imgAspect > cardAspect)
                {
                    // Image is wider - fit to width
                    int scaledHeight = (int)(CardWidth / imgAspect);
                    int offsetY = (CardHeight - scaledHeight) / 2;
                    destRect = new Rectangle(0, offsetY, CardWidth, scaledHeight);
                }
                else
                {
                    // Image is taller - fit to height
                    int scaledWidth = (int)(CardHeight * imgAspect);
                    int offsetX = (CardWidth - scaledWidth) / 2;
                    destRect = new Rectangle(offsetX, 0, scaledWidth, CardHeight);
                }

                g.DrawImage(image, destRect);
            }
            return thumbnail;
        }

        /// <summary>
        /// Positions overlay in bottom-right corner of screen
        /// </summary>
        private void PositionInCorner()
        {
            Screen screen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            Rectangle workingArea = screen.WorkingArea;

            int margin = 20;
            int x = workingArea.Right - CardWidth - margin;
            int y = workingArea.Bottom - CardHeight - margin;

            Location = new Point(x, y);
        }

        /// <summary>
        /// Mouse enter - show buttons
        /// </summary>
        private void Overlay_MouseEnter(object? sender, EventArgs e)
        {
            isHovering = true;
            ShowAllButtons();
            Invalidate();
            ResetAutoDismissTimer();
        }

        /// <summary>
        /// Mouse leave - hide buttons
        /// </summary>
        private void Overlay_MouseLeave(object? sender, EventArgs e)
        {
            // Check if mouse actually left (not just moved to a button)
            if (!ClientRectangle.Contains(PointToClient(Cursor.Position)))
            {
                isHovering = false;
                HideAllButtons();
                Invalidate();
            }
        }

        /// <summary>
        /// Show all buttons
        /// </summary>
        private void ShowAllButtons()
        {
            btnPin!.Visible = true;
            btnClose!.Visible = true;
            btnUpload!.Visible = true;
            btnEdit!.Visible = true;
            btnCopy!.Visible = true;
            btnSave!.Visible = true;
        }

        /// <summary>
        /// Hide all buttons
        /// </summary>
        private void HideAllButtons()
        {
            btnPin!.Visible = false;
            btnClose!.Visible = false;
            btnUpload!.Visible = false;
            btnEdit!.Visible = false;
            btnCopy!.Visible = false;
            btnSave!.Visible = false;
        }

        /// <summary>
        /// Start drag-and-drop from thumbnail
        /// </summary>
        private void Thumbnail_MouseDown(object? sender, MouseEventArgs e)
        {
            // Only drag if not clicking on a button
            if (e.Button == MouseButtons.Left && !isHovering)
            {
                if (currentImage != null)
                {
                    // Start drag-and-drop operation
                    DataObject data = new DataObject();
                    data.SetData(DataFormats.Bitmap, currentImage);
                    DoDragDrop(data, DragDropEffects.Copy);
                }
            }
        }

        /// <summary>
        /// Auto-dismiss timer tick
        /// </summary>
        private void AutoDismissTimer_Tick(object? sender, EventArgs e)
        {
            if (!isHovering)
            {
                Hide();
            }
        }

        /// <summary>
        /// Reset auto-dismiss timer
        /// </summary>
        private void ResetAutoDismissTimer()
        {
            if (autoDismissTimer != null && autoDismissTimeout > 0)
            {
                autoDismissTimer.Stop();
                autoDismissTimer.Start();
            }
        }

        /// <summary>
        /// Stop auto-dismiss timer
        /// </summary>
        private void StopAutoDismissTimer()
        {
            autoDismissTimer?.Stop();
        }

        /// <summary>
        /// Clean up resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                autoDismissTimer?.Dispose();
                thumbnailImage?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

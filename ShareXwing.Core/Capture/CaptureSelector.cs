using System;
using System.Drawing;
using System.Windows.Forms;
using ShareXwing.Core.Win32;

namespace ShareXwing.Core.Capture
{
    /// <summary>
    /// All-in-One Capture selector - unified capture mode selection overlay
    /// </summary>
    public partial class CaptureSelector : Form, ICaptureSelector
    {
        private const int ButtonWidth = 200;
        private const int ButtonHeight = 50;
        private const int ButtonSpacing = 10;
        private const int Padding = 15;

        private readonly CaptureModeButton[] modeButtons;

        /// <summary>
        /// Initializes a new instance of the CaptureSelector class
        /// </summary>
        public CaptureSelector()
        {
            modeButtons = new CaptureModeButton[]
            {
                new CaptureModeButton(CaptureMode.Region, "Region", "Select area", Keys.R),
                new CaptureModeButton(CaptureMode.Window, "Window", "Capture window", Keys.W),
                new CaptureModeButton(CaptureMode.Fullscreen, "Fullscreen", "Entire screen", Keys.F),
                new CaptureModeButton(CaptureMode.ActiveMonitor, "Active Monitor", "Current monitor", Keys.M),
                new CaptureModeButton(CaptureMode.LastRegion, "Last Region", "Repeat last", Keys.L),
                new CaptureModeButton(CaptureMode.ScrollingCapture, "Scrolling", "Long capture", Keys.S)
            };

            InitializeComponent();
            CreateModeButtons();
        }

        /// <inheritdoc/>
        public bool IsVisible => Visible;

        /// <inheritdoc/>
        public event EventHandler<CaptureMode>? CaptureModeSelected;

        /// <inheritdoc/>
        public event EventHandler? Cancelled;

        /// <inheritdoc/>
        public new void Show()
        {
            PositionAtCursor();
            base.Show();
            WindowHelper.SetTopMost(Handle, true);
            Activate();
        }

        /// <inheritdoc/>
        public new void Hide()
        {
            base.Hide();
        }

        /// <summary>
        /// Initializes the form components
        /// </summary>
        private void InitializeComponent()
        {
            // Form properties
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(45, 45, 48); // Dark theme
            Opacity = 0.95;

            // Calculate size based on button count
            int formHeight = (modeButtons.Length * ButtonHeight) + ((modeButtons.Length - 1) * ButtonSpacing) + (Padding * 2);
            Size = new Size(ButtonWidth + (Padding * 2), formHeight);

            // Border for visual clarity
            Paint += CaptureSelector_Paint;

            // Keyboard handling
            KeyPreview = true;
            KeyDown += CaptureSelector_KeyDown;

            // Click outside to dismiss
            Deactivate += CaptureSelector_Deactivate;
        }

        /// <summary>
        /// Creates the capture mode buttons
        /// </summary>
        private void CreateModeButtons()
        {
            int y = Padding;

            foreach (var modeButton in modeButtons)
            {
                var button = CreateButton(modeButton, y);
                Controls.Add(button);
                y += ButtonHeight + ButtonSpacing;
            }
        }

        /// <summary>
        /// Creates a styled button for a capture mode
        /// </summary>
        private Button CreateButton(CaptureModeButton modeButton, int y)
        {
            var button = new Button
            {
                Text = $"{modeButton.Label}\n{modeButton.Description}  ({modeButton.Hotkey})",
                Location = new Point(Padding, y),
                Size = new Size(ButtonWidth, ButtonHeight),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(63, 63, 70),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Tag = modeButton.Mode
            };

            button.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 122, 204);

            button.Click += ModeButton_Click;
            button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(0, 122, 204);
            button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(63, 63, 70);

            return button;
        }

        /// <summary>
        /// Positions the selector at the cursor with screen edge detection
        /// </summary>
        private void PositionAtCursor()
        {
            Point cursorPos = Cursor.Position;
            Screen screen = Screen.FromPoint(cursorPos);
            Rectangle screenBounds = screen.WorkingArea;

            // Default: bottom-right of cursor
            int x = cursorPos.X + 20;
            int y = cursorPos.Y + 20;

            // Check right edge
            if (x + Width > screenBounds.Right)
            {
                x = cursorPos.X - Width - 20;
            }

            // Check bottom edge
            if (y + Height > screenBounds.Bottom)
            {
                y = cursorPos.Y - Height - 20;
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

        private void ModeButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is CaptureMode mode)
            {
                SelectMode(mode);
            }
        }

        private void CaptureSelector_KeyDown(object? sender, KeyEventArgs e)
        {
            // ESC to cancel
            if (e.KeyCode == Keys.Escape)
            {
                Cancel();
                e.Handled = true;
                return;
            }

            // Check for mode hotkeys
            foreach (var modeButton in modeButtons)
            {
                if (e.KeyCode == modeButton.Hotkey)
                {
                    SelectMode(modeButton.Mode);
                    e.Handled = true;
                    return;
                }
            }
        }

        private void CaptureSelector_Deactivate(object? sender, EventArgs e)
        {
            // Hide when clicking outside
            Cancel();
        }

        private void CaptureSelector_Paint(object? sender, PaintEventArgs e)
        {
            // Draw border
            using (var pen = new Pen(Color.FromArgb(0, 122, 204), 2))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }

        private void SelectMode(CaptureMode mode)
        {
            Hide();
            CaptureModeSelected?.Invoke(this, mode);
        }

        private void Cancel()
        {
            Hide();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary>
        /// Helper class for capture mode button data
        /// </summary>
        private class CaptureModeButton
        {
            public CaptureMode Mode { get; }
            public string Label { get; }
            public string Description { get; }
            public Keys Hotkey { get; }

            public CaptureModeButton(CaptureMode mode, string label, string description, Keys hotkey)
            {
                Mode = mode;
                Label = label;
                Description = description;
                Hotkey = hotkey;
            }
        }
    }
}

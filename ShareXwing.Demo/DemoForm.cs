using System;
using System.Drawing;
using System.Windows.Forms;
using ShareXwing.Core.FloatingWindows;

namespace ShareXwing.Demo;

/// <summary>
/// Demo form for testing FloatingWindow functionality.
/// </summary>
public class DemoForm : Form
{
    private readonly FloatingWindowManager windowManager;
    private Button? btnCreateWindow;
    private Button? btnCloseAll;
    private Label? lblActiveCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemoForm"/> class.
    /// </summary>
    public DemoForm()
    {
        windowManager = new FloatingWindowManager();
        InitializeComponents();
        UpdateActiveCount();
    }

    /// <summary>
    /// Initializes the form components.
    /// </summary>
    private void InitializeComponents()
    {
        Text = "ShareXwing Floating Window Demo";
        ClientSize = new Size(400, 200);
        StartPosition = FormStartPosition.CenterScreen;

        // Create floating window button
        btnCreateWindow = new Button
        {
            Text = "Create Floating Window",
            Location = new Point(20, 20),
            Size = new Size(360, 40),
            Font = new Font("Segoe UI", 12F)
        };
        btnCreateWindow.Click += BtnCreateWindow_Click;
        Controls.Add(btnCreateWindow);

        // Close all button
        btnCloseAll = new Button
        {
            Text = "Close All Windows",
            Location = new Point(20, 70),
            Size = new Size(360, 40),
            Font = new Font("Segoe UI", 12F)
        };
        btnCloseAll.Click += BtnCloseAll_Click;
        Controls.Add(btnCloseAll);

        // Active count label
        lblActiveCount = new Label
        {
            Location = new Point(20, 120),
            Size = new Size(360, 40),
            Font = new Font("Segoe UI", 10F),
            TextAlign = ContentAlignment.MiddleLeft
        };
        Controls.Add(lblActiveCount);

        // Instructions label
        var lblInstructions = new Label
        {
            Text = "Right-click floating windows for options:\n• Opacity control\n• Lock (click-through) toggle\n• Close",
            Location = new Point(20, 160),
            Size = new Size(360, 60),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(lblInstructions);
    }

    /// <summary>
    /// Handles create window button click.
    /// </summary>
    private void BtnCreateWindow_Click(object? sender, EventArgs e)
    {
        try
        {
            // Create a test image with gradient
            var image = CreateTestImage();

            // Create and show floating window
            var window = new FloatingWindow(image);
            windowManager.RegisterWindow(window);

            // Handle window close to unregister
            window.FormClosed += (s, args) =>
            {
                windowManager.UnregisterWindow(window);
                UpdateActiveCount();
            };

            window.Show();
            UpdateActiveCount();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error creating floating window: {ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Handles close all button click.
    /// </summary>
    private void BtnCloseAll_Click(object? sender, EventArgs e)
    {
        windowManager.CloseAll();
        UpdateActiveCount();
    }

    /// <summary>
    /// Updates the active window count display.
    /// </summary>
    private void UpdateActiveCount()
    {
        if (lblActiveCount != null)
        {
            lblActiveCount.Text = $"Active floating windows: {windowManager.ActiveCount}";
        }
    }

    /// <summary>
    /// Creates a test image with gradient and text.
    /// </summary>
    private static Image CreateTestImage()
    {
        int width = 400;
        int height = 300;
        var bitmap = new Bitmap(width, height);

        using (var graphics = Graphics.FromImage(bitmap))
        {
            // Gradient background
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, width, height),
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(41, 128, 185),
                45F))
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
            }

            // Draw text
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var font = new Font("Segoe UI", 24F, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            {
                var text = "ShareXwing\nFloating Window";
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                graphics.DrawString(text, font, textBrush, new RectangleF(0, 0, width, height), format);
            }

            // Draw border
            using (var pen = new Pen(Color.White, 4))
            {
                graphics.DrawRectangle(pen, 2, 2, width - 4, height - 4);
            }
        }

        return bitmap;
    }

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            windowManager?.CloseAll();
        }

        base.Dispose(disposing);
    }
}

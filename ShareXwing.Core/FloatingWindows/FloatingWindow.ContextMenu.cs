using System;
using System.Windows.Forms;

namespace ShareXwing.Core.FloatingWindows;

/// <summary>
/// Context menu functionality for FloatingWindow.
/// </summary>
public partial class FloatingWindow
{
    private ContextMenuStrip? contextMenu;
    private ToolStripMenuItem? opacityMenuItem;
    private ToolStripMenuItem? lockMenuItem;

    /// <summary>
    /// Creates and configures the context menu.
    /// </summary>
    private void CreateContextMenu()
    {
        contextMenu = new ContextMenuStrip();

        // Opacity submenu
        opacityMenuItem = new ToolStripMenuItem("Opacity");
        CreateOpacityMenuItems(opacityMenuItem);
        contextMenu.Items.Add(opacityMenuItem);

        // Separator
        contextMenu.Items.Add(new ToolStripSeparator());

        // Lock toggle
        lockMenuItem = new ToolStripMenuItem("Lock (Click-Through)")
        {
            CheckOnClick = true,
            Checked = isLocked
        };
        lockMenuItem.Click += LockMenuItem_Click;
        contextMenu.Items.Add(lockMenuItem);

        // Separator
        contextMenu.Items.Add(new ToolStripSeparator());

        // Close
        var closeMenuItem = new ToolStripMenuItem("Close");
        closeMenuItem.Click += (s, e) => Close();
        contextMenu.Items.Add(closeMenuItem);

        // Attach to form
        ContextMenuStrip = contextMenu;
    }

    /// <summary>
    /// Creates opacity submenu items (10%, 25%, 50%, 75%, 100%).
    /// </summary>
    private void CreateOpacityMenuItems(ToolStripMenuItem parent)
    {
        double[] opacityValues = { 0.10, 0.25, 0.50, 0.75, 1.0 };

        foreach (double opacity in opacityValues)
        {
            var item = new ToolStripMenuItem($"{opacity * 100:0}%")
            {
                Tag = opacity,
                Checked = Math.Abs(currentOpacity - opacity) < 0.01
            };
            item.Click += OpacityMenuItem_Click;
            parent.DropDownItems.Add(item);
        }
    }

    /// <summary>
    /// Handles opacity menu item clicks.
    /// </summary>
    private void OpacityMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem item && item.Tag is double opacity)
        {
            SetOpacity(opacity);
            UpdateOpacityMenuChecks();
        }
    }

    /// <summary>
    /// Handles lock menu item clicks.
    /// </summary>
    private void LockMenuItem_Click(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem item)
        {
            SetLocked(item.Checked);
        }
    }

    /// <summary>
    /// Updates check marks on opacity menu items.
    /// </summary>
    private void UpdateOpacityMenuChecks()
    {
        if (opacityMenuItem?.DropDownItems == null)
        {
            return;
        }

        foreach (ToolStripItem item in opacityMenuItem.DropDownItems)
        {
            if (item is ToolStripMenuItem menuItem && menuItem.Tag is double opacity)
            {
                menuItem.Checked = Math.Abs(currentOpacity - opacity) < 0.01;
            }
        }
    }

    /// <summary>
    /// Updates the lock menu item checked state.
    /// </summary>
    private void UpdateLockMenuCheck()
    {
        if (lockMenuItem != null)
        {
            lockMenuItem.Checked = isLocked;
        }
    }
}

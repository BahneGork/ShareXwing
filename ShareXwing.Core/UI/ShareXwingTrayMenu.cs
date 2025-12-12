#region License Information (GPL v3)

/*
    ShareXwing - Enhanced screenshot tool based on ShareX
    Copyright (c) 2007-2025 ShareX Team
    Copyright (c) 2025 ShareXwing Contributors

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Windows.Forms;

namespace ShareXwing.Core.UI
{
    /// <summary>
    /// Manages the simplified CleanShot X-inspired tray menu for ShareXwing.
    /// Provides minimal, essential operations while keeping advanced features accessible.
    /// </summary>
    public class ShareXwingTrayMenu
    {
        private readonly object mainForm;

        public ShareXwingTrayMenu(object mainForm)
        {
            this.mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public ContextMenuStrip CreateSimpleMenu()
        {
            var menu = new ContextMenuStrip();

            // Title
            var title = new ToolStripLabel("ShareXwing v18.0.2");
            title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            menu.Items.Add(title);
            menu.Items.Add(new ToolStripSeparator());

            // All-in-One Capture
            var allInOne = new ToolStripMenuItem("All-in-One Capture");
            allInOne.Click += AllInOne_Click;
            menu.Items.Add(allInOne);

            menu.Items.Add(new ToolStripSeparator());

            // Capture Area
            var area = new ToolStripMenuItem("Capture Area");
            area.Click += CaptureArea_Click;
            menu.Items.Add(area);

            // Capture Window
            var window = new ToolStripMenuItem("Capture Window");
            window.Click += CaptureWindow_Click;
            menu.Items.Add(window);

            // Capture Fullscreen
            var fullscreen = new ToolStripMenuItem("Capture Fullscreen");
            fullscreen.Click += CaptureFullscreen_Click;
            menu.Items.Add(fullscreen);

            // Capture Scrolling
            var scrolling = new ToolStripMenuItem("Capture Scrolling");
            scrolling.Click += CaptureScrolling_Click;
            menu.Items.Add(scrolling);

            menu.Items.Add(new ToolStripSeparator());

            // Screen Recording (submenu)
            var recording = new ToolStripMenuItem("Screen Recording");

            var recordFFmpeg = new ToolStripMenuItem("Record with FFmpeg");
            recordFFmpeg.Click += RecordFFmpeg_Click;
            recording.DropDownItems.Add(recordFFmpeg);

            var recordGIF = new ToolStripMenuItem("Record as GIF");
            recordGIF.Click += RecordGIF_Click;
            recording.DropDownItems.Add(recordGIF);

            menu.Items.Add(recording);

            menu.Items.Add(new ToolStripSeparator());

            // Recent Captures
            var recent = new ToolStripMenuItem("Recent Captures");
            recent.Click += RecentCaptures_Click;
            menu.Items.Add(recent);

            // Pin Screenshot
            var pin = new ToolStripMenuItem("Pin Screenshot");
            pin.Click += PinScreenshot_Click;
            menu.Items.Add(pin);

            menu.Items.Add(new ToolStripSeparator());

            // Advanced Features
            var advanced = new ToolStripMenuItem("Advanced Features...");
            advanced.Click += AdvancedFeatures_Click;
            menu.Items.Add(advanced);

            // Settings
            var settings = new ToolStripMenuItem("Settings...");
            settings.Click += Settings_Click;
            menu.Items.Add(settings);

            menu.Items.Add(new ToolStripSeparator());

            // Quit
            var quit = new ToolStripMenuItem("Quit ShareXwing");
            quit.Click += Quit_Click;
            menu.Items.Add(quit);

            return menu;
        }

        #region Event Handlers

        private void AllInOne_Click(object sender, EventArgs e)
        {
            // Call TaskHelpers.ShowCaptureSelector() via reflection to avoid direct dependency
            InvokeStaticMethod("ShareX.TaskHelpers", "ShowCaptureSelector", null);
        }

        private void CaptureArea_Click(object sender, EventArgs e)
        {
            // new CaptureRegion().Capture(taskSettings)
            InvokeCaptureMethod("ShareX.ScreenCaptureLib.CaptureRegion");
        }

        private void CaptureWindow_Click(object sender, EventArgs e)
        {
            // new CaptureCustomWindow().Capture(taskSettings)
            InvokeCaptureMethod("ShareX.ScreenCaptureLib.CaptureCustomWindow");
        }

        private void CaptureFullscreen_Click(object sender, EventArgs e)
        {
            // new CaptureFullscreen().Capture(taskSettings)
            InvokeCaptureMethod("ShareX.ScreenCaptureLib.CaptureFullscreen");
        }

        private void CaptureScrolling_Click(object sender, EventArgs e)
        {
            InvokeStaticMethod("ShareX.TaskHelpers", "OpenScrollingCapture", null);
        }

        private void RecordFFmpeg_Click(object sender, EventArgs e)
        {
            InvokeStaticMethod("ShareX.TaskHelpers", "StartScreenRecording",
                new object[] { GetScreenRecordOutput("FFmpeg"), null, null });
        }

        private void RecordGIF_Click(object sender, EventArgs e)
        {
            InvokeStaticMethod("ShareX.TaskHelpers", "StartScreenRecording",
                new object[] { GetScreenRecordOutput("GIF"), null, null });
        }

        private void RecentCaptures_Click(object sender, EventArgs e)
        {
            InvokeStaticMethod("ShareX.TaskHelpers", "OpenHistory", null);
        }

        private void PinScreenshot_Click(object sender, EventArgs e)
        {
            InvokeStaticMethod("ShareX.TaskHelpers", "OpenPinToScreen", null);
        }

        private void AdvancedFeatures_Click(object sender, EventArgs e)
        {
            InvokeMainFormMethod("ShowFullTrayMenu");
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            InvokeMainFormMethod("ForceActivate");
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            InvokeMainFormMethod("ForceClose");
        }

        #endregion

        #region Helper Methods

        private void InvokeCaptureMethod(string typeName)
        {
            try
            {
                var type = Type.GetType(typeName);
                if (type != null)
                {
                    var instance = Activator.CreateInstance(type);
                    var captureMethod = type.GetMethod("Capture", new Type[] { });
                    if (captureMethod != null)
                    {
                        captureMethod.Invoke(instance, new object[] { null });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invoking capture method {typeName}: {ex.Message}");
            }
        }

        private void InvokeStaticMethod(string className, string methodName, object[] parameters)
        {
            try
            {
                var type = Type.GetType(className);
                if (type != null)
                {
                    var method = type.GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, parameters);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invoking static method {className}.{methodName}: {ex.Message}");
            }
        }

        private void InvokeMainFormMethod(string methodName)
        {
            try
            {
                var method = mainForm.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(mainForm, null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error invoking MainForm method {methodName}: {ex.Message}");
            }
        }

        private object GetScreenRecordOutput(string outputType)
        {
            try
            {
                var type = Type.GetType("ShareX.ScreenCaptureLib.ScreenRecordOutput, ShareX.ScreenCaptureLib");
                if (type != null && type.IsEnum)
                {
                    return Enum.Parse(type, outputType);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting ScreenRecordOutput: {ex.Message}");
            }
            return null;
        }

        #endregion
    }
}

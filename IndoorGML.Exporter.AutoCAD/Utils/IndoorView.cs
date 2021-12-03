using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CityGML.Exporter.AutoCAD.Utils
{
    class IndoorView
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        //this is a constant indicating the window that we want to send a text message
        private const int WM_OPENFILE = 1110;

        public static void ViewIndoorGML(string outputFile)
        {
            if (File.Exists(outputFile))
            {
                var process = Process.GetProcessesByName("InviewerDesktopGUI");
                if (process != null && process.Length > 0)
                {
                    string file = Path.Combine(Path.GetTempPath(), "IndoorGML.tmp");
                    File.WriteAllText(file, outputFile);
                    SendMessage(process[0].MainWindowHandle, WM_OPENFILE, 0, 0);
                }
                else
                {

                    Process p = new Process();
                    p.StartInfo.FileName = GetViewerPath();
                    p.StartInfo.Arguments = $"\"{outputFile}\"";
                    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
                    p.Start();

                }
            }
        }

        public static string GetViewerPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "IndoorViewer\\InviewerDesktopGUI.exe");
        }
    }
}

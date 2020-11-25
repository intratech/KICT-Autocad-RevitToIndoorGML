using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AutoCadPluginRegister
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {                        
            Environment.ExitCode = 0;

            var mode = string.Empty;
            if (!GetParameterValue(args, "-mode", out mode))
            {
                Environment.ExitCode = 1;
                return;
            }

            var pluginRegKey = string.Empty;
            if (!GetParameterValue(args, "-regKey", out pluginRegKey))
            {
                Environment.ExitCode = 1;
                return;
            }

            if (mode == "install")
            {
                var dllName = string.Empty;
                if (!GetParameterValue(args, "-dllName", out dllName))
                {
                    Environment.ExitCode = 1;
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SelectVersionForm(pluginRegKey, dllName));
                //new SelectVersionForm(pluginRegKey, dllName).Show();
            }
            else if (mode == "uninstall")
            {
                new Uninstaller(pluginRegKey).Run();
            }
        }

        private static bool GetParameterValue(string[] args, string compareString, out string value)
        {
            value = "";
            var isGetValue = false;

            foreach (var arg in args)
            {
                if (isGetValue)
                {
                    if (!arg.StartsWith("-"))
                    {
                        value = arg;
                    }
                    break;
                }
                if (!arg.StartsWith("-"))
                    continue;

                if (arg == compareString)
                {
                    isGetValue = true;
                    continue;
                }
            }

            if (value == "")
                return false;

            return true;
        }
    }
}

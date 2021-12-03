using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CityGMLExporter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            var exitCode = 0;
            try
            {               
                if (args.Length == 2)
                {
                    Exporter exporter = new Exporter();

                    string jsonPath = args[0];
                    string savePath = args[1];
                    if (!File.Exists(jsonPath))
                    {
                        Console.WriteLine("Json file is not found!");
                        exitCode = 1;
                        Environment.ExitCode = exitCode;
                        return;
                    }                    
                    if (!savePath.EndsWith(".gml"))
                    {
                        Console.WriteLine("File extension is incorrect!");
                        exitCode = 1;
                        Environment.ExitCode = exitCode;
                        return;
                    }

                    exporter.exportCityGMLFromJson(jsonPath, savePath);
                }
                else
                {
                    Console.WriteLine("Parameters are missed!");
                    exitCode = 1;
                    Environment.ExitCode = exitCode;
                    return;
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                exitCode = 1;
                Environment.ExitCode = exitCode;
                return;
            }
        }
    }
}

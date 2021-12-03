using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using CityGML.Exporter.AutoCAD.Utils;
using System;
using System.IO;
using System.Windows.Media.Imaging;

using Autodesk.AutoCAD.Windows;
using CityGML.Exporter.AutoCAD;
using IndoorGML.Exporter.AutoCAD.Command;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

[assembly: ExtensionApplication(typeof(IndoorGML.Exporter.AutoCAD.Plugin))]
namespace IndoorGML.Exporter.AutoCAD
{
    public class Plugin : IExtensionApplication
    {
        public static IndoorGMLPalette tvp;
        public void Initialize()
        {
            try
            {
                tvp = new IndoorGMLPalette();
                Application.Idle += new System.EventHandler(Application_Idle);
            }
            catch(Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.Message);
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.ShowAlertDialog(ex.Message);
            }
        }


        void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new System.EventHandler(Application_Idle);
            InitRibbon();
        }

        public void InitRibbon() {
            var tab = UICreator.CreateRibbonTab("IndoorGML-Intra", "IndoorGML");
            var panel = UICreator.CreateRibbonPanel(tab, "IndoorGML-Panel", "Exporter");

            var largeImage = PngImageSource("IndoorGML.Exporter.AutoCAD.Resources.export.bmp");
            var smallImage = PngImageSource("IndoorGML.Exporter.AutoCAD.Resources.export_16.bmp");
            var buttonRoomPicker = UICreator.CreateRibbonButton(panel, "IndoorGML-Exporter", "Export",
                largeImage,
                smallImage);
            if (buttonRoomPicker.CommandHandler == null)
            {
                buttonRoomPicker.CommandHandler = new IndoorGMLCommand();
                //buttonRoomPicker.
            }
        }

        public void Terminate()
        {
            try
            {
                RegSetting.Close();
            }
            catch { }
            //throw new NotImplementedException();
        }
        private System.Windows.Media.ImageSource PngImageSource(string embeddedPath)
        {
            try
            {
                Stream stream = this.GetType().Assembly.GetManifestResourceStream(embeddedPath);
                //var decoder = new System.Windows.Media.Imaging.PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return BitmapFrame.Create(stream);
            }
            catch 
            {
                return null;
            }
        }



    }
}

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using IndoorGML.Exporter.Revit.UI;
using IndoorGML.Exporter.Revit.Utils;

namespace IndoorGML.Exporter.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class Plugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            new MainWindow(uiapp).ShowDialog();

            return Result.Succeeded;
        }
       
        private void Export2D()
        {
            var outputFile = IOUtils.ShowSaveFileDialog();
            if (string.IsNullOrEmpty(outputFile))
            {
                return;
            }

            var ex = new TwoDExporter();
        }
        
    }
}

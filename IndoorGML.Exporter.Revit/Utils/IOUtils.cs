using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class IOUtils
    {
        public static string ShowSelectFolderDialog()
        {
            var dialog = new CommonOpenFileDialog() { IsFolderPicker = true };

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
        public static string ShowSaveFileDialog()
        {
            var dialog = new CommonSaveFileDialog();

            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }
        public static string GetFileName(string filePath, bool withoutExtension = false)
        {
            return withoutExtension ? Path.GetFileNameWithoutExtension(filePath) : Path.GetFileName(filePath);
        }
        public static string GetCurrentExecutingAssemplyLocation()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}

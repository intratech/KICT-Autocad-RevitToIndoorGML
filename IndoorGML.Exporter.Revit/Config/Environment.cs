using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Config
{
    public static class Environment
    {

        public static string CONST_NOT_PLACE = "Not Placed";

        public static readonly string AppSettingFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AppSettings.xml");

        public const string COLUMN_LEVEL = "Level";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_NUMBER = "Number";
        public const string COLUMN_MARK = "Mark";
        public const string COLUMN_TYPE = "Type";
        public const string COLUMN_FROM_ROOM = "From Room Name";
        public const string COLUMN_TO_ROOM = "To Room Name";
    }
}

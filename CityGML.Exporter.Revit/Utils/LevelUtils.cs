using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
    public static class LevelUtils
    {
        public static IEnumerable<Level> GetLevelsByRooms(Document doc)
        {
            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().Cast<Room>();
            return rooms.GroupBy(r => r.LevelId).Select(l => doc.GetElement(l.Key) as Level);
        }
        public static IEnumerable<Level> GetLevels(Document doc) {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).WhereElementIsNotElementType().Cast<Level>();
        }
    }
}

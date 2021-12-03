using Autodesk.Revit.DB;
using GML.Core.DTO.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Point3DIntra;
using IndoorGML.Exporter.Revit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Exporter
{
    public class TestExporter
    {
        public void Export(Document doc)
        {
            var rooms = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Rooms);
            var doors = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Doors);
            var walls = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Walls);
            var windows = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Windows);

            var output = @"G:\revit.json";
            //var wt = new StreamWriter(output);
            var jArray = new JArray();

            foreach (var e in windows)
            {
                var wd = e as FamilyInstance;
                if (wd == null)
                    continue;

                Point3D j_location = null;
                var loc = wd.Location;
                
                if(loc != null)
                {
                    var point = loc as LocationPoint;
                    if (point != null)
                    {
                        j_location = new Point3D(point.Point.X, point.Point.Y, point.Point.Z);
                    }
                }

                //Level lv = null;
                //lv.


                jArray.Add(JObject.FromObject(new {
                    name = wd.Name,
                    location = j_location
                }));
            }

            File.WriteAllText(output, jArray.ToString());
        }
    }
}

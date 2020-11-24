using Autodesk.Revit.DB;
using GML.Core.DTO.Json;
using Newtonsoft.Json;
using Point3DIntra;
using IndoorGML.Exporter.Revit.Exporter;
using IndoorGML.Exporter.Revit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using IndoorGML.Adapter;
using System.Windows;

namespace IndoorGML.Exporter.Revit
{
    public class TwoDExporter
    {
        //include SendMessage
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

     
        //this is a constant indicating the window that we want to send a text message
        private const int WM_OPENFILE = 1110;

        public TwoDExporter() {
            
        }

        public void Export(Document doc, string outputFile, IEnumerable<int> levelIds, double configWallThickness)
        {
            Log.Info("Querying rooms");
            var rooms = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Rooms).Where(i => levelIds.Contains(i.LevelId.IntegerValue));
            Log.Info("Room count: "+rooms.Count());

            Log.Info("Query doors");
            var doors = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Doors).Where(i => levelIds.Contains(i.LevelId.IntegerValue));
            Log.Info("Doors: " + doors.Count());

            //Log.Info("Query walls");
            //var walls = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Walls).Where(i => levelIds.Contains(i.LevelId.IntegerValue));
            //Log.Info("Walls count: " + walls.Count());

            Log.Info("Query windows");
            var windows = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_Windows).Where(i => levelIds.Contains(i.LevelId.IntegerValue));
            Log.Info("Windows count: "+ windows.Count());

            Log.Info("Room seperators");
            var roomSeperators = ElementUtils.QueryElementsOfCategory(doc, BuiltInCategory.OST_RoomSeparationLines).Where(i => levelIds.Contains(i.LevelId.IntegerValue));
            Log.Info("Room seperator count: " + roomSeperators.Count());


            Phase threeDViewPhase = null;

            Log.Info("Creating temporary view 3D");
            var view3d = ViewUtils.GetView3D(doc);
            var phases = ViewUtils.GetPhases(doc);
            if (view3d != null)
            {
                var _phaseName = view3d.get_Parameter(BuiltInParameter.VIEW_PHASE);
                if(_phaseName != null)
                {
                    threeDViewPhase = phases.Where(p => p.Name == _phaseName.AsValueString()).FirstOrDefault();                 
                }
            }
            
            var jsonEx = new JsonExporter();
            BoundingBoxXYZ roomsBoundingBox;


            Log.Info("Extracting spatial data (room,door)");
            var jRooms = jsonEx.ExportRoomsEx(rooms, doc, out roomsBoundingBox);
            var jDoors = jsonEx.ExportDoorsEx(doors, jRooms, phases);
            var jWindows = jsonEx.ExportWindowsEx(windows, jRooms, threeDViewPhase);

            Log.Info("Extracting virtual door based on room seperators");
            var jsSeperator = jsonEx.ExportModelLineToDoor(roomSeperators, jRooms, threeDViewPhase);
            jDoors.AddRange(jsSeperator);


            //Levels
            var j_levels = rooms.GroupBy(r => r.LevelId).Select(lv => new JLevel {
                id = lv.Key.IntegerValue,
                name = doc.GetElement(lv.Key).Name,
                elevation = (doc.GetElement(lv.Key) as Level).Elevation
            }).OrderBy(lv => lv.elevation);

            Log.Info("Generating Json Model");
            var jModel = new JModel
            {
                model_name = IOUtils.GetFileName(doc.PathName, false),
                bounding_box = new JBoundingBox
                {
                    min = new Point3D(roomsBoundingBox.Min.X, roomsBoundingBox.Min.Y, roomsBoundingBox.Min.Z),
                    max = new Point3D(roomsBoundingBox.Max.X, roomsBoundingBox.Max.Y, roomsBoundingBox.Max.Z)
                },
                levels = j_levels.ToList(),
                rooms = jRooms,
                doors = jDoors,
                windows = jWindows,
                scale = UnitUtils.ConvertFromInternalUnits(roomsBoundingBox.Max.Z, DisplayUnitType.DUT_MILLIMETERS) / roomsBoundingBox.Max.Z
            };


            
            if(outputFile.EndsWith(".gml", StringComparison.OrdinalIgnoreCase))
            {
                GML.Core.Config.DefaultThickness = configWallThickness / jModel.scale;

                Log.Info("Save as " +outputFile);
                JModel2Indoor indoorConverter = new JModel2Indoor();
                indoorConverter.Convert(jModel).Save(outputFile);

                Log.Info("View model " + outputFile);

                MessageBox.Show("Export finished!", "Revit to IndoorGML", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if(outputFile.EndsWith(".json",StringComparison.OrdinalIgnoreCase))
            {
                Log.Info("Save as " + outputFile);
                File.WriteAllText(outputFile, JsonConvert.SerializeObject(jModel));
            }           
        }

        

        private string GetViewerPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "./IndoorViewer/InviewerDesktopGUI.exe");
        }

    
    }
}

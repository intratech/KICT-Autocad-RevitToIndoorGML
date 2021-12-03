using IndoorGML.Exporter.Revit.Entities;
using IndoorGML.Exporter.Revit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Exporter
{
    public class ExcelExporter
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void ExportRoomSchedule(IEnumerable<RoomScheduleItem> rooms, string outputFile)
        {
            try
            {
                using (var wt = new StreamWriter(outputFile, false, Encoding.UTF8))
                {
                    wt.WriteLine("No, Level, Name, Number");

                    var no = 1;
                    foreach (var room in rooms)
                    {
                        wt.WriteLine($"{++no},{room.Level},{room.Name},{room.Number}");
                    }
                    wt.Flush();
                    wt.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }
        public void ExportDoorSchedule(IEnumerable<DoorScheduleItem> doors, string outputFile)
        {
            try
            {
                using (var wt = new StreamWriter(outputFile, false, Encoding.UTF8))
                {
                    wt.WriteLine("No, Level, Mark, Type, From Room Name, To Room Name");

                    var no = 1;
                    foreach (var door in doors)
                    {
                        wt.WriteLine($"{++no},{door.Level},{door.Mark},{door.Type},{door.FromRoom},{door.ToRoom}");
                    }
                    wt.Flush();
                    wt.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }
    }
}

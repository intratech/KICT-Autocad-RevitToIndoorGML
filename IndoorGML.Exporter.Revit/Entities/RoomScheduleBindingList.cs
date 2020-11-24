using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Entities
{
    public class RoomScheduleBindingList : List<RoomScheduleItem>
    {
        public List<ColumnFilter> Filters { get; set; }

        public RoomScheduleBindingList()
        {
            this.Filters = new List<ColumnFilter>();
        }
    }
    public class DoorScheduleBindingList : List<DoorScheduleItem>
    {
        public List<ColumnFilter> Filters { get; set; }

        public DoorScheduleBindingList()
        {
            this.Filters = new List<ColumnFilter>();
        }
    }

    public class ColumnFilter
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
        public string ColumnName { get; set; }

        public ColumnFilter()
        {
            System.Threading.Thread.Sleep(1);
            Id = Guid.NewGuid().ToString();
        }
        public ColumnFilter(string text, string colName, bool active = true)
        {
            System.Threading.Thread.Sleep(1);
            Id = Guid.NewGuid().ToString();

            this.Text = text;
            this.ColumnName = colName;
            this.Active = active;
        }
    }
}

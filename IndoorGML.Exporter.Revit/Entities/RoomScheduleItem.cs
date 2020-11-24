using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Entities
{
    public class RoomScheduleItem
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public bool HighLight { get; set; }
        public string Area { get; set; }
        public string Location { get; set; }

        public RoomScheduleItem()
        {

        }
        public RoomScheduleItem(int id, string level, string name, int number, bool highLight = false)
        {
            this.Id = id;
            Level = level;
            Name = name;
            Number = number;
            HighLight = highLight;
        }
    }
    public class DoorScheduleItem
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public int? Mark { get; set; }
        public string Type { get; set; }
        public string FromRoom { get; set; }
        public string ToRoom { get; set; }
        public bool HighLight { get; set; }

        public DoorScheduleItem()
        {

        }
    }
    public class CheckBoxModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public bool IsChecked { get; set; }

        public CheckBoxModel()
        {

        }
        public CheckBoxModel(string id, string text, bool isChecked = false)
        {
            this.Id = id;
            this.Text = text;
            this.IsChecked = isChecked;
        }
    }

}
